using System;
  

namespace DummyServer
{
    class Program
    {
        public static string m_DomainName = Environment.GetEnvironmentVariable("DummyDomainName");//"test.emtek.at";
        public static string m_ExternalIp = Environment.GetEnvironmentVariable("ExternalIP");//"172.22.20.104";
        
        static void Main(string[] args)
        {
            
            //AsynchronousSocketListener.StartListening();
            GreeSocketHandler gsh = new GreeSocketHandler();
            gsh.AcceptConnections();
        }
    }
}