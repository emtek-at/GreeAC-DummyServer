using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyServer
{
    public class AsynchronousSocketListener
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            // Establish the local endpoint for the socket.  
            IPAddress ipAddress = IPAddress.Any; 
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);


            // Bind the socket to the local endpoint and listen for incoming connections.  
            while (true)
            {
                try
                {
                    // Create a TCP/IP socket.  
                    Socket listener = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);
                    
                    listener.Bind(localEndPoint);
                    listener.Listen(100);

                    Console.WriteLine("GreeAC DummyServer Started");
                    Console.WriteLine("Domainname for AC Devices: " + Program.m_DomainName);
                    Console.WriteLine("IP Address for AC Devices: " + Program.m_ExternalIp);

                    while (true)
                    {
                        try
                        {
                            // Set the event to nonsignaled state.  
                            allDone.Reset();

                            // Start an asynchronous socket to listen for connections.  
                            Console.WriteLine("Waiting for a connection...");
                            listener.BeginAccept(
                                new AsyncCallback(AcceptCallback),
                                listener);

                            // Wait until a connection is made before continuing.  
                            allDone.WaitOne();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            
            Console.WriteLine("Accepted Connection from "+handler.RemoteEndPoint.ToString());

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            bool bReadMore = true;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                //content = Encoding.ASCII.GetString(state.buffer);
                content = state.sb.ToString();
                //Console.WriteLine("read: "+content);
                if (content.IndexOf("\n") > -1)
                {
                    // All the data has been read from the client.
                    state.sb = new StringBuilder();
                    GreeHandlerResponse response = GreeHandler.process(content);
                    bReadMore = response.keepAlive;
                    state.keepAlive = response.keepAlive;
                    
                    if (response.text != "")
                    {
                        Send(state, response.text);
                    }
                    else if (!response.keepAlive)
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        Console.WriteLine("Connection shutdown");
                    }

                    /*if (response.keepAlive)
                    {
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                    }
                    */
                }
            }
            
            if(bReadMore)
            {
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
        }

        private static void Send(StateObject state, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            byte[] newArray = new byte[byteData.Length + 1];
            byteData.CopyTo(newArray, 0);
            newArray[byteData.Length] = 0x0a;
            
            Socket handler = state.workSocket;

            // Begin sending the data to the remote device.  
            handler.BeginSend(newArray, 0, newArray.Length, 0,
                new AsyncCallback(SendCallback), state);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                StateObject state = (StateObject) ar.AsyncState;
                Socket handler = state.workSocket;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                if (!state.keepAlive)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Console.WriteLine("Connection shutdown");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}