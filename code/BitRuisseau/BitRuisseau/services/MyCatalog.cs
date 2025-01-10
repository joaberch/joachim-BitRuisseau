using BitRuisseau.confs;
using BitRuisseau.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitRuisseau.services
{
    public class MyCatalog
    {
        public MyCatalog() 
        {
            List<MediaData> catalog = new List<MediaData>();
        }

        List<MediaData> musicFiles = new List<MediaData>();
        string path = @"../../../../musicList.csv";

        public void AddMusic(MediaData music)
        {
            musicFiles.Add(music);
            SaveMusicDataInTxt(music);
        }

        private void DisplayMusicAdded(MediaData musicFile)
        {
            MessageBox.Show($"Musique ajoutée :\n" +
                $"Nom: {musicFile.FileName}\n" +
                $"Artiste: {musicFile.FileArtist}\n" +
                $"Emplacement du fichier: {musicFile.FilePath}\n" +
                $"Taille du fichier: {musicFile.FileSize}\n" +
                $"Extension du fichier: {musicFile.FileType}",
                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveMusicDataInTxt(MediaData music)
        {
            if (!Path.Exists(path))
            {
                System.IO.File.Create(path);
            }
            try
            {
                string fileData = File.ReadAllText(path);
                if (!fileData.Contains(music.FilePath))
                {
                    System.IO.File.AppendAllText(path, $"{music.FileName}; {music.FileArtist}; {music.FileSize}; {music.FilePath}; {music.FileType};" + Environment.NewLine);
                    DisplayMusicAdded(music);
                } else
                {
                    MessageBox.Show("Musique déjà sélectionné.");
                }
            }
            catch {
                MessageBox.Show("Erreur, veuillez réessayer.");
            }
        }

        public static List<MediaData> GetMedia()
        {
            List<MediaData> medias = new List<MediaData>();
            string path = @"../../../../musicList.csv";
            using (StreamReader sr = new StreamReader(path))
            {
                string lines;
                while ((lines = sr.ReadLine()) != null)
                {
                    string[] line_data = lines.Replace("\\\\", "\\").Split(';');
                    Debug.WriteLine(line_data[3]);
                    if (File.Exists(line_data[3])) { Debug.WriteLine("exists"); }
                    //var file = TagLib.File.Create(line_data[3]);
                    MediaData data = new MediaData()
                    {
                        FileName = line_data[0],
                        FileArtist = line_data[1],
                        FileType = line_data[4],
                        FileSize = (long)Convert.ToDouble(line_data[2]),
                        FileDuration = "0",
                    };
                    medias.Add(data);
                }
            }

            return medias;
        }
    }
}
