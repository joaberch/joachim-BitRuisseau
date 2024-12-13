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
            this.Size = new Size(500, 400);
            this.AutoSize = true;
            this.BackColor = Color.LightGray;

            Label lblMusicName = new Label()
            {
                Text = "Nom de la musique:",
                Location = new Point(20, 20),
                AutoSize = true,
            };

            txtMusicName = new TextBox()
            {
                Location = new Point(200, 20),
                Width = 300
            };

            Label lblArtistName = new Label()
            {
                Text = "Nom de l'artiste:",
                Location = new Point(20, 60),
                AutoSize = true,
            };

            txtArtistName = new TextBox()
            {
                Location = new Point(200, 60),
                Width = 300,
            };

            Label lblFile = new Label()
            {
                Text = "Fichier de musique:",
                Location = new Point(20, 100),
                AutoSize = true,
            };

            lblFilePath = new Label()
            {
                Location = new Point(200, 100),
                Width = 300,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
            };

            Button btnFile = new Button()
            {
                Text = "Choisir un fichier",
                Location = new Point(150, 140),
                AutoSize = true,
            };
            btnFile.Click += BtnBrowse_Click;

            Button btnAdd = new Button()
            {
                Text = "Ajouter",
                Location = new Point(200, 200),
                AutoSize = true,
            };
            btnAdd.Click += BtnAdd_Click;

            this.Controls.Add(lblMusicName);
            this.Controls.Add(txtMusicName);
            this.Controls.Add(lblArtistName);
            this.Controls.Add(txtArtistName);
            this.Controls.Add(lblFile);
            this.Controls.Add(lblFilePath);
            this.Controls.Add(btnFile);
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
            
            //TODO - emit event that reload the listbox
            this.Close();
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
