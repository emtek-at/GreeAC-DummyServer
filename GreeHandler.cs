using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DummyServer
{
    public class GreeHandlerResponse
    {
        public string text = "";
        public bool keepAlive = false;
    }
    public class GreeHandler
    {
        public static GreeHandlerResponse process(string input)
        {
            input = input.Split("\n")[0];
            JObject resObj = new JObject();
            JObject resPackObj = new JObject();
            GreeHandlerResponse response = new GreeHandlerResponse();
            
            JObject req = JObject.Parse(input);
            string cmd = (string)req["t"];
            
            Console.WriteLine("################################################## "+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(input);

            switch (cmd)
            {
                case "dis": // Discover
                    Console.WriteLine("Request: Discover");
                    string mac = (string)req["mac"];
                    
                    resPackObj["datHost"] = Program.m_DomainName;
                    resPackObj["datHostPort"] = 5000;
                    resPackObj["host"] = Program.m_ExternalIp;
                    resPackObj["ip"] = Program.m_ExternalIp;
                    resPackObj["ip2"] = Program.m_ExternalIp;
                    resPackObj["protocol"] = "TCP";
                    resPackObj["t"] = "svr";
                    resPackObj["tcpPort"] = 5000;
                    resPackObj["udpPort"] = 5000;
                    
                    resObj["cid"] = "";
                    resObj["i"] = 1;
                    resObj["pack"] = Crypter.Encrypt(resPackObj.ToString(), "");
                    resObj["t"] = "pack";
                    resObj["tcid"] = mac;
                    resObj["uid"] = 0;

                    //Console.WriteLine("response pack: "+resPackObj.ToString());
                    response.text = resObj.ToString();
                    response.keepAlive = false;
                    break;
                case "pack": // look in Pack Object
                    JObject pack = JObject.Parse(Crypter.Decrypt((string)req["pack"], ""));
                    switch ((string)pack["t"])
                    {
                        case "devLogin":
                            Console.WriteLine("Request: devLogin ");
                            string obscuredMac = (string) pack["mac"];
                            char[] obscuredArr = obscuredMac.ToCharArray();
                            char[] normalizedArr = new char[12];

                            normalizedArr[0] = obscuredArr[8];
                            normalizedArr[1] = obscuredArr[9];
                            normalizedArr[2] = obscuredArr[14];
                            normalizedArr[3] = obscuredArr[15];
                            normalizedArr[4] = obscuredArr[2];
                            normalizedArr[5] = obscuredArr[3];
                            normalizedArr[6] = obscuredArr[10];
                            normalizedArr[7] = obscuredArr[11];
                            normalizedArr[8] = obscuredArr[4];
                            normalizedArr[9] = obscuredArr[5];
                            normalizedArr[10] = obscuredArr[0];
                            normalizedArr[11] = obscuredArr[1];

                            string normalizedMac = new string(normalizedArr);
                            
                            resPackObj["t"] = "loginRes";
                            resPackObj["r"] = 200;
                            resPackObj["cid"] = normalizedMac;
                            resPackObj["uid"] = 0;
                                
                            resObj["t"] = "pack";
                            resObj["i"] = 1;
                            resObj["tcid"] = "";
                            resObj["cid"] = "";
                            resObj["uid"] = 0;
                            resObj["pack"] = Crypter.Encrypt(resPackObj.ToString(), "");
                            
                            //Console.WriteLine("response pack: "+resPackObj.ToString());
                            response.text = resObj.ToString();
                            response.keepAlive = true;
                            break;
                        default:
                            Console.WriteLine("Request Pack unknown: "+(string)pack["t"]);
                            response.text = "";
                            response.keepAlive = false;
                            break;
                    }
                    
                    break;
                case "tm": // get Time
                    Console.WriteLine("Request: Time");
                    resObj["t"] = "tm";
                    resObj["time"] = DateTime.Now.ToString("yyyy-MM-ddHH:mm:ss");

                    response.text = resObj.ToString();
                    response.keepAlive = true;
                    break;
                case "hb": // Heartbeat
                    Console.WriteLine("Request: Heartbeat");
                    resObj["t"] = "hbok";

                    response.text = resObj.ToString();
                    response.keepAlive = true;
                    break;
                default:
                    Console.WriteLine("Request unknown: "+cmd);
                    response.text = "";
                    response.keepAlive = false;
                    break;
            }

            response.text = response.text.Trim().Replace("\r", "").Replace("\n", "").Replace(" ", "");
            Console.WriteLine("Response: "+response.text);
            
            return response;
        }
    }
}