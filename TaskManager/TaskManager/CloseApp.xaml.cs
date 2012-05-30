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
using System.Diagnostics;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for CloseApp.xaml
    /// </summary>
    public partial class CloseApp : Window
    {
        Process _procToClose;
        public CloseApp(Process procToClose)
        {
            InitializeComponent();
            _procToClose = procToClose;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _procToClose.Kill();
            }
            catch (Exception)
            { }
            finally
            {
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
