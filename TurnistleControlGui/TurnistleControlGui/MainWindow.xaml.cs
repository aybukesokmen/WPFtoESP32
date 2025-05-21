using System;
using System.IO.Ports;
using System.Windows;

namespace TurnistleControlGui
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;

        public MainWindow()
        {
            InitializeComponent();
            comPortComboBox.ItemsSource = SerialPort.GetPortNames();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string portName = comPortComboBox.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(portName))
                {
                    MessageBox.Show("Lütfen bir COM port seçin.");
                    return;
                }

                serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                serialPort.Open();
                Log("Bağlantı kuruldu: " + portName);
            }
            catch (Exception ex)
            {
                Log("Bağlantı hatası: " + ex.Message);
            }
        }

        private void SendCommand(string commandKey)
        {
            try
            {
                byte[] fullCommand = ModbusCommandLibrary.GetCommandWithCRC(commandKey);

                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Write(fullCommand, 0, fullCommand.Length);
                    Log("Gönderildi [" + commandKey + "]: " + BitConverter.ToString(fullCommand).Replace("-", " "));
                }
                else
                {
                    Log("Port açık değil.");
                }
            }
            catch (Exception ex)
            {
                Log("Hata: " + ex.Message);
            }
        }

        private void Log(string message)
        {
            txtStatus.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            txtStatus.ScrollToEnd();
        }

        // Buton olayları
        private void btnMotorEnable_Click(object sender, RoutedEventArgs e) => SendCommand("MotorEnable");
        private void btnMotorDisable_Click(object sender, RoutedEventArgs e) => SendCommand("MotorDisable");
        private void btnForwardOpen_Click(object sender, RoutedEventArgs e) => SendCommand("ForwardOpen");
        private void btnReverseOpen_Click(object sender, RoutedEventArgs e) => SendCommand("ReverseOpen");
        private void btnStop_Click(object sender, RoutedEventArgs e) => SendCommand("Stop");
        private void btnCloseDoor_Click(object sender, RoutedEventArgs e) => SendCommand("CloseDoor");
        private void btnStart_Click(object sender, RoutedEventArgs e) => SendCommand("Start");
        private void btnReset_Click(object sender, RoutedEventArgs e) => SendCommand("Reset");
    }
}
