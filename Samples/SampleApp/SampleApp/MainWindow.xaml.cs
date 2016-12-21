using System.Diagnostics;
using System.Windows;

namespace SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void helpButtonClick(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "https://github.com/stanac/EasyCrypto";
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
        }
    }
}
