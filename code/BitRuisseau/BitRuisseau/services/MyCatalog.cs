using BitRuisseau.confs;
using BitRuisseau.Models;
using System;
using System.Collections.Generic;
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

        List<MusicFile> musicFiles = new List<MusicFile>();
        string path = @"../../../../musicList.csv";

        public void AddMusic(MusicFile music)
        {
            musicFiles.Add(music);
            SaveMusicDataInTxt(music);
        }

        private void DisplayMusicAdded(MusicFile musicFile)
        {
            MessageBox.Show($"Musique ajoutée :\n" +
                $"Nom: {musicFile.Name}\n" +
                $"Artiste: {musicFile.ArtistName}\n" +
                $"Emplacement du fichier: {musicFile.Path}\n" +
                $"Taille du fichier: {musicFile.Size}\n" +
                $"Extension du fichier: {musicFile.Extension}",
                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveMusicDataInTxt(MusicFile music)
        {
            if (!Path.Exists(path))
            {
                System.IO.File.Create(path);
            }
            try
            {
                string fileData = File.ReadAllText(path);
                if (!fileData.Contains(music.Path))
                {
                    System.IO.File.AppendAllText(path, $"{music.Name}; {music.ArtistName}; {music.Size}; {music.Path}; {music.Extension};" + Environment.NewLine);
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
    }
}
