using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.confs
{
    internal class MQTT
    {
        //inf-n510-p301 - mqtt.blue.section-inf.ch
        public static string BrokerIP { get; set; }
        public static int BrokerPort = 1883;

        public static string ClientId = Guid.NewGuid().ToString();
        public static string Topic = "test";
        public static string Username = "ict";
        public static string Password = "321";
    }
}
