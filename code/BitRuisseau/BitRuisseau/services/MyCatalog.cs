﻿using BitRuisseau.confs;
using System.Diagnostics;

namespace BitRuisseau.services
{
    public class MyCatalog
    {
        public MyCatalog() 
        {
            myMusicFiles = GetMyMedia();
        }

        List<MediaData> myMusicFiles = new List<MediaData>();
        List<MediaData> potentialMusicFiles = new List<MediaData>();
        List<MediaData> wantedFiles = new List<MediaData>();
        string path = @"../../../../musicList.csv";

        public static DataGridView dataGridView = new DataGridView();

        public bool WantThisFile(MediaData mediaData)
        {
            return wantedFiles.Where(music => music.FileName == mediaData.FileName).Any();
        }


		public MediaData GetMusic(string musicName)
        {
            return myMusicFiles.Where(music => music.FileName == musicName).FirstOrDefault();
        }

        public MediaData GetPotentialMusic(string musicName)
        {
            return potentialMusicFiles.Where(music => music.FileName == musicName).FirstOrDefault();
        }

		public DataGridView UpdatePotentialMusic(DataGridView dataGridView)
        {
             potentialMusicFiles
                .Where(music => music.FileSize > 0)
                .ToList()
                .ForEach(music => dataGridView.Rows.Add(music.FileName, music.FileArtist, music.FileType, music.FileSize, music.FileDuration, "Télécharger"));

            return dataGridView;
        }

        public void ClearPotentialMusic()
        {
            potentialMusicFiles.Clear();
        }

        public void AddMusic(MediaData music)
        {
            myMusicFiles.Add(music);
        }

        public void AddPotentialMusic(MediaData music)
        {
            potentialMusicFiles.Add(music);
        }

        public void AddWantedMusic(MediaData music)
        {
            if (music == null) { return; }
            wantedFiles.Add(music);
        }
        public void RemoveWantedMusic(MediaData mediaData)
        {
            wantedFiles.Remove(wantedFiles.Where(music => music.FileName == mediaData.FileName).FirstOrDefault());
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

        public void SaveMusicDataInTxt(MediaData music)
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

        public static List<MediaData> GetMyMedia()
        {
            List<MediaData> medias = new List<MediaData>();
            string path = @"../../../../musicList.csv";

			if (!Path.Exists(path))
			{
                //To close the file to process it after
                using (FileStream fs = File.Create(path)) ;
			}

			using (StreamReader sr = new StreamReader(path))
            {
                string lines;
                while ((lines = sr.ReadLine()) != null)
                {
                    string[] line_data = lines.Replace("\\\\", "\\").Split(';');
                    Debug.WriteLine(line_data[3]);
                    if (File.Exists(line_data[3])) { Debug.WriteLine("exists"); }
                    //var file = TagLib.File.Create(line_data[3]);
                    try
                    {
                        MediaData data = new MediaData()
                        {
                            FileName = line_data[0],
                            FileArtist = line_data[1],
                            FileType = line_data[4],
                            FileSize = (long)Convert.ToDouble(line_data[2]),
                            FileDuration = "0",
                            FilePath = line_data[3],
                        };

                        medias.Add(data);
                    } catch (Exception e) 
                    {
                        Debug.WriteLine(e);
                    }
                }
            }

            return medias;
        }
    }
}
