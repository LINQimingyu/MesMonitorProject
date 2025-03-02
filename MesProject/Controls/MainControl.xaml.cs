using System.Windows;
using System.Windows.Controls;
using MesProject.Views;

namespace MesProject.Controls
{
    /// <summary>
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}
