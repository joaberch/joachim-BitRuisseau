using BitRuisseau.confs;
using BitRuisseau.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace BitRuisseau.services
{
    public class MQTT
    {
        static IMqttClient mqttClient; // Client MQTT global
        static MqttClientOptions mqttOptions; // Global connection options
        MyCatalog catalog = new MyCatalog();

        /// <summary>
        /// Connect to the broker specified in the confs
        /// </summary>
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

        static public async void SendData(string data)
        {
            // Create the message
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(confs.MQTT.Topic)
            .WithPayload(data)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .Build();

            // Send the message
            mqttClient.PublishAsync(message);
            Console.WriteLine("Message sent successfully!");
        }

        private void ReceiveMessage(MqttApplicationMessageReceivedEventArgs message)
        {
            try
            {
                Debug.Write(Encoding.UTF8.GetString(message.ApplicationMessage.Payload));
                GenericEnvelope envelope = JsonSerializer.Deserialize<GenericEnvelope>(Encoding.UTF8.GetString(message.ApplicationMessage.Payload));
                if (envelope.SenderId == confs.MQTT.ClientId) return;
                switch (envelope.MessageType)
                {
                    case MessageType.SEND_CATALOG:
                        {
                            EnvelopeSendCatalog enveloppeEnvoieCatalogue = JsonSerializer.Deserialize<EnvelopeSendCatalog>(envelope.EnvelopeJson);
                            break;
                        }
                    case MessageType.ASK_CATALOG:
                        {
                            EnvelopeSendCatalog sendCatalog = new EnvelopeSendCatalog();
                            //sendCatalog.Content = catalog. _maListMediaData;
                            //SendMessage(mqttClient, MessageType.ENVOIE_CATALOGUE, confs.MQTT.ClientId, sendCatalog, "test");
                            break;
                        }
                    case MessageType.SEND_FILE:
                        {
                            EnvelopeSendFile enveloppeEnvoieFichier = JsonSerializer.Deserialize<EnvelopeSendFile>(envelope.EnvelopeJson);
                            break;
                        }
                    case MessageType.ASK_FILE:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public static async void GetMessage()
        {
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                string jsonReceivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload); //Get message
                MessageBox.Show(jsonReceivedMessage);
                Debug.WriteLine($"Message received : {jsonReceivedMessage}");

                //TODO
                //in CreateConnection noLocal is disabled (version error) so check sender id, if contains "Joachim" we consider it's ourself so we don't respond to ourself
                if (jsonReceivedMessage.Contains("Joachim")) { return; }

                GenericEnvelope? deserializedMessage = DeserializeGenericMessage(jsonReceivedMessage);
                Debug.WriteLine($"Message successfully deserialized : messageType={deserializedMessage.MessageType} - SenderId={deserializedMessage.SenderId} - EnvelopeJson={deserializedMessage.EnvelopeJson}");

                switch (deserializedMessage.MessageType)
                {
                    case MessageType.ASK_CATALOG:
                        {
                            MessageBox.Show("ASK_CATALOG");
                            break;
                        }
                    case MessageType.SEND_CATALOG:
                        {
                            MessageBox.Show("SEND_CATALOG");
                            break;
                        }
                    case MessageType.SEND_FILE:
                        {
                            MessageBox.Show("SEND_FILE");
                            break;
                        }
                    case MessageType.ASK_FILE:
                        {
                            MessageBox.Show("ASK_FILE");
                            break;
                        }
                }
            };
        }

        private static GenericEnvelope? DeserializeGenericMessage(string serializedMessage)
        {
            return JsonSerializer.Deserialize<GenericEnvelope>(serializedMessage);
        }

        public static async void GetAndRespondToCatalogAsking()
        {
            // Callback function when a message is received
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                string jsonReceivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                MessageBox.Show($"Received message: {jsonReceivedMessage}");

                // HELLO is the message to ask the catalog - also check if the sender is not myself (noLocal deactivated)
                if (/*receivedMessage.Contains("HELLO") == true && */!jsonReceivedMessage.Contains("Joachim")) //TODO find where the messageType is check if is ASK_CATALOG
                {
                    // Get the list of the music
                    string path = @"../../../../musicList.csv";
                    string musicList = GetMusicList(path);

                    // Deserialize envelope
                    var messageEnvelope = JsonSerializer.Deserialize<GenericEnvelope>(jsonReceivedMessage);
                    var envelopeAskCatalog = JsonSerializer.Deserialize<EnvelopeAskCatalog>(messageEnvelope.EnvelopeJson);

                    //Creating the envelope
                    Debug.WriteLine("test" + jsonReceivedMessage);

                    string response = $"{confs.MQTT.ClientId} (Joachim) possède les musiques suivantes :\n{musicList}"; // TODO send serialized catalog

                    if (mqttClient == null || !mqttClient.IsConnected)
                    {
                        MessageBox.Show("Client not connected. Reconnecting...");
                        await mqttClient.ConnectAsync(mqttOptions);
                    }

                    // Créez le message à envoyer
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(confs.MQTT.Topic)
                        .WithPayload(response)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag(false)
                        .Build();

                    // Envoyez le message
                    mqttClient.PublishAsync(message);
                    Console.WriteLine("Message sent successfully!");
                }

                return;
            };
        }

        private static string GetMusicList(string path)
        {
            string musicData = File.ReadAllText(path);
            List<string> musicTitle = new List<string>();
            string[] data = musicData.Split(';');
            StringBuilder result = new StringBuilder();

            for (int i = 0; i<= data.Count(); ++i)
            {
                if (i == 0 || i % 5 == 0)
                {
                    musicTitle.Add(data[i]);
                }
            }

            musicTitle.ForEach(m => result.Append(m));

            return result.ToString();
        }
    }
}
