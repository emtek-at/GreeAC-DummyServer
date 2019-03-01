using System;
  

namespace DummyServer
{
    class Program
    {
        public static string m_DomainName = "test.emtek.at";
        public static string m_ExternalIp = "172.22.20.104";
        
        static void Main(string[] args)
        {
            
            AsynchronousSocketListener.StartListening();
        }
    }
}