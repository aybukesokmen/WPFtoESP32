using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;

namespace TurnistleControlGui
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;

        public MainWindow()
        {
            InitializeComponent();
            ListAvailablePorts();
            RegisterButtonEvents();
            BtnConnect.Click += BtnConnect_Click;
        }

        private void ListAvailablePorts()
        {
            ComboComPorts.ItemsSource = SerialPort.GetPortNames().OrderBy(p => p).ToArray();
            if (ComboComPorts.Items.Count > 0)
                ComboComPorts.SelectedIndex = 0;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                BtnConnect.Content = "Connect";
                MessageBox.Show("Bağlantı kesildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (ComboComPorts.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir COM port seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                serialPort = new SerialPort(ComboComPorts.SelectedItem.ToString(), 9600);
                serialPort.Open();
                BtnConnect.Content = "Disconnect";
                MessageBox.Show("Bağlantı başarılı.", "Bağlandı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButtonEvents()
        {
            BtnOnOff.Click += (s, e) => SendCommand("PWR_TOGGLE");
            BtnEmergency.Click += (s, e) => SendCommand("EMERGENCY");

            BtnL1Entry.Click += (s, e) => SendCommand("L1_ENTRY");
            BtnL1Exit.Click += (s, e) => SendCommand("L1_EXIT");
            BtnL1Free.Click += (s, e) => SendCommand("L1_FREE");

            BtnL2Entry.Click += (s, e) => SendCommand("L2_ENTRY");
            BtnL2Exit.Click += (s, e) => SendCommand("L2_EXIT");
            BtnL2Free.Click += (s, e) => SendCommand("L2_FREE");
        }

        private void SendCommand(string command)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.WriteLine(command);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Komut gönderilemedi: " + ex.Message, "İletişim Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Seri port açık değil!", "Bağlantı Sorunu", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();

            base.OnClosed(e);
        }
    }
}
