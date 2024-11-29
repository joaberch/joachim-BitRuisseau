using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.Models
{
    public class MusicFile
    {
        public string Name { get; set; }
        public string ArtistName { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }

        public MusicFile(string name, long size, string extension, string path, string artistName) 
        { 
            Name = name;
            Size = size;
            Extension = extension;
            Path = path;
            ArtistName = artistName;
        }
        public MusicFile(long size, string extension, string path) 
        { 
            Size = size;
            Extension = extension;
            Path = path;
        }
    }
}
