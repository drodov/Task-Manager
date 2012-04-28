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
using System.Windows.Threading;
using System.ComponentModel;


namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Collection of processes.
        /// </summary>
        List<Proc> _procColl;

        /// <summary>
        /// Collection of apps.
        /// </summary>
        List<CApp> _appColl;

        /// <summary>
        /// Collection of services.
        /// </summary>
        List<CService> _servColl;

        /// <summary>
        /// Kind of sorting processes: 1 - by name; 2 - by Id; 3 - by count of threads; 4 - by priority; 5 - by description; 6 - by process's owner.
        /// </summary>

        int _flagProcSort = 1;
        /// <summary>
        /// Kind of sorting apps: 1 - by name; 2 - by responding.
        /// </summary>
        int _flagAppSort = 1;

        /// <summary>
        /// Kind of sorting services: 1 - by name; 2 - by process's Id; 3 - by description; 4 - by running status.
        /// </summary>
        int _flagServSort = 1;

        /// <summary>
        /// Directions of processes sorting: true - ascending; false - descending.
        /// </summary>
        Boolean[] _directionProcSorting = new Boolean[6] { false, false, false, false, false, false };

        /// <summary>
        /// Directions of services sorting: true - ascending; false - descending.
        /// </summary>
        Boolean[] _directionServSorting = new Boolean[4] { false, false, false, false };

        /// <summary>
        /// Directions of apps sorting: true - ascending; false - descending.
        /// </summary>
        Boolean[] _directionAppSorting = new Boolean[2] { false, false };
        
        /// <summary>
        /// Var for pointing direction of sorting in ListView.
        /// </summary>
        ListSortDirection _direction;

        /// <summary>
        /// Total virtual memory on PC.
        /// </summary>
        long _totalVirtualMemory;

        /// <summary>
        /// Total physical memory on PC.
        /// </summary>
        long _totalPhysicalMemory = 0;

        /// <summary>
        /// Current value of using virtual memory.
        /// </summary>
        long _curVirtualMemory;

        /// <summary>
        /// Current value of using physical memory.
        /// </summary>
        long _curPhysicalMemory;

        public MainWindow()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High; // уст. приоритет нашего приложения
            InitializeComponent();
        }

        /// <summary>
        /// Refresh list pf processes.
        /// </summary>
        void ProcRefresh()
        {
            _curPhysicalMemory = 0;
            _curVirtualMemory = 0;
            _totalPhysicalMemory = 0;
            ManagementObjectSearcher man = new ManagementObjectSearcher("SELECT TotalVirtualMemorySize FROM Win32_OperatingSystem"); // получаеем размер вирт.памяти
            foreach (ManagementObject obj in man.Get())
                _totalVirtualMemory = long.Parse(obj["TotalVirtualMemorySize"].ToString());
            man = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory"); // получаем объем физ. памяти
            foreach (ManagementObject obj in man.Get())
                _totalPhysicalMemory += long.Parse(obj["Capacity"].ToString());
            _procColl = new List<Proc>();

            foreach (Process proc in Process.GetProcesses())    // получаем процессы
            {
                _procColl.Add(new Proc(proc));
                _curPhysicalMemory += proc.WorkingSet64;        // подсчет используемой физ. памяти
                _curVirtualMemory += proc.VirtualMemorySize64;  // подсчет используемой виртуальной памяти
            }
            ProcCountLabel.Content = _procColl.Count.ToString();
            PhMemLabel.Content = (100 * _curPhysicalMemory / _totalPhysicalMemory).ToString() + "%"; // процент используемой физ. памяти

            man = new ManagementObjectSearcher("SELECT LoadPercentage  FROM Win32_Processor"); // CPU
            foreach (ManagementObject obj in man.Get())
                CPUPercentLabel.Content = obj["LoadPercentage"].ToString() + "%";
            ProcListView.ItemsSource = _procColl;       // указываем источник ListView
            /* switch (_flagProcSort)
             {
                 case 1: SortByName(_procColl); break;
                 case 2: SortById(_procColl); break;
                 case 3: SortByThreads(_procColl); break;
                 case 4: SortByPrior(_procColl); break;
                 case 5: SortByThreads(_procColl); break;
                 case 6: SortByPrior(_procColl); break;
                 default: break;
             }*/
        }

        /// <summary>
        /// Refresh list of apps.
        /// </summary>
        void AppRefresh()
        {
            _appColl = new List<CApp>();
            foreach (Process proc in Process.GetProcesses()) // выбираем процессы, у которых есть окна
            {
                string[] argList = new string[] { string.Empty };
                if (proc.MainWindowTitle != "")
                {
                    _appColl.Add(new CApp(proc));
                }
            }/*
            switch (_flagAppSort)
            {
                case 1: SortByTask(_appColl); break;
                case 2: SortByResp(_appColl); break;
                default: break;
            }*/
            AppListView.ItemsSource = _appColl; // отображение приложений
        }

        /// <summary>
        /// Refresh list of services.
        /// </summary>
        void ServRefresh()
        {
            _servColl = new List<CService>();
            foreach (ServiceController srvc in ServiceController.GetServices()) // получаем список служб
            {
                CService s = new CService(srvc);
                _servColl.Add(s);
            }/*
            switch (_flagServSort)
            {
                case 1: SortByName(_servColl); break;
                case 2: SortByDescr(_servColl); break;
                case 3: SortByStatus(_servColl); break;
                case 4: SortById(_servColl); break;
                default: break;
            }*/
            ServListView.ItemsSource = _servColl; // отображаем
        }

        /// <summary>
        /// Call process's refreshing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcRefreshButton_Click(object sender, RoutedEventArgs e)
        {
        //Refresh RefreshAll = new Refresh(ProcRefresh);
        //this.ProcRefreshButton.Dispatcher.BeginInvoke(DispatcherPriority.Normal, RefreshAll);
            ProcRefresh();
        }

        /// <summary>
        /// Call app's refreshing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            AppRefresh();
        }

        /// <summary>
        /// Call serve's refreshing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ServRefresh();
        }

        /// <summary>
        /// Call starting new task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskStartButton_Click(object sender, RoutedEventArgs e)
        {
            CreatProcWindow CrPrWind = new CreatProcWindow();
            CrPrWind.ShowDialog();
            AppRefresh();
            ProcRefresh();
        }

        /// <summary>
        /// Call killing of process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcKillButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process ProcToKill = Process.GetProcessById((ProcListView.SelectedItem as Proc).Id);
                if (ProcToKill == null)
                    return;
                ProcToKill.Kill();
                ProcRefresh();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Call killing of app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppKillButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process AppToKill = Process.GetProcessById((AppListView.SelectedItem as CApp).Id);
                if (AppToKill == null)
                    return;
                AppToKill.Kill();
                AppRefresh();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Call closing of app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppCloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process ProcToClose = Process.GetProcessById((AppListView.SelectedItem as CApp).Id);
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

        /// <summary>
        /// Call list processes sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewProcColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource as GridViewColumnHeader == null)
                return;
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
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("Name", _direction));
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
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("ThreadsCount", _direction));
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
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("Priority", _direction));
                    _flagProcSort = 4;
                    break;
                case "description":
                    ProcListView.Items.SortDescriptions.Clear();
                    if (_flagProcSort == 5 && _directionProcSorting[4] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionProcSorting[4] = false;
                    }
                    else if (_flagProcSort == 5 && _directionProcSorting[4] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[4] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[4] = true;
                    }
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("Description", _direction));
                    _flagProcSort = 5;
                    break;
                case "user":
                    ProcListView.Items.SortDescriptions.Clear();
                    if (_flagProcSort == 6 && _directionProcSorting[5] == true)
                    {
                        _direction = ListSortDirection.Descending;
                        _directionProcSorting[5] = false;
                    }
                    else if (_flagProcSort == 6 && _directionProcSorting[5] == false)
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[5] = true;
                    }
                    else
                    {
                        _direction = ListSortDirection.Ascending;
                        _directionProcSorting[5] = true;
                    }
                    ProcListView.Items.SortDescriptions.Add(new SortDescription("User", _direction));
                    _flagProcSort = 6;
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

        /// <summary>
        /// Call list apps sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewAppColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource as GridViewColumnHeader == null)
                return;
            if ((e.OriginalSource as GridViewColumnHeader).Content == null)
                return;
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

        /// <summary>
        /// Call list services sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewServColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource as GridViewColumnHeader == null)
                return;
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
        /*
                void SortByName(List<Proc> lst)
                {
                    lst.Sort(new Comparison<Proc>((Proc a, Proc b) =>
                    {
                        return String.Compare(a.Name, b.Name);
                    }));
                }

                void SortById(List<Proc> lst)
                {
                    lst.Sort(new Comparison<Proc>((Proc a, Proc b) =>
                    {
                        if (a.Id > b.Id)
                            return 1;
                        else if (a.Id == b.Id)
                            return 0;
                        else
                            return -1;
                    }));
                }

                void SortByThreads(List<Proc> lst)
                {
                    lst.Sort(new Comparison<Proc>((Proc a, Proc b) =>
                    {
                        if (a.ThreadsCount > b.ThreadsCount)
                            return 1;
                        else if (a.ThreadsCount == b.ThreadsCount)
                            return 0;
                        else
                            return -1;
                    }));
                }

                void SortByPrior(List<Proc> lst)
                {
                    lst.Sort(new Comparison<Proc>((Proc a, Proc b) =>
                    {
                        if (a.Priority > b.Priority)
                            return 1;
                        else if (a.Priority == b.Priority)
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
                }*/

        /// <summary>
        /// Show selected process's additional information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process ProcToView = Process.GetProcessById((ProcListView.SelectedItem as Proc).Id);
                if (ProcToView != null)
                {
                    ProcToView.PriorityClass.ToString();
                    ShowProcInfoWindow ShwPrInfWind = new ShowProcInfoWindow(ProcToView);
                    ShwPrInfWind.ShowDialog();
                }
            }
            catch (Exception)
            {
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
