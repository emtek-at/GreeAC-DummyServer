using System;
  

namespace DummyServer
{
    class Program
    {
        public static string m_DomainName = Environment.GetEnvironmentVariable("DOMAIN_NAME");
        public static string m_ExternalIp = Environment.GetEnvironmentVariable("EXTERNAL_IP");
        
        static void Main(string[] args)
        {
            GreeSocketHandler gsh = new GreeSocketHandler();
            gsh.AcceptConnections();
        }
    }
}