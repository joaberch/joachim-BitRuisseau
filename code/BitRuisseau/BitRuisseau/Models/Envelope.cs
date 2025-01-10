using BitRuisseau.confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitRuisseau.Models
{
    public class GenericEnvelope
    {
        string _senderId;
        MessageType _messageType;

        string _envelopeJson; //serialised class

        public MessageType MessageType { get => _messageType; set => _messageType = value; }
        public string SenderId { get => _senderId; set => _senderId = value; }
        public string EnvelopeJson { get => _envelopeJson; set => _envelopeJson = value; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public class EnvelopeSendCatalog
    {
        /* 
            type 1 SEND_CATALOG
         */
        private List<MediaData> _content;

        public List<MediaData> Content { get => _content; set => _content = value; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class EnvelopeAskCatalog
    {
        /* 
            type 2 ASK_CATALOG
         */
        private string _content;

        public string Content { get => _content; set => _content = value; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class EnvelopeSendFile
    {
        /* 
            type 3 SEND_FILE
         */
        private string _content;

        public string Content { get => _content; set => _content = value; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public class AskFile
    {
        /*
            type 4 ASK_FILE
        */
        private string _file_name;

        public string FileName
        {
            get => _file_name;
            set => _file_name = value;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
