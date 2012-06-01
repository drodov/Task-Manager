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
    /// Interaction logic for ShowProcInfoWindow.xaml
    /// </summary>
    public partial class ShowProcInfoWindow : Window
    {
        /// <summary>
        /// Thread for updating main fields of window.
        /// </summary>
        BackgroundWorker _bwMainFields = new BackgroundWorker();

        /// <summary>
        /// Thread for updating information about selected thread.
        /// </summary>
        BackgroundWorker _bwThreadFields = new BackgroundWorker();

        /// <summary>
        /// Selected thread for viewing information.
        /// </summary>
        ProcessThread _curThread;

        /// <summary>
        /// Flag for starting updating information about selected thread.
        /// </summary>
        Boolean _flagBwThreadRun = false;

        /// <summary>
        /// Flag of changing process priority.
        /// </summary>
        Boolean _flagChangePriority = false;

        /// <summary>
        /// Flag for closing window.
        /// </summary>
        Boolean _flagCloseWindow = false;

        /// <summary>
        /// Data about selected process.
        /// </summary>
        String[] _procState = new String[15];

        /// <summary>
        /// Data about selected thread.
        /// </summary>
        String[] _threadState = new String[4];

        /// <summary>
        /// Selected process to view additional information. 
        /// </summary>
        Process ProcToView;

        /// <summary>
        /// PID of selected process.
        /// </summary>
        int ProcToViewId;

        /// <summary>
        /// Window creation for viewing selected process.
        /// </summary>
        /// <param name="SelectedProc">Selected process.</param>
        public ShowProcInfoWindow(int SelectedProcId)
        {
            InitializeComponent();
            ProcToViewId = SelectedProcId;
            ProcToView = Process.GetProcessById(ProcToViewId);
            // set priority
            foreach (ComboBoxItem val in PriorComboBox.Items)
            {
                if (val.Content.ToString() == ProcToView.PriorityClass.ToString())
                    PriorComboBox.SelectedItem = val;
            }
            _flagChangePriority = true;
            //set other params
            OwnerLabel.Content = SystemInfo.GetProcessOwnerName(ProcToView);
            StartTimeLabel.Content = ProcToView.StartTime;
            UserTimeLabel.Content = ProcToView.UserProcessorTime;
            TotalTimeLabel.Content = ProcToView.TotalProcessorTime;
            NonPagedSystMemLabel.Content = (ProcToView.NonpagedSystemMemorySize64 / 1024).ToString() + " Kb";
            PagedSystMemLabel.Content = (ProcToView.PagedSystemMemorySize64 / 1024).ToString() + " Kb";
            PagedMemLabel.Content = (ProcToView.PagedMemorySize64 / 1024).ToString() + " Kb";
            PeakPagedMemLabel.Content = (ProcToView.PeakPagedMemorySize64 / 1024).ToString() + " Kb";
            PeakVirtMemLabel.Content = (ProcToView.PeakVirtualMemorySize64 / 1024).ToString() + " Kb";
            PeakWorkingSetLabel.Content = (ProcToView.PeakWorkingSet64 / 1024).ToString() + " Kb";
            PrivateMemorySizeLabel.Content = (ProcToView.PrivateMemorySize64 / 1024).ToString() + " Kb";
            VirtualMemorySizeLabel.Content = (ProcToView.VirtualMemorySize64 / 1024).ToString() + " Kb";
            WorkingSetLabel.Content = (ProcToView.WorkingSet64 / 1024).ToString() + " Kb";
            CountThreadsLabel.Content = ProcToView.Threads.Count.ToString();
            // set list of threads
            ThreadsListView.ItemsSource = ProcToView.Threads;

            _bwMainFields.WorkerReportsProgress = true;
            _bwMainFields.WorkerSupportsCancellation = true;
            _bwMainFields.DoWork += new DoWorkEventHandler(bwMainFields_DoWork);
            _bwMainFields.ProgressChanged += new ProgressChangedEventHandler(bwMainFields_ProgressChanged);
            _bwMainFields.RunWorkerAsync();

            _bwThreadFields.WorkerReportsProgress = true;
            _bwThreadFields.WorkerSupportsCancellation = true;
            _bwThreadFields.DoWork += new DoWorkEventHandler(bwThreadFields_DoWork);
            _bwThreadFields.ProgressChanged += new ProgressChangedEventHandler(bwThreadFields_ProgressChanged);
        }

        /// <summary>
        /// Call changing priority of selected process.
        /// </summary>
        private void PriorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_flagChangePriority == true)
            {
                switch ((PriorComboBox.SelectedItem as ComboBoxItem).Content.ToString().ToLower())
                {
                    case "idle": ProcToView.PriorityClass = ProcessPriorityClass.Idle; break;
                    case "normal": ProcToView.PriorityClass = ProcessPriorityClass.Normal; break;
                    case "abovenormal": ProcToView.PriorityClass = ProcessPriorityClass.AboveNormal; break;
                    case "belownormal": ProcToView.PriorityClass = ProcessPriorityClass.BelowNormal; break;
                    case "high": ProcToView.PriorityClass = ProcessPriorityClass.High; break;
                    case "realtime": ProcToView.PriorityClass = ProcessPriorityClass.RealTime; break;
                }
            }
        }

        /// <summary>
        /// Collect information about selected process.
        /// </summary>
        private void bwMainFields_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                Thread.Sleep(1000);
                //
                try
                {
                    ProcToView = Process.GetProcessById(ProcToViewId);
                }
                catch (Exception)
                {
                    _flagCloseWindow = true;
                    worker.ReportProgress(0);
                }
                // set priority
                _procState[14] = ProcToView.PriorityClass.ToString();
                // set other params
                _procState[0] = SystemInfo.GetProcessOwnerName(ProcToView);
                _procState[1] = ProcToView.StartTime.ToString();
                _procState[2] = ProcToView.UserProcessorTime.ToString();
                _procState[3] = ProcToView.TotalProcessorTime.ToString();
                _procState[4] = (ProcToView.NonpagedSystemMemorySize64 / 1024).ToString() + " Kb";
                _procState[5] = (ProcToView.PagedSystemMemorySize64 / 1024).ToString() + " Kb";
                _procState[6] = (ProcToView.PagedMemorySize64 / 1024).ToString() + " Kb";
                _procState[7] = (ProcToView.PeakPagedMemorySize64 / 1024).ToString() + " Kb";
                _procState[8] = (ProcToView.PeakVirtualMemorySize64 / 1024).ToString() + " Kb";
                _procState[9] = (ProcToView.PeakWorkingSet64 / 1024).ToString() + " Kb";
                _procState[10] = (ProcToView.PrivateMemorySize64 / 1024).ToString() + " Kb";
                _procState[11] = (ProcToView.VirtualMemorySize64 / 1024).ToString() + " Kb";
                _procState[12] = (ProcToView.WorkingSet64 / 1024).ToString() + " Kb";
                _procState[13] = ProcToView.Threads.Count.ToString();
                //
                worker.ReportProgress(0);
            }
        }

        /// <summary>
        /// Update information about selected process.
        /// </summary>
        private void bwMainFields_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_flagCloseWindow == true)
            {
                _bwMainFields.CancelAsync();
                _bwThreadFields.CancelAsync();
                MessageBox.Show("Process you're viewing has been closed!");
                this.Close();
            }
            OwnerLabel.Content = _procState[0];
            StartTimeLabel.Content = _procState[1];
            UserTimeLabel.Content = _procState[2];
            TotalTimeLabel.Content = _procState[3];
            NonPagedSystMemLabel.Content = _procState[4];
            PagedSystMemLabel.Content = _procState[5];
            PagedMemLabel.Content = _procState[6];
            PeakPagedMemLabel.Content = _procState[7];
            PeakVirtMemLabel.Content = _procState[8];
            PeakWorkingSetLabel.Content = _procState[9];
            PrivateMemorySizeLabel.Content = _procState[10];
            VirtualMemorySizeLabel.Content = _procState[11];
            WorkingSetLabel.Content = _procState[12];
            CountThreadsLabel.Content = _procState[13];
            _flagChangePriority = false;
            // set priority
            foreach (ComboBoxItem val in PriorComboBox.Items)
            {
                if (val.Content.ToString() == _procState[14])
                    PriorComboBox.SelectedItem = val;
            }
            _flagChangePriority = true;
            //
            ThreadsListView.ItemsSource = ProcToView.Threads;
            if (_curThread != null)
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
        }

        /// <summary>
        /// Collect information about selected thread.
        /// </summary>
        private void bwThreadFields_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                try
                {
                    _threadState[0] = _curThread.StartTime.ToString();
                    _threadState[1] = _curThread.TotalProcessorTime.ToString();
                    _threadState[2] = _curThread.UserProcessorTime.ToString();
                    if (_curThread.ThreadState == System.Diagnostics.ThreadState.Wait)
                        _threadState[3] = _curThread.WaitReason.ToString();
                    else
                        _threadState[3] = "";
                }
                catch (Exception)
                {
                    _threadState[0] = "";
                    _threadState[1] = "";
                    _threadState[2] = "";
                    _threadState[3] = "";
                }
                finally
                {
                    Thread.Sleep(1000);
                    worker.ReportProgress(0);
                }
            }
        }

        /// <summary>
        /// Update information about selected thread.
        /// </summary>
        private void bwThreadFields_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ThrStartTimeLabel.Content = _threadState[0];
            ThrTotalTimeLabel.Content = _threadState[1];
            ThrUserTimeLabel.Content = _threadState[2];
            WaitReasonLabel.Content = _threadState[3];
        }

        private void ThreadsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessThread curThread = (ThreadsListView.SelectedItem as ProcessThread);
            if (curThread == null)
                return;
            if (_curThread != curThread)
            {
                _curThread = curThread;
                if (_flagBwThreadRun == false)
                {
                    _flagBwThreadRun = true;
                    _bwThreadFields.RunWorkerAsync();
                }
            }
        }
    }
}