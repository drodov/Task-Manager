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
    /// Interaction logic for CreatProcWindow.xaml
    /// </summary>
    public partial class CreatProcWindow : Window
    {
        public CreatProcWindow()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(textBox1.Text);
                this.Close();
            }
            catch (Exception excpt)
            {
                textBox1.Text = excpt.Message;
            }
        }
    }
}
