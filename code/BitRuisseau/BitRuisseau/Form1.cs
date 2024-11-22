using Microsoft.VisualBasic;
using BitRuisseau.services;

namespace BitRuisseau
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeSearch();
        }
        private void InitializeSearch()
        {
            Button button = new Button();
            button.Text = "Rechercher";
            button.Location = new Point(50, 50);
            button.Click += (sender, e) => openSearch();
            this.Controls.Add(button);
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
            MQTT.getList().ForEach(text => list.Items.Add(text));

            formSearch.Controls.Add(list);
            formSearch.ShowDialog();
        }
    }
}
