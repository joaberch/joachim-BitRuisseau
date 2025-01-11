using BitRuisseau.confs;
using BitRuisseau.Models;
using Microsoft.VisualBasic.ApplicationServices;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BitRuisseau.services
{
	public class MQTT
    {
        static IMqttClient mqttClient; // Client MQTT global
        static MqttClientOptions mqttOptions; // Global connection options
        public static MyCatalog myCatalog = new MyCatalog();

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

				//TODO
				//in CreateConnection noLocal is disabled (version error) so check sender id, if contains "Joachim" we consider it's ourself so we don't respond to ourself
				if (jsonReceivedMessage.Contains(confs.MQTT.ClientId)) { Debug.WriteLine("It's my own message, stop processing"); return; }

				Debug.WriteLine($"Message received : {jsonReceivedMessage}");

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
                        GetCatalog(deserializedMessage);
                        break;
                    }
                case MessageType.SEND_FILE:
                    {
                        Debug.WriteLine("SEND_FILE");
                        DownloadFile(deserializedMessage);
                        break;
                    }
                case MessageType.ASK_FILE:
                    {
                        Debug.WriteLine("ASK_FILE");
                        SendFile(deserializedMessage);
                        break;
                    }
            }
        }

        /// <summary>
        /// Send file
        /// </summary>
        private static void SendFile(GenericEnvelope deserializedMessage)
        {
            try
            {
                AskFile askFile = JsonSerializer.Deserialize<AskFile>(deserializedMessage.EnvelopeJson);
                MediaData music = myCatalog.GetMusic(askFile.FileName);
                if(music == null) { return; }
                if (music.FileName.Length > 0) //Check name of the music to know if empty
                {
                    EnvelopeSendFile envelopeSendFile = new EnvelopeSendFile();
                    envelopeSendFile.Content = Convert.ToBase64String(File.ReadAllBytes(music.FilePath.Trim())); //Trim because the first character is a space in the csv
                    envelopeSendFile.FileInfo = music;

					GenericEnvelope envelope = new GenericEnvelope();
					envelope.MessageType = MessageType.SEND_FILE;
					envelope.SenderId = confs.MQTT.ClientId;
					envelope.EnvelopeJson = envelopeSendFile.ToJson();

					SendData(envelope.ToJson());
				}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Get File
        /// </summary>
        private static void DownloadFile(GenericEnvelope? deserializedMessage)
        {
            EnvelopeSendFile envelopeSendFile = JsonSerializer.Deserialize<EnvelopeSendFile>(deserializedMessage.EnvelopeJson);
            if (!myCatalog.WantThisFile(envelopeSendFile.FileInfo)) { return; }

			MediaData metaData = envelopeSendFile.FileInfo;
			byte[] file = Convert.FromBase64String(envelopeSendFile.Content);
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\\{metaData.FileName.Trim()}.{metaData.FileType.Trim()}";
			File.WriteAllBytes(path, file);
            MessageBox.Show($"La musique {metaData.FileName} a été téléchargé avec succès à l'emplacement : {path}");

            myCatalog.RemoveWantedMusic(metaData);
            myCatalog.AddMusic(metaData);
            myCatalog.SaveMusicDataInTxt(metaData);
		}

        /// <summary>
        /// Get Catalog
        /// </summary>
        private static void GetCatalog(GenericEnvelope deserializedMessage)
        {
            try
            {
                string json = deserializedMessage.EnvelopeJson;
                EnvelopeSendCatalog envelopeCatalog = JsonSerializer.Deserialize<EnvelopeSendCatalog>(json);
                envelopeCatalog.Content.ForEach(myCatalog.AddPotentialMusic);
                myCatalog.GetPotentialMusic(MyCatalog.dataGridView);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static DataGridView GetPotentialCatalog(DataGridView dataGridView)
        {
            return myCatalog.GetPotentialMusic(dataGridView);
        }

        /// <summary>
        /// Send catalog
        /// </summary>
        private static async void SendCatalog()
        {
            EnvelopeSendCatalog sendCatalog = new EnvelopeSendCatalog();

            string path = @"../../../../musicList.csv";
            List<string> musicList = GetMusicList(path);

            //if musicList is empty return
            if (musicList.Count()==0) { return; }

            sendCatalog.Content = MyCatalog.GetMyMedia();
            GenericEnvelope envelope = new GenericEnvelope();
            envelope.MessageType = MessageType.SEND_CATALOG;
            envelope.SenderId = confs.MQTT.ClientId;
            envelope.EnvelopeJson = sendCatalog.ToJson();

            SendData(envelope.ToJson());
        }

		private static string ToJson(object obj)
		{
			var properties = obj.GetType().GetProperties();
			var jsonParts = properties.Select(prop =>
			{
				var value = prop.GetValue(obj);

				if (value is string) return $"\"{prop.Name}\": \"{value}\"";
				if (value is DateTime) return $"\"{prop.Name}\": \"{((DateTime)value).ToString("o")}\"";
				if (value is IEnumerable<object> enumerable)
				{
					var items = string.Join(", ", enumerable.Select(item => ToJson(item)));
					return $"\"{prop.Name}\": [{items}]";
				}
				return $"\"{prop.Name}\": {value}";
			});

			return "{" + string.Join(", ", jsonParts) + "}";
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
        private static List<string> GetMusicList(string path)
        {
			List<string> musicTitles = new List<string>();

			foreach (string line in File.ReadLines(path))
			{
				string[] data = line.Split(';'); // Split each line by semicolons

				// Process each element in the current line
				for (int i = 0; i < data.Length; i++)
				{
					if (i == 0)
					{
						musicTitles.Add(data[i]);
					}
				}
			}

            return musicTitles;

		}

        /// <summary>
        /// Ask Music File
        /// </summary>
        /// <param name="dataGridView"></param>
        public static void AskMusic(string musicName)
        {
            if (!(myCatalog.GetMusic(musicName) == null)) { MessageBox.Show("Vous avez déjà cette musique");  return; }
            AskFile askFile = new AskFile()
            {
                FileName = musicName,
            };
            myCatalog.AddWantedMusic(myCatalog.GetMusic(musicName));
            GenericEnvelope genericEnvelope = new GenericEnvelope
            {
                MessageType = MessageType.ASK_FILE,
                SenderId = confs.MQTT.ClientId,
                EnvelopeJson = askFile.ToJson(),
            };
            SendData(JsonSerializer.Serialize(genericEnvelope));
        }
    }
}
