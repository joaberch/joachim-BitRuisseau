using BitRuisseau.confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
    public class EnvelopeSendCatalog
    {
        /* 
            type 1 ENVOIE_CATALOGUE
         */
        private int _type;
        private string _guid;
        private List<MediaData> _content;

        public string Guid { get => _guid; set => _guid = value; }
        public List<MediaData> Content { get => _content; set => _content = value; }
        public int Type { get => _type; set => _type = value; }
    }

    public class EnvelopeAskCatalog
    {
        /* 
            type 2 DEMANDE_CATALOGUE
         */
        private int _type;
        private string _guid;
        private string _content;

        public string Guid { get => _guid; set => _guid = value; }
        public string Content { get => _content; set => _content = value; }
        public int Type { get => _type; set => _type = value; }
    }

    public class EnvelopeSendFile
    {
        /* 
            type 3 ENVOIE_FICHIER
         */
        private int _type;
        private string _guid;
        private string _content;

        public string Guid { get => _guid; set => _guid = value; }
        public string Content { get => _content; set => _content = value; }
        public int Type { get => _type; set => _type = value; }
    }
}
