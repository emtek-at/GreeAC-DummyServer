using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyServer
{
    public class GreeSocketHandler
    {
        private TcpListener _server;
        private bool _isRunning;
 
        public GreeSocketHandler()
        {
            _server = new TcpListener(IPAddress.Any, 5000);
            _server.Start();
 
            _isRunning = true;
            
            Console.WriteLine("GreeAC DummyServer Started");
            Console.WriteLine("Domainname for AC Devices: " + Program.m_DomainName);
            Console.WriteLine("IP Address for AC Devices: " + Program.m_ExternalIp);
        }
 
        public void AcceptConnections()
        {
            while (_isRunning)
            {
                try
                {
                    // wait for client connection
                    TcpClient newClient = _server.AcceptTcpClient();

                    // client found.
                    // create a thread to handle communication
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(newClient);
                }
                catch (Exception e)
                {
                    Console.WriteLine("AcceptConnections Error: "+e.Message);
                }
            }
        }
 
        private void HandleClient(object obj)
        {
            TcpClient client = null;
            StreamWriter sWriter = null;
            StreamReader sReader = null;

            try
            {
                client = (TcpClient) obj;
                sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
                sReader = new StreamReader(client.GetStream(), Encoding.ASCII);

                client.ReceiveTimeout = 5 * 60 * 1000; // 5 minutes
                bool bClientConnected = true;
                string sData = "";

                while (bClientConnected)
                {
                    // reads from stream
                    sData = sReader.ReadLine();

                    GreeHandlerResponse response = GreeHandler.process(sData);
                    bClientConnected = response.keepAlive;

                    if (response.text != "")
                    {
                        sWriter.WriteLine(response.text);
                        sWriter.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Handle Error: "+e.Message);
            }

            try { sWriter.Close(); } catch {}
            try { sReader.Close(); } catch {}
            try { client.Close(); } catch {}

            Console.WriteLine("Connection shutdown");
        }
    }
}