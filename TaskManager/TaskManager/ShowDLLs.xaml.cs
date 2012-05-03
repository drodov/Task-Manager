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
using System.ComponentModel;
using System.Threading;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for ShowDLLs.xaml
    /// </summary>
    public partial class ShowDLLs : Window
    {
        int _procToViewDllId;
        Boolean _flagCloseWindow = false;
        Process _procToView;
        BackgroundWorker _bw = new BackgroundWorker();
        public ShowDLLs(int procId)
        {
            InitializeComponent();
            _procToViewDllId = procId;
            _procToView = Process.GetProcessById(procId);
            DllsListView.ItemsSource = _procToView.Modules;

            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            _bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                Thread.Sleep(1000);
                //
                try
                {
                    _procToView = Process.GetProcessById(_procToViewDllId);
                }
                catch (Exception)
                {
                    _flagCloseWindow = true;
                }
                finally
                {
                    worker.ReportProgress(0);
                }
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_flagCloseWindow == true)
            {
                _bw.CancelAsync();
                MessageBox.Show("Process which Dlls you're viewing has been closed!");
                this.Close();
            }
            DllsListView.ItemsSource = _procToView.Modules;
/*          if (_curThread != null)
            {
                foreach (ProcessThread thr in ThreadsListView.Items)
                {
                    if (thr.Id == _curThread.Id)
                    {
                        ThreadsListView.SelectedValue = thr;
                        break;
                    }
                }
            }
            else
            {
                ThrStartTimeLabel.Content = "";
                ThrTotalTimeLabel.Content = "";
                ThrUserTimeLabel.Content = "";
                WaitReasonLabel.Content = "";
            }
*/
        }

        private void DllsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProcessModule procMod = (DllsListView.SelectedItem as ProcessModule);
            if (procMod != null)
            {
                MessageBox.Show(procMod.FileVersionInfo.ToString());
            }
        }
    }
}
