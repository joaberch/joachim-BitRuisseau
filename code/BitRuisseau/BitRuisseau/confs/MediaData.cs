using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.confs
{
    public class MediaData
    {
        private string _file_name;
        private string _file_artist;
        private string _file_type;
        private long _file_size;
        private string _file_duration;

        public string File_name { get => _file_name; set => _file_name = value; }
        public string File_artist { get => _file_artist; set => _file_artist = value; }
        public string File_type { get => _file_type; set => _file_type = value; }
        public long File_size { get => _file_size; set => _file_size = value; }
        public string File_duration { get => _file_duration; set => _file_duration = value; }

        public MediaData(string name, string artist, string type, long size, string duration)
        {
            File_name = name;
            File_artist = artist;
            File_type = type;
            File_size = size;
            File_duration = duration;
        }
    }
}
