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
        private string _file_path;

        public string FileName { get => _file_name; set => _file_name = value; }
        public string FileArtist { get => _file_artist; set => _file_artist = value; }
        public string FileType { get => _file_type; set => _file_type = value; }
        public long FileSize { get => _file_size; set => _file_size = value; }
        public string FileDuration { get => _file_duration; set => _file_duration = value; }
        public string FilePath { get => _file_path; set => _file_path = value; }

        //public MediaData(string name, string artist, string type, long size, string duration)
        //{
        //    FileName = name;
        //    FileArtist = artist;
        //    FileType = type;
        //    FileSize = size;
        //    FileDuration = duration;
        //}
    }
}
