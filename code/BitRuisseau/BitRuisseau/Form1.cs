using Microsoft.VisualBasic;
using BitRuisseau.services;
using System.IO;
using System.Windows.Forms;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        MyCatalog MyMusic  = new MyCatalog();

        public Form1()
        {
            InitializeComponent();
            InitializeCatalogMenu();
            confs.MQTT.BrokerIP = "mqtt.blue.section-inf.ch"; //base value
            services.MQTT.CreateConnection();
            //services.MQTT.Subscribe();
        }

        /// <summary>
        /// Element of the second menu
        /// </summary>
        public void InitializeSearchMenu()
        {
            this.Text = "BitRuisseau";

            Button myCatalog = new Button()
            {
                Text = "Ma médiathèque",
                BackColor = Color.FromArgb(217, 217, 217),
                Bounds = new Rectangle(),
                AutoSize = true,
                Location = new Point(100, 30)
            };
            Button SearchMusic = new Button()
            {
                Text = "Chercher une musique",
                BackColor = Color.FromArgb(111, 209, 236),
                Bounds = new Rectangle(),
                AutoSize = true,
                Location = new Point(400, 30)
            };
            Button search = new Button()
            {
                Text = "Rechercher",
                AutoSize = true,
                Location = new Point(20, 100)
            };

            myCatalog.Click += new EventHandler(MyCatalogMenu);
            SearchMusic.Click += new EventHandler(SearchMenu);
            search.Click += new EventHandler(SearchCatalog);

            this.Controls.Add(myCatalog);
            this.Controls.Add(SearchMusic);
            this.Controls.Add(search);
        }

        /// <summary>
        /// Element of the first menu
        /// </summary>
        public void InitializeCatalogMenu()
        {
            this.Text = "BitRuisseau";
            ListBox listBox = new ListBox()
            {
                Size = new Size(550, 300),
                Location = new Point(120, 90)
            };
            Button addMusicButton = new Button()
            {
                Text = "Ajouter une musique",
                AutoSize = true,
                Location = new Point(125, 400)
            };
            Button networkSelectButton = new Button()
            {
                Text = "Sélectionner un réseau",
                AutoSize = true,
                Location = new Point(350, 400)
            };
            Button myCatalog = new Button()
            {
                Text = "Ma médiathèque",
                BackColor = Color.FromArgb(111, 209, 236),
                Bounds = new Rectangle(),
                AutoSize = true,
                Location = new Point(100, 30)
            };
            Button SearchMusic = new Button()
            {
                Text = "Chercher une musique",
                BackColor = Color.FromArgb(217, 217, 217),
                Bounds = new Rectangle(),
                AutoSize = true,
                Location = new Point(400, 30)
            };

            AddMusicInListBox(listBox);

            networkSelectButton.Click += new EventHandler(NetworkSelectButtonClicked);
            addMusicButton.Click += new EventHandler(AddMusicButtonClicked);
            myCatalog.Click += new EventHandler(MyCatalogMenu);
            SearchMusic.Click += new EventHandler(SearchMenu);

            this.Controls.Add(myCatalog);
            this.Controls.Add(SearchMusic);
            this.Controls.Add(listBox);
            this.Controls.Add(addMusicButton);
            this.Controls.Add(networkSelectButton);
        }

        /// <summary>
        /// Add each music in a listbox
        /// </summary>
        /// <param name="listbox"></param>
        private void AddMusicInListBox(ListBox listbox)
        {
            string path = @"../../../../musicList.csv";
            if (File.Exists(path)) {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        listbox.Items.Add(line);
                    }
                }
            } else
            {
                File.Create(path);
            }
        }

        /// <summary>
        /// Open the form to add the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddMusicButtonClicked(object sender, EventArgs e)
        {
            AddMusic form3 = new AddMusic();
            form3.ShowDialog();
        }

        /// <summary>
        /// Open the form to select the network
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NetworkSelectButtonClicked(object sender, EventArgs e)
        {
            NetworkSelect form2 = new NetworkSelect();
            form2.ShowDialog();
        }

        /// <summary>
        /// Display the second menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchMenu(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeSearchMenu();
        }

        /// <summary>
        /// Display the first menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MyCatalogMenu(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeCatalogMenu();
        }

        private async void SearchCatalog(object sender, EventArgs e)
        {
            services.MQTT.GetAndRespondToCatalogAsking();
            services.MQTT.SendData("HELLO");
        }
    }
}
