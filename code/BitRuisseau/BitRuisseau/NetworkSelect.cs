using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitRuisseau
{
    public partial class NetworkSelect : Form
    {
        public NetworkSelect()
        {
            InitializeComponent();
            InitializeMenu();
        }
        public void InitializeMenu()
        {
            this.Text = "Sélectionner un réseau";
            this.ClientSize = new System.Drawing.Size(400, 200);

            GroupBox groupBox = new GroupBox
            {
                Text = "Options",
                Location = new Point(100, 20),
                Size = new Size(200, 150)
            };

            string[] options = { "inf-n510-p301", "mqtt.blue.section-inf.ch", "127.0.0.1" };
            int yOffset = 20;

            foreach (string option in options)
            {
                RadioButton radioButton = new RadioButton
                {
                    Text = option,
                    Location = new Point(10, yOffset),
                    AutoSize = true
                };
                radioButton.CheckedChanged += new EventHandler((sender, e) => NetworkChanged(sender, e, radioButton.Text));
                groupBox.Controls.Add(radioButton);
                yOffset += 20;
            }
            

            this.Controls.Add(groupBox);
        }
        public void NetworkChanged(object sender, EventArgs e, string text)
        {
            if (!sender.ToString().Contains("Checked: True")) { return;}
            confs.MQTT.BrokerIP = text;
            services.MQTT.Disconnect();
            services.MQTT.Connect();
        }
    }
}
