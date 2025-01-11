using System.Text.Json;

namespace BitRuisseau.Models
{
    public class GenericEnvelope
    {
        public MessageType MessageType { get; set; } //Type of the envelope in EnvelopeJson
        public string SenderId { get; set; }
        public string EnvelopeJson { get; set; } //Another envelope serialized in it, his type is defined in MessageType
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public GenericEnvelope Deserialize(string json)
        {
            return JsonSerializer.Deserialize<GenericEnvelope>(json);
        }
    }
}
