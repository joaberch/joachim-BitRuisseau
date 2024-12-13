using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Diagnostics;
using System.Text;

namespace BitRuisseau.services
{
    public class MQTT
    {
        static IMqttClient mqttClient; // Client MQTT global
        static MqttClientOptions mqttOptions; // Global connection options

        public static async void CreateConnection()
        {
            try
            {
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
                // MQTT connection param
                mqttOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(confs.MQTT.BrokerIP, confs.MQTT.BrokerPort)
                    .WithCredentials(confs.MQTT.Username, confs.MQTT.Password)
                    .WithClientId(confs.MQTT.ClientId)
                    .WithCleanSession()
                    .Build();

                //Connect to the broker
                var connectResult = await mqttClient.ConnectAsync(mqttOptions);

                //Check broker connection
                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    MessageBox.Show("Connected to MQTT broker successfully.");

                    // Subscribe
                    var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(f =>
                        {
                            f.WithTopic(confs.MQTT.Topic);
                            f.WithNoLocal(false); // Ensure the client does not receive its own messages - disabled because requires a certain version : TODO
                        })
                            .Build();
                    // Subscribe to a topic
                    await mqttClient.SubscribeAsync(subscribeOptions);
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the broker");
            }
        }
        //public static async void GetAndRespondToCatalogAsking()
        //{
        //    // Callback function when a message is received
        //    mqttClient.ApplicationMessageReceivedAsync += async e =>
        //    {
        //        string receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        //        MessageBox.Show($"Received message: {receivedMessage}");

        //        // Vérifier que le message contient HELLO
        //        if (receivedMessage.Contains("HELLO") == true)
        //        {
        //            // Obtenir la liste des musiques à envoyer
        //            string musicList = GetMusicList();

        //            // Construisez le message à envoyer (sera changé en JSON)
        //            string response = $"{confs.MQTT.ClientId} (Joachim) possède les musiques suivantes :\n{musicList}";

        //            if (mqttClient == null || !mqttClient.IsConnected)
        //            {
        //                MessageBox.Show("Client not connected. Reconnecting...");
        //                await mqttClient.ConnectAsync(mqttOptions);
        //            }

        //            // Créez le message à envoyer
        //            var message = new MqttApplicationMessageBuilder()
        //                .WithTopic(confs.MQTT.Topic)
        //                .WithPayload(response)
        //                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        //                .WithRetainFlag(false)
        //                .Build();

        //            // Envoyez le message
        //            mqttClient.PublishAsync(message);
        //            Console.WriteLine("Message sent successfully!");
        //        }

        //        return;
        //    };
        //}
    }
}
