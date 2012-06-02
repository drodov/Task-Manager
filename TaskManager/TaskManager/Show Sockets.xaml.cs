using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TcpUdpConnections;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for Show_Sockets.xaml
    /// </summary>
    public partial class Show_Sockets : Window
    {
        public Show_Sockets(List<CSocket> list)
        {
            InitializeComponent();
            SockListView.ItemsSource = list;
        }
    }
}
