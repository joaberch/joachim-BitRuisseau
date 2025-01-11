using System.Text.Json;

namespace BitRuisseau.Models
{
	public class EnvelopeAskCatalog
	{ //MessageType 2 : ASK_CATALOG
		public string ToJson()
		{
			return JsonSerializer.Serialize(this);
		}
	}
}
