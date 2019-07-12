using System;
using System.Collections.Generic;
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

namespace test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow:Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Dictionary<string, MainWindow> win = new Dictionary<string, MainWindow>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = this.textBox.Text.ToString();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Title = str;
            mainWindow.Owner = this;
            win.Add(str, mainWindow);
            this.tvwin.Items.Add(str);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            

            try
            {
                string s = this.treeView.SelectedItem.ToString();

            }
            catch(Exception ex)
            {

            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string s = this.Title.ToString();
            win.Remove(s);
            this.tvwin.Items.Remove(s);
        }
    }
}
