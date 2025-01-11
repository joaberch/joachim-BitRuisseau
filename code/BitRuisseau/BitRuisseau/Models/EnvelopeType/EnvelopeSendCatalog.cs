using BitRuisseau.confs;
using System.Text.Json;

namespace BitRuisseau.Models
{
	public class EnvelopeSendCatalog
	{ //MessageType 1 : SEND_CATALOG
		public List<MediaData> Content { get; set; } //List of the MediaData we have
		public string ToJson()
		{
			return JsonSerializer.Serialize(this);
		}
		public EnvelopeSendCatalog Deserialize(string json)
		{
			return JsonSerializer.Deserialize<EnvelopeSendCatalog>(json);
		}
	}
}
