using Microsoft.VisualBasic;
using BitRuisseau.services;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeMenu();
            services.MQTT.Connect();
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
                Size = new Size(150, 30),
                Location = new Point(125, 400)
            };
            Button networkSelectButton = new Button()
            {
                Text = "Sélectionner un réseau",
                Size = new Size(150, 30),
                Location = new Point(300, 400)
            };
            networkSelectButton.Click += new EventHandler(networkSelectButtonClicked);

            this.Controls.Add(listBox);
            this.Controls.Add(addMusicButton);
            this.Controls.Add(networkSelectButton);
        }
        private async void networkSelectButtonClicked(object sender, EventArgs e)
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
