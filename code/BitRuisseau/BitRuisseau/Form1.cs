using Microsoft.VisualBasic;
using BitRuisseau.services;
using System.IO;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        MyMusic MyMusic  = new MyMusic();

        public Form1()
        {
            InitializeComponent();
            InitializeMenu();
            confs.MQTT.BrokerIP = "mqtt.blue.section-inf.ch"; //base value
            services.MQTT.CreateConnection();
            //services.MQTT.Subscribe();
        }

        public void InitializeMenu()
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
            AddMusic(listBox);

            networkSelectButton.Click += new EventHandler(NetworkSelectButtonClicked);
            addMusicButton.Click += new EventHandler(AddMusicButtonClicked);

            this.Controls.Add(listBox);
            this.Controls.Add(addMusicButton);
            this.Controls.Add(networkSelectButton);
        }
        private void AddMusic(ListBox listbox)
        {
            using (StreamReader sr = new StreamReader(@"../../../../musicList.csv"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    listbox.Items.Add(line);
                }
            }
        }
        private async void AddMusicButtonClicked(object sender, EventArgs e)
        {
            AddMusic form3 = new AddMusic();
            form3.ShowDialog();
        }
        private async void NetworkSelectButtonClicked(object sender, EventArgs e)
        {
            NetworkSelect form2 = new NetworkSelect();
            form2.ShowDialog();
        }
        public void openSearch()
        {
            Form formSearch = new Form()
            {
                Size = new Size(300, 600)
            };
            ListBox list = new ListBox()
            {
                Location = new Point(40, 50),
                Size = new Size(200, 400),
            };
            getList().ForEach(text => list.Items.Add(text));

            formSearch.Controls.Add(list);
            formSearch.ShowDialog();
        }

        public static List<string> getList()
        {
            return new List<string>() { "test1", "test2" };
        }
    }
}
