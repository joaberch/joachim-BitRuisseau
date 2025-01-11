namespace BitRuisseau.confs
{
    internal class MQTT
    {
        public static string BrokerIP { get; set; }
        public static int BrokerPort = 1883;

        public static string ClientId = "Joachim-" + Guid.NewGuid().ToString();
        public static string Topic = "global";
        public static string Username = "ict";
        public static string Password = "321";
    }
}
