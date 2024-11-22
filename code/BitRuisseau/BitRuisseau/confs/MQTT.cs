using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.confs
{
    internal class MQTT
    {
        public static string BrokerIp = "inf-n510-p301";
        public static int BrokerPort = 1883;

        public static string ClientId = Guid.NewGuid().ToString();
        public static string Topic { get; set; }
        public static string Username = "ict";
        public static string Password = "123";
    }
}
