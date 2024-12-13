using BitRuisseau.Models;
using BitRuisseau.services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BitRuisseau
{
    public partial class AddMusic : Form
    {
        MyMusic MyMusic = new MyMusic();

        private TextBox txtMusicName;
        private TextBox txtArtistName;
        private Label lblFilePath;

        public AddMusic()
        {
            InitializeComponent();
            InitializeMenu();
        }

        public void InitializeMenu()
        {
            this.Text = "Ajouter une musique";
            this.Size = new Size(500, 300);
            this.BackColor = Color.LightGray;

            Label lblMusicName = new Label();
            lblMusicName.Text = "Nom de la musique:";
            lblMusicName.Location = new Point(20, 20);
            lblMusicName.AutoSize = true;

            txtMusicName = new TextBox();
            txtMusicName.Location = new Point(150, 20);
            txtMusicName.Width = 300;

            Label lblArtistName = new Label();
            lblArtistName.Text = "Nom de l'artiste:";
            lblArtistName.Location = new Point(20, 60);
            lblArtistName.AutoSize = true;

            txtArtistName = new TextBox();
            txtArtistName.Location = new Point(150, 60);
            txtArtistName.Width = 300;

            Label lblFile = new Label();
            lblFile.Text = "Fichier de musique:";
            lblFile.Location = new Point(20, 100);
            lblFile.AutoSize = true;

            lblFilePath = new Label();
            lblFilePath.Location = new Point(150, 100);
            lblFilePath.Width = 300;
            lblFilePath.BorderStyle = BorderStyle.FixedSingle;

            Button btnBrowse = new Button();
            btnBrowse.Text = "Choisir un fichier";
            btnBrowse.Location = new Point(150, 140);
            btnBrowse.Click += BtnBrowse_Click;

            Button btnAdd = new Button();
            btnAdd.Text = "Ajouter";
            btnAdd.Location = new Point(200, 200);
            btnAdd.Click += BtnAdd_Click;

            this.Controls.Add(lblMusicName);
            this.Controls.Add(txtMusicName);
            this.Controls.Add(lblArtistName);
            this.Controls.Add(txtArtistName);
            this.Controls.Add(lblFile);
            this.Controls.Add(lblFilePath);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(btnAdd);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.flac"; //only music
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblFilePath.Text = openFileDialog.FileName;
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string musicName = txtMusicName.Text;
            string artistName = txtArtistName.Text;
            string filePath = lblFilePath.Text;

            if (string.IsNullOrEmpty(musicName) || string.IsNullOrEmpty(artistName) || string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Toutes les valeurs nécessaires n'ont pas été remplies.");
                return;
            }

            long fileSize = GetFileSize(lblFilePath.Text);
            string fileExtension = GetExtension(lblFilePath.Text);

            MusicFile musicFile = new MusicFile(musicName, fileSize, fileExtension, filePath, artistName);

            MyMusic.AddMusic(musicFile);
            //DisplayMusicAdded(musicFile);

            this.Close();
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
        private string GetExtension(string path)
        {
            try
            {
                var path_split = path.Split('.');
                return path_split[path_split.Count() - 1];
            }
            catch
            {
                return null;
            }
        }

        private long GetFileSize(string path)
        {
            if (File.Exists(path))
            {
                long length = new FileInfo(path).Length;
                return length;
            }
            return 0;
        }
    }
}
