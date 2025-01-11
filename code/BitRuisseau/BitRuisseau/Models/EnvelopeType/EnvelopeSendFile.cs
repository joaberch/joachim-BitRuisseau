using BitRuisseau.confs;
using System.Text.Json;

namespace BitRuisseau.Models
{
	public class EnvelopeSendFile
	{ //MessageType 3 : SEND_FILE
		public string Content { get;  set; } //File in Base64 
		public MediaData FileInfo { get; set; } //metadata of the file
		public string ToJson()
		{
			return JsonSerializer.Serialize(this);
		}
		public EnvelopeSendFile Deserialize(string json) 
		{
			return JsonSerializer.Deserialize<EnvelopeSendFile>(json);
		}
	}
}
