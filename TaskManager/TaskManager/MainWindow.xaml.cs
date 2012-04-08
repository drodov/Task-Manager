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
using System.ServiceProcess;
using System.Management;
using System.Threading;
using System.ComponentModel;


namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Process> _procColl;
        List<CApp> _appColl;
        List<CService> _servColl;
        int _flagProcSort = 1;
        int _flagAppSort = 1;
        int _flagServSort = 1;
        Boolean[] _directionProcSorting = new Boolean[4] { true, false, false, false };
        Boolean[] _directionServSorting = new Boolean[4] { true, false, false, false };
        Boolean[] _directionAppSorting = new Boolean[2] { true, false };
        ListSortDirection _direction;
        public MainWindow()
        {
            InitializeComponent();
        }

        void ProcRefresh()
        {
            _procColl = new List<Process>();
            foreach (Process proc in Process.GetProcesses())
            {
                _procColl.Add(proc);
            }
            ProcCountLabel.Content = _procColl.Count.ToString();

            System.Management.ManagementObjectSearcher man = new System.Management.ManagementObjectSearcher("SELECT LoadPercentage  FROM Win32_Processor");
            foreach (System.Management.ManagementObject obj in man.Get())
                CPUPercentLabel.Content = obj["LoadPercentage"].ToString() + "%";

            switch (_flagProcSort)
            {
                case 1: SortByName(_procColl); break;
                case 2: SortById(_procColl); break;
                case 3: SortByThreads(_procColl); break;
                case 4: SortByPrior(_procColl); break;
                default: break;
            }
            ProcListView.ItemsSource = _procColl;
        }

        void AppRefresh()
        {
            _appColl = new List<CApp>();
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.MainWindowTitle != "")
                {
                    _appColl.Add(new CApp(proc));
                }
            }
            switch (_flagAppSort)
            {
                case 1: SortByTask(_appColl); break;
                case 2: SortByResp(_appColl); break;
                default: break;
            }
            AppListView.ItemsSource = _appColl;
        }

        void ServRefresh()
        {
            _servColl = new List<CService>();
            foreach (ServiceController srvc in ServiceController.GetServices())
            {
                CService s = new CService(srvc);
                _servColl.Add(s);
            }
            switch (_flagServSort)
            {
                case 1: SortByName(_servColl); break;
                case 2: SortByDescr(_servColl); break;
                case 3: SortByStatus(_servColl); break;
                case 4: SortById(_servColl); break;
                default: break;
            }
            ServListView.ItemsSource = _servColl;
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
                case "name":
                    ProcListView.Items.SortDescriptions.Clear();
                    if (_flagProcSort == 1 && _directionProcSorting[0] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionProcSorting[0] = false;
                    }
                    else if (_flagProcSort == 1 && _directionProcSorting[0] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[0] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[0] = true;
                    }
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("ProcessName", _direction));
                    _flagProcSort = 1;
                    break;
                case "id":
                    ProcListView.Items.SortDescriptions.Clear();
                    if (_flagProcSort == 2 && _directionProcSorting[1] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionProcSorting[1] = false;
                    }
                    else if (_flagProcSort == 2 && _directionProcSorting[1] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[1] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[1] = true;
                    }
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("Id", _direction));
                    _flagProcSort = 2;
                    break;
                case "threads":
                    ProcListView.Items.SortDescriptions.Clear();
                    if (_flagProcSort == 3 && _directionProcSorting[2] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionProcSorting[2] = false;
                    }
                    else if (_flagProcSort == 3 && _directionProcSorting[2] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[2] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[2] = true;
                    }
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("Threads.Count", _direction));
                    _flagProcSort = 3;
                    break;
                case "priority":
                    ProcListView.Items.SortDescriptions.Clear();
                    if (_flagProcSort == 4 && _directionProcSorting[3] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionProcSorting[3] = false;
                    }
                    else if (_flagProcSort == 4 && _directionProcSorting[3] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[3] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[3] = true;
                    }
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("BasePriority", _direction));
                    _flagProcSort = 4;
                    break;
                default: break;
            }
            /*
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "name": _flagProcSort = 1; break;
                case "id": _flagProcSort = 2; break;
                case "threads": _flagProcSort = 3; break;
                case "priority": _flagProcSort = 4; break;
                default: _flagProcSort = 0; break;
            }
            ProcRefresh();*/
        }

        private void GridViewAppColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "apps":
                    AppListView.Items.SortDescriptions.Clear();
                    if (_flagAppSort == 1 && _directionAppSorting[0] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionAppSorting[0] = false;
                    }
                    else if (_flagAppSort == 1 && _directionAppSorting[0] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionAppSorting[0] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionAppSorting[0] = true;
                    }
                    AppListView.Items.SortDescriptions.Add(new SortDescription("Name", _direction));
                    _flagAppSort = 1;
                    break;
                case "responding":
                    AppListView.Items.SortDescriptions.Clear();
                    if (_flagAppSort == 2 && _directionAppSorting[1] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionAppSorting[1] = false;
                    }
                    else if (_flagAppSort == 2 && _directionAppSorting[1] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionAppSorting[1] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionAppSorting[1] = true;
                    }
                    AppListView.Items.SortDescriptions.Add(new SortDescription("Status", _direction));
                    _flagAppSort = 2;
                    break;
                default: break;
            }
            /*
            if ((e.OriginalSource as GridViewColumnHeader).Content == null)
                return;
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "task": _flagAppSort = 1; break;
                case "responding": _flagAppSort = 2; break;
                default: _flagAppSort = 0; break;
            }
            AppRefresh();*/
        }

        private void GridViewServColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "name":
                    ServListView.Items.SortDescriptions.Clear();
                    if (_flagServSort == 1 && _directionServSorting[0] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionServSorting[0] = false;
                    }
                    else if (_flagServSort == 1 && _directionServSorting[0] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[0] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[0] = true;
                    }
                    ServListView.Items.SortDescriptions.Add(new SortDescription("Name", _direction));
                    _flagServSort = 1;
                    break;
                case "id":
                    ServListView.Items.SortDescriptions.Clear();
                    if (_flagServSort == 2 && _directionServSorting[1] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionServSorting[1] = false;
                    }
                    else if (_flagServSort == 2 && _directionServSorting[1] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[1] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[1] = true;
                    }
                    ServListView.Items.SortDescriptions.Add(new SortDescription("Id", _direction));
                    _flagServSort = 2;
                    break;
                case "description":
                    ServListView.Items.SortDescriptions.Clear();
                    if (_flagServSort == 3 && _directionServSorting[2] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionServSorting[2] = false;
                    }
                    else if (_flagServSort == 3 && _directionServSorting[2] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[2] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[2] = true;
                    }
                    ServListView.Items.SortDescriptions.Add(new SortDescription("Description", _direction));
                    _flagServSort = 3;
                    break;
                case "status":
                    ServListView.Items.SortDescriptions.Clear();
                    if (_flagServSort == 4 && _directionServSorting[3] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionServSorting[3] = false;
                    }
                    else if (_flagServSort == 4 && _directionServSorting[3] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[3] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionServSorting[3] = true;
                    }
                    ServListView.Items.SortDescriptions.Add(new SortDescription("Status", _direction));
                    _flagServSort = 4;
                    break;
                default: break;
            }
            /*
            switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString().ToLower())
            {
                case "name": _flagServSort = 1; break;
                case "description": _flagServSort = 2; break;
                case "status": _flagServSort = 3; break;
                case "id": _flagServSort = 4; break;
                default: _flagServSort = 0; break;
            }
            ServRefresh();*/
        }

        void SortByName(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>((Process a, Process b) =>
            {
                return String.Compare(a.ProcessName, b.ProcessName);
            }));
        }

        void SortById(List<Process> lst)
        {
            lst.Sort(new Comparison<Process>((Process a, Process b) =>
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
            lst.Sort(new Comparison<Process>((Process a, Process b) =>
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
            lst.Sort(new Comparison<Process>((Process a, Process b) =>
            {
                if (a.BasePriority > b.BasePriority)
                    return 1;
                else if (a.BasePriority == b.BasePriority)
                    return 0;
                else
                    return -1;
            }));
        }

        void SortByTask(List<CApp> lst)
        {
            lst.Sort(new Comparison<CApp>((CApp a, CApp b) =>
            {
                return String.Compare(a.Name, b.Name.ToString());
            }));
        }

        void SortByResp(List<CApp> lst)
        {
            lst.Sort(new Comparison<CApp>((CApp a, CApp b) =>
            {
                if (a.Status == b.Status)
                    return 0;
                else if (a.Status == false && b.Status == true)
                    return 1;
                else
                    return -1;
            }));
        }

        void SortByName(List<CService> lst)
        {
            lst.Sort(new Comparison<CService>((CService a, CService b) =>
            {
                return String.Compare(a.Name, b.Name);
            }));
        }

        void SortByDescr(List<CService> lst)
        {
            lst.Sort(new Comparison<CService>((CService a, CService b) =>
            {
                return String.Compare(a.Description, b.Description);
            }));
        }

        void SortByStatus(List<CService> lst)
        {
            lst.Sort(new Comparison<CService>((CService a, CService b) =>
            {
                return String.Compare(a.Status.ToString(), b.Status.ToString());
            }));
        }

        void SortById(List<CService> lst)
        {
            lst.Sort(new Comparison<CService>((CService a, CService b) =>
            {
                if (a.Id > b.Id)
                    return 1;
                else if (a.Id == b.Id)
                    return 0;
                else
                    return -1;
            }));
        }

        private void ProcListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process ProcToView = ProcListView.SelectedItem as Process;
            if (ProcToView != null)
            {
                ShowProcInfoWindow ShwPrInfWind = new ShowProcInfoWindow(ProcToView);
                ShwPrInfWind.ShowDialog();
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ProcRefresh();
            AppRefresh();
            ServRefresh();
        }
    }
}
