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

namespace ComputerResourceBroadcaster
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<resouce_sender> res;
        public MainWindow()
        {
            InitializeComponent();
            res = new List<resouce_sender>();
        }

        private void Send_button_Click(object sender, RoutedEventArgs e)
        {
            res.Add(new resouce_sender());
        }

        private void receive(object sender, EventArgs e)
        {
            if(e is UDP_ReceiveData)
            {
                var d = e as UDP_ReceiveData;
                Console.WriteLine(d.data);
            }
        }
    }
}
