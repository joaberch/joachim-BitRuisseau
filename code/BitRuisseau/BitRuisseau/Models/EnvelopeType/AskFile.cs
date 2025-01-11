using System.Text.Json;

namespace BitRuisseau.Models
{
	public class AskFile
	{ //MessageType 4 : ASK_FILE
		public string FileName { get;  set; } //Name of the music asked
		public string ToJson()
		{
			return JsonSerializer.Serialize(this);
		}
		public AskFile Deserialize(string json)
		{
			return JsonSerializer.Deserialize<AskFile>(json);
		}
	}
}
