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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections;
using TaskManager;
using System.ServiceProcess;
using System.Management;
using System.Threading;

 
namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Process> ProcColl;
        List<Process> AppColl;
        List<ServiceController> ServColl;
        int FlagProcSort = 0;
        int FlagAppSort = 0;
        int FlagServSort = 0;
        public MainWindow()
        {
            InitializeComponent();
            ProcRefresh();
            AppRefresh();
            ServRefresh();
        }

        void ProcRefresh()
        {
            ProcColl = new List<Process>();
            foreach (Process proc in Process.GetProcesses())
            {
                ProcColl.Add(proc);
            }
            ProcCountLabel.Content = ProcColl.Count.ToString();

            System.Management.ManagementObjectSearcher man = new System.Management.ManagementObjectSearcher("SELECT LoadPercentage  FROM Win32_Processor");
            foreach (System.Management.ManagementObject obj in man.Get())
                CPUPercentLabel.Content = obj["LoadPercentage"].ToString() + "%";

            switch (FlagProcSort)
            {
                case 1: SortByName(ProcColl); break;
                case 2: SortById(ProcColl); break;
                case 3: SortByThreads(ProcColl); break;
                case 4: SortByPrior(ProcColl); break;
                default: break;
            }
            ProcListView.ItemsSource = ProcColl;
        }

        void AppRefresh()
        {
            AppColl = new List<Process>();
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.MainWindowTitle != "")
                    AppColl.Add(proc);
            }
            switch (FlagAppSort)
            {
                case 1: SortByTask(AppColl); break;
                case 2: SortByResp(AppColl); break;
                default: break;
            }
            AppListView.ItemsSource = AppColl;
        }

        void ServRefresh()
        {
            ServColl = new List<ServiceController>();
            foreach (ServiceController srvc in ServiceController.GetServices())
            {
                ServColl.Add(srvc);
                ManagementObject MO = new ManagementObject(@"Win32_service.Name='" + srvc.ServiceName + "'");
                //MessageBox.Show(MO.GetPropertyValue("ProcessID").ToString());
            }
            switch (FlagServSort)
            {
                case 1: SortByName(ServColl); break;
                case 2: SortByDescr(ServColl); break;
                default: break;
            }
            ServListView.ItemsSource = ServColl;
        }

        private void ProcRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ProcRefresh();
        }

        private void AppRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            AppRefresh();
        }

        private void ServRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ServRefresh();
        }

        private void TaskStartButton_Click(object sender, RoutedEventArgs e)
        {
            CreatProcWindow CrPrWind = new CreatProcWindow();
            CrPrWind.ShowDialog();
            AppRefresh();
            ProcRefresh();
        }

        private void ProcKillButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process ProcToKill = (Process)ProcListView.SelectedItem;
                if (ProcToKill == null)
                    return;
                ProcToKill.Kill();
                ProcRefresh();
            }
            catch (Exception)
            {
            }
        }

        private void AppCloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process ProcToClose = (Process)AppListView.SelectedItem;
                if (ProcToClose == null)
                    return;
                ProcToClose.CloseMainWindow();
                AppRefresh();
            }
            catch (Exception)
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void GridViewProcColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "name": FlagProcSort = 1; break;
                case "id": FlagProcSort = 2; break;
                case "threads": FlagProcSort = 3; break;
                case "priority": FlagProcSort = 4; break;
                default: FlagProcSort = 0; break;
            }
            ProcRefresh();
        }

        private void GridViewAppColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "task": FlagAppSort = 1; break;
                case "responding": FlagAppSort = 2; break;
                default: FlagAppSort = 0; break;
            }
            AppRefresh();
        }

        private void GridViewServColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "name": FlagServSort = 1; break;
                case "description": FlagServSort = 2; break;
                default: FlagServSort = 0; break;
            }
            ServRefresh();
        }

        void SortByName(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>(delegate(Process a, Process b)
            {
                return String.Compare(a.ProcessName, b.ProcessName);
            }));
        }

        void SortById(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>(delegate(Process a, Process b)
            {
                if (a.Id > b.Id)
                    return 1;
                else if (a.Id == b.Id)
                    return 0;
                else
                    return -1;
            }));
        }

        void SortByThreads(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>(delegate(Process a, Process b)
            {
                if (a.Threads.Count > b.Threads.Count)
                    return 1;
                else if (a.Threads.Count == b.Threads.Count)
                    return 0;
                else
                    return -1;
            }));
        }

        void SortByPrior(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>(delegate(Process a, Process b)
            {
                if (a.BasePriority > b.BasePriority)
                    return 1;
                else if (a.BasePriority == b.BasePriority)
                    return 0;
                else
                    return -1;
            }));
        }

        void SortByTask(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>(delegate(Process a, Process b)
            {
                return String.Compare(a.MainWindowTitle.ToString(), b.MainWindowTitle.ToString());
            }));
        }

        void SortByResp(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>(delegate(Process a, Process b)
            {
                if (a.Responding == b.Responding)
                    return 0;
                else if (a.Responding == false && b.Responding == true)
                    return 1;
                else 
                    return -1;
            }));
        }

        void SortByName(List<ServiceController> lst)
        {
            lst.Sort(new Comparison<ServiceController>(delegate(ServiceController a, ServiceController b)
            {
                return String.Compare(a.ServiceName, b.ServiceName);
            }));
        }

        void SortByDescr(List<ServiceController> lst)
        {
            lst.Sort(new Comparison<ServiceController>(delegate(ServiceController a, ServiceController b)
            {
                return String.Compare(a.DisplayName, b.DisplayName);
            }));
        }

        private void ProcListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process ProcToView =  ProcListView.SelectedItem as Process;
            if (ProcToView != null)
            {
                ShowProcInfoWindow ShwPrInfWind = new ShowProcInfoWindow(ProcToView);
                ShwPrInfWind.ShowDialog();
            }
        }
    }
}
