using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Diagnostics;

namespace BitRuisseau.services
{
    public class MQTT
    {
        public static MqttFactory mqttFactory = new MqttFactory();

        public static MqttClientOptions _client = new MqttClientOptionsBuilder()
            .WithTcpServer(confs.MQTT.BrokerIP, confs.MQTT.BrokerPort)
            .WithCredentials(confs.MQTT.Username, confs.MQTT.Password)
            .WithClientId(confs.MQTT.ClientId)
            .Build();
        public static async Task Subscribe()
        {
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()/*.WithTopicTemplate(sampleTemplate.WithParameter("id", "2"))*/.Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

                Debug.WriteLine("MQTT client subscribed to topic.");

                Debug.WriteLine("Press enter to exit.");
            }
        }
        public static async Task Connect()
        {
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var response = await mqttClient.ConnectAsync(_client, CancellationToken.None);

                Debug.WriteLine("The MQTT client is connected.");

                Debug.WriteLine(response);
            }
        }

        public static async Task Disconnect()
        {
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();
                await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
            }
        }
    }
}
