using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Avrdude_Tansa
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>

    public class Fuses
    {
        public string low { get; set; }
        public string high { get; set; }
        public string ext { get; set; }
    }

    public class Turnike
    {
        public string model { get; set; }
        public string chip { get; set; }
        public string hex { get; set; }
        public string elf { get; set; }
        public Fuses fuses { get; set; }
    }


    public partial class MainWindow : Window
    {
        private List<Turnike> turnikeList = new();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LoadTurnikeler()
        {
            try
            {
                string json = File.ReadAllText("turnikeler.json");
                var dict = JsonSerializer.Deserialize<Dictionary<string, List<Turnike>>>(json);
                turnikeList = dict["turnikeler"];

                foreach (var t in turnikeList)
                {
                    comboModel.Items.Add(t.model);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("JSON dosyası yüklenemedi: " + ex.Message);
            }
        }

        private void comboModel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selectedModel = comboModel.SelectedItem?.ToString();
            var t = turnikeList.Find(x => x.model == selectedModel);
            if (t != null)
            {
                txtHex.Text = t.hex;
                txtElf.Text = t.elf;
                txtFuses.Text = $"Low: {t.fuses.low}, High: {t.fuses.high}, Ext: {t.fuses.ext}";
            }
        }

        private void btnYukle_Click(object sender, RoutedEventArgs e)
        {
            string selectedModel = comboModel.SelectedItem?.ToString();
            var t = turnikeList.Find(x => x.model == selectedModel);
            if (t == null)
            {
                MessageBox.Show("Lütfen bir turnike modeli seçin.");
                return;
            }

            string avrdudePath = @"C:\avrdude\avrdude.exe"; // kendi sistemine göre güncelle
            if (!File.Exists(avrdudePath))
            {
                MessageBox.Show("AVRDUDE bulunamadı!");
                return;
            }

            string args = $"-p {t.chip.ToLower()} -c usbasp -U flash:w:{t.hex}:i " +
                          $"-U lfuse:w:{t.fuses.low}:m -U hfuse:w:{t.fuses.high}:m -U efuse:w:{t.fuses.ext}:m";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = avrdudePath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process proc = Process.Start(psi);
                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                txtLog.Text = output + "\n" + error;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yükleme sırasında hata oluştu: " + ex.Message);
            }
        }
    }
}
