using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Diagnostics;

namespace BitRuisseau.services
{
    public class MQTT
    {
        public static MqttClientOptions _client = new MqttClientOptionsBuilder()
            .WithTcpServer(confs.MQTT.BrokerIp, confs.MQTT.BrokerPort)
            .WithCredentials(confs.MQTT.Username, confs.MQTT.Password)
            .WithClientId(confs.MQTT.ClientId)
            .Build();
        public static async Task Subscribe(string topic)
        {
        }
        public static async Task Connect()
        {
            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var response = await mqttClient.ConnectAsync(_client, CancellationToken.None);

                Debug.WriteLine("The MQTT client is connected.");

                Debug.WriteLine(response);
            }
        }

        public static async Task Disconnect()
        {
            var mqttFactory = new MqttFactory();
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();
                await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
            }
        }
    }
}
