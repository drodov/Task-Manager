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
        CApp _appToStart;
        Proc _procToStart;
        public CreatProcWindow(Proc ProcToStart, CApp AppToStart)
        {
            _procToStart = ProcToStart;
            _appToStart = AppToStart;
            InitializeComponent();
            textBox1.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process proc = Process.Start(textBox1.Text);
                if (proc != null)
                {
                    _procToStart.CopyFromProcess(proc);
                    _appToStart.CopyFromProcess(proc);
                }
                this.Close();
            }
            catch (Exception excpt)
            {
                textBox1.Text = excpt.Message;
            }
        }
    }
}
