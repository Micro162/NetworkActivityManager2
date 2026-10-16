
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace NetworkActivityManager2
{
    public partial class Form1 : Form
    {
        ListBox adapters = new ListBox();
        TextBox info = new TextBox();
        Button refresh = new Button();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public Form1()
        {

            Text = "Диспетчер мережевої активності";
            Width = 800;
            Height = 500;

            adapters.SetBounds(10, 10, 250, 400);
            info.SetBounds(270, 10, 500, 400);
            info.Multiline = true;
            info.ScrollBars = ScrollBars.Vertical;
            info.ReadOnly = true;

            refresh.Text = "Оновити";
            refresh.SetBounds(10, 420, 150, 30);
            refresh.Click += (s, e) => UpdateInfo();

            Controls.Add(adapters);
            Controls.Add(info);
            Controls.Add(refresh);

            LoadAdapters();

            timer.Interval = 2000;
            timer.Tick += (s, e) => UpdateInfo();
            timer.Start();
        }

        void LoadAdapters()
        {
            adapters.Items.Clear();

            foreach (NetworkInterface adapter in
                NetworkInterface.GetAllNetworkInterfaces())
            {
                adapters.Items.Add(adapter.Name);
            }

            if (adapters.Items.Count > 0)
                adapters.SelectedIndex = 0;

            adapters.SelectedIndexChanged += (s, e) => UpdateInfo();
        }

        void UpdateInfo()
        {
            if (adapters.SelectedItem == null)
                return;

            foreach (NetworkInterface adapter in
                NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.Name != adapters.SelectedItem.ToString())
                    continue;

                IPInterfaceProperties properties =
                    adapter.GetIPProperties();

                string result = "Назва: " + adapter.Name + "\r\n";
                result += "Стан: " + adapter.OperationalStatus + "\r\n";
                result += "Тип: " + adapter.NetworkInterfaceType + "\r\n\r\n";

                result += "IP-адреси:\r\n";

                foreach (UnicastIPAddressInformation ip in
                    properties.UnicastAddresses)
                {
                    result += ip.Address + "\r\n";
                }

                result += "\r\nСтатистика:\r\n";

                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceStatistics stats =
                        adapter.GetIPv4Statistics();

                    result += "Отримано байтів: " +
                        stats.BytesReceived + "\r\n";

                    result += "Відправлено байтів: " +
                        stats.BytesSent + "\r\n";

                    result += "Отримано пакетів: " +
                        stats.UnicastPacketsReceived + "\r\n";

                    result += "Відправлено пакетів: " +
                        stats.UnicastPacketsSent + "\r\n";
                }

                info.Text = result;
                break;
            }
        }
    }
}
