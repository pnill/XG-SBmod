using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace XG.Business.Helper
{
    public static class NetworkActions
    {
        public static int CurPort = 0;
        public static int[] Ports = { 9995, 9996, 9997, 9998, 9999 };

        public static long IP2Long(IPAddress addr)
        {
            try
            {
                String[] ipbytes;
                double num = 0;

                ipbytes = addr.ToString().Split('.');
                for (int i = ipbytes.Length - 1; i >= 0; i--)
                    num += ((int.Parse(ipbytes[i]) % 256) * Math.Pow(256, (3 - i)));

                return (long)num;
            }
            catch
            {
                return -1;
            }
        }

        private static IPAddress findMatch(IPAddress[] addresses, IPAddress gateway)
        {
            byte[] gatewayBytes = gateway.GetAddressBytes();
            foreach (IPAddress ip in addresses)
            {
                byte[] ipBytes = ip.GetAddressBytes();
                if (ipBytes[0] == gatewayBytes[0]
                    && ipBytes[1] == gatewayBytes[1]
                    && ipBytes[2] == gatewayBytes[2])
                {
                    return ip;
                }
            }
            return null;
        }

        private static string getInternetGateway()
        {

            Process tracert = new Process { Command = "tracert.exe", Arguments = "-h 1 8.8.8.8", Silent = true };
            if (!tracert.Run())
            {
                Console.WriteLine("getLANAddress() - getInternetGateway() failed");
                return "192.168.0.1"; // assign a default gateway not reliable at all.
            }
            

            // So hacky not even sure it will work on all configurations.
            string[] lines = tracert.Output.Split( new string[] { Environment.NewLine }, StringSplitOptions.None );
            string line = lines[3];
            string[] spaces = line.Split(new string[] { " " }, StringSplitOptions.None);
            string gateway = spaces[19];
         
            return gateway;
         }

        public static IPAddress getLANAddress2()
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in addresses)
            {
                if(address.ToString() != "127.0.0.1")
                {
                    return address;
                }
            }
            return null;
        }

        public static IPAddress getLANAddress()
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
                IPAddress gateway = IPAddress.Parse(getInternetGateway());

                return findMatch( addresses, gateway );
            }
            catch (FormatException e) { return null; }
        }

        public static IPAddress getWANAddress()
        {
            return IPAddress.Parse(new WebClient().DownloadString("http://bot.whatismyipaddress.com"));
        }
    }
}
