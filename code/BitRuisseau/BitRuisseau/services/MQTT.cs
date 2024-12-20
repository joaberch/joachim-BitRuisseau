using BitRuisseau.confs;
using BitRuisseau.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.ComponentModel;
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

        /// <summary>
        /// Send simple message in the broker
        /// </summary>
        /// <param name="data"></param>
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

        /// <summary>
        /// Receive every message sent
        /// </summary>
        public static async void GetMessage()
        {
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                string jsonReceivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload); //Get message
                MessageBox.Show(jsonReceivedMessage);
                Debug.WriteLine($"Message received : {jsonReceivedMessage}");

                //TODO
                //in CreateConnection noLocal is disabled (version error) so check sender id, if contains "Joachim" we consider it's ourself so we don't respond to ourself
                if (jsonReceivedMessage.Contains("Joachim")) { Debug.WriteLine("It's my own message, stop processing"); return; }

                //Deserialize the basic message to get his type
                GenericEnvelope? deserializedMessage = DeserializeGenericMessage(jsonReceivedMessage);
                Debug.WriteLine($"Message successfully deserialized : messageType={deserializedMessage.MessageType} - SenderId={deserializedMessage.SenderId} - EnvelopeJson={deserializedMessage.EnvelopeJson}");

                ProcessMessage(deserializedMessage);
            };
        }

        /// <summary>
        /// Process message depending of his type
        /// </summary>
        /// <param name="deserializedMessage"></param>
        private static void ProcessMessage(GenericEnvelope? deserializedMessage)
        {
            switch (deserializedMessage.MessageType)
            {
                case MessageType.ASK_CATALOG:
                    {
                        Debug.WriteLine("ASK_CATALOG");
                        SendCatalog();
                        break;
                    }
                case MessageType.SEND_CATALOG:
                    {
                        Debug.WriteLine("SEND_CATALOG");
                        GetCatalog();
                        break;
                    }
                case MessageType.SEND_FILE:
                    {
                        Debug.WriteLine("SEND_FILE");
                        DownloadFile();
                        break;
                    }
                case MessageType.ASK_FILE:
                    {
                        Debug.WriteLine("ASK_FILE");
                        SendFile();
                        break;
                    }
            }
        }

        /// <summary>
        /// Send file
        /// </summary>
        private static void SendFile()
        {
            //EnvelopeSendFile enveloppeEnvoieFichier = JsonSerializer.Deserialize<EnvelopeSendFile>(envelope.EnvelopeJson);
        }

        /// <summary>
        /// Get File
        /// </summary>
        private static void DownloadFile()
        {

        }

        /// <summary>
        /// Get Catalog
        /// </summary>
        private static void GetCatalog()
        {

        }

        /// <summary>
        /// Send catalog
        /// </summary>
        private static async void SendCatalog()
        {
            EnvelopeSendCatalog sendCatalog = new EnvelopeSendCatalog();

            string path = @"../../../../musicList.csv";
            string musicList = GetMusicList(path);

            //if musicList is empty return
            if (musicList.Length==0) { return; }

            List<MediaData> list = new List<MediaData>();
            list = GetMedia();

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

        public static List<MediaData> GetMedia()
        {
            List<MediaData> medias = new List<MediaData>();
            string path = @"../../../../musicList.csv";
            using (StreamReader sr = new StreamReader(path))
            {
                string lines;
                while ((lines = sr.ReadLine()) != null)
                {
                    string[] line_data = lines.Split(';');
                    FileInfo fileInfo = new FileInfo(line_data[0]);
                    //MediaData data = new MediaData(line_data[0], line_data[1], line_data[4], line_data[2]);
                }
            }

            return medias;
        }

        /// <summary>
        /// Ask Catalog on button "Rechercher" clicked
        /// </summary>
        public static async void AskCatalog()
        {
            EnvelopeAskCatalog askCatalog = new EnvelopeAskCatalog();

            GenericEnvelope genericEnvelope = new GenericEnvelope();
            genericEnvelope.MessageType = MessageType.ASK_CATALOG;
            genericEnvelope.SenderId = confs.MQTT.ClientId;
            genericEnvelope.EnvelopeJson = askCatalog.ToJson();

            SendData(JsonSerializer.Serialize(genericEnvelope));
        }

        /// <summary>
        /// Deserialize Generic Envelope
        /// </summary>
        /// <param name="serializedMessage"></param>
        /// <returns></returns>
        private static GenericEnvelope? DeserializeGenericMessage(string serializedMessage)
        {
            return JsonSerializer.Deserialize<GenericEnvelope>(serializedMessage);
        }

        /// <summary>
        /// Return the list of the music title
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
