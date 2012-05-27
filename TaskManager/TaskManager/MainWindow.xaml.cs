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
using System.Runtime.InteropServices;
using ZedGraph;
using System.Net.NetworkInformation;
using TcpUdpConnections;


namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int NUM_POINTS = 50;
        int _physMemSize = (int)(SystemInfo.GetTotalPhysicalMemory() / 1024 / 1024);
        int _physUsage;
        int _virtUsage;
        int _CPUsage;
        int _pageFileSize;
        int _curPageFileSize;
        long _prevBSent = 0;
        long _prevBReceived = 0;
        int[] _masCPU = new int[NUM_POINTS];
        int[] _masPageFile = new int[NUM_POINTS];
        int[] _masPhysMem = new int[NUM_POINTS];
        bool _flagShowNetSpeed = false;

        ZedGraphControl CPUZedGraph;
        ZedGraphControl PageFileZedGraph;
        ZedGraphControl PhysMemZedGraph;

        NetworkInterface[] _niArr;
        NetworkInterface _niToView = null;

        int? _appPIDSelect = null;
        int? _procPIDSelect = null;

        /// <summary>
        /// Collection of processes.
        /// </summary>
        List<Proc> _procColl;

        /// <summary>
        /// Collection of apps.
        /// </summary>
        List<CApp> _appColl = new List<CApp>();

        /// <summary>
        /// Collection of services.
        /// </summary>
        List<CService> _servColl = new List<CService>();

        /// <summary>
        /// Collection of sockets.
        /// </summary>
        List<CSocket> _socketColl = new List<CSocket>();

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

        BackgroundWorker _bwList = new BackgroundWorker();
        BackgroundWorker _bwStat = new BackgroundWorker();
        BackgroundWorker _bwNet = new BackgroundWorker();

        public MainWindow()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High; // уст. приоритет нашего приложения
            InitializeComponent();
        }

        /// <summary>
        /// Call starting new task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskStartButton_Click(object sender, RoutedEventArgs e)
        {
            Proc ProcToStart = new Proc();
            CApp AppToStart = new CApp();
            CreatProcWindow CrPrWind = new CreatProcWindow(ProcToStart, AppToStart);
            CrPrWind.ShowDialog();
            if (ProcToStart.Id != 0) // if new task has started
            {
                _procColl.Add(ProcToStart);
                _procColl = new List<Proc>(_procColl);
                ProcListView.ItemsSource = _procColl;
                if (AppToStart.Name != "") // if new task has window
                {
                    _appColl.Add(AppToStart);
                    _appColl = new List<CApp>(_appColl);
                    AppListView.ItemsSource = _appColl;
                }
            }
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
                _procColl.RemoveAll((Proc a) => // update proc. list
                {
                    if (a.Id == ProcToKill.Id)
                        return true;
                    else
                        return false;
                });
                _procColl = new List<Proc>(_procColl);
                ProcListView.ItemsSource = _procColl;

                _appColl.RemoveAll((CApp a) => // update app. list
                {
                    if (a.Id == ProcToKill.Id)
                        return true;
                    else
                        return false;
                });
                _appColl = new List<CApp>(_appColl);
                AppListView.ItemsSource = _appColl;
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
                Process ProcToKill = Process.GetProcessById((AppListView.SelectedItem as CApp).Id);
                if (ProcToKill == null)
                    return;
                ProcToKill.Kill();
                _procColl.RemoveAll((Proc a) => // update proc. list
                {
                    if (a.Id == ProcToKill.Id)
                        return true;
                    else
                        return false;
                });
                _procColl = new List<Proc>(_procColl);
                ProcListView.ItemsSource = _procColl;

                _appColl.RemoveAll((CApp a) => // update app. list
                {
                    if (a.Id == ProcToKill.Id)
                        return true;
                    else
                        return false;
                });
                _appColl = new List<CApp>(_appColl);
                AppListView.ItemsSource = _appColl;
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
                bool sucFlag = ProcToClose.CloseMainWindow();
                if (sucFlag == false) // can't close app safely
                {
                    AppKillButton_Click(sender, e);
                }
                else if (Process.GetProcessById(ProcToClose.Id) == null)
                {
                    _procColl.RemoveAll((Proc a) => // update proc. list
                    {
                        if (a.Id == ProcToClose.Id)
                            return true;
                        else
                            return false;
                    });
                    _procColl = new List<Proc>(_procColl);
                    ProcListView.ItemsSource = _procColl;

                    _appColl.RemoveAll((CApp a) => // update app. list
                    {
                        if (a.Id == ProcToClose.Id)
                            return true;
                        else
                            return false;
                    });
                    _appColl = new List<CApp>(_appColl);
                    AppListView.ItemsSource = _appColl;
                }
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
        }

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
                    ShowProcInfoWindow ShwPrInfWind = new ShowProcInfoWindow(ProcToView.Id);
                    ShwPrInfWind.ShowDialog();
                }
            }
            catch (Exception)
            {
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _procColl = SystemInfo.GetProcessList();
            ProcCountLabel.Content = _procColl.Count.ToString(); // кол-во процессов
            ProcListView.ItemsSource = _procColl;       // указываем источник ListView

            _appColl = SystemInfo.GetAppList();
            AppListView.ItemsSource = _appColl; // отображение приложений

            _servColl = SystemInfo.GetServiceList();
            ServListView.ItemsSource = _servColl; // отображаем

            _socketColl = GetTcpUdp.GetTcpUdpList();
            SocketListView.ItemsSource = _socketColl;

            PhMemLabel.Content = SystemInfo.GetPhysicalUsage().ToString() + "%"; // процент используемой физ. памяти
            VirtMemLabel.Content = SystemInfo.GetVirtualUsage().ToString() + "%"; // процент используемой вирт. памяти
            CPUPercentLabel.Content = SystemInfo.GetCPU().ToString() + "%"; // процент CPU

            // get information about OS, processor, RAM
            CSystem syst = new CSystem(SystemInfo.GetOSInfo());
            OSNameLabel.Content = syst.Name;
            BuildNumLabel.Content = syst.BuildNum;
            VersionLabel.Content = syst.Version;
            CSDVersionabel.Content = syst.CSDVersion;
            CSNameLabel.Content = syst.CSName;
            string date = syst.InstallDate;
            InstallDateLabel.Content = date.Substring(0, 4) + "." + date.Substring(4, 2) + "." + date.Substring(6, 2) + " " + date.Substring(8, 2) + ":" + date.Substring(10, 2) + ":" + date.Substring(12, 2);
            date = syst.LastBootUpTime;
            LastBootLabel.Content = date.Substring(0, 4) + "." + date.Substring(4, 2) + "." + date.Substring(6, 2) + " " + date.Substring(8, 2) + ":" + date.Substring(10, 2) + ":" + date.Substring(12, 2);
            OSArchLabel.Content = syst.OSArchitecture;
            SerialNumLabel.Content = syst.SerialNumber;
            ProcNameLabel.Content = syst.ProcName;
            RAMLabel.Content = syst.PhysMemory;

            CPUZedGraph = CPUWFH.Child as ZedGraphControl;
            CPUZedGraph.GraphPane.Title.Text = "CPU";
            CPUZedGraph.GraphPane.XAxis.Title.Text = "";
            CPUZedGraph.GraphPane.YAxis.Title.Text = "%";
            CPUZedGraph.GraphPane.YAxis.Scale.Max = 100;
            CPUZedGraph.GraphPane.YAxis.Scale.Min = 0;
            CPUZedGraph.GraphPane.XAxis.Scale.Max = 50;
            CPUZedGraph.GraphPane.XAxis.Scale.Min = 0;

            PageFileZedGraph = PageFileWFH.Child as ZedGraphControl;
            PageFileZedGraph.GraphPane.Title.Text = "Page File Usage";
            PageFileZedGraph.GraphPane.XAxis.Title.Text = "";
            PageFileZedGraph.GraphPane.YAxis.Title.Text = "%";
            PageFileZedGraph.GraphPane.YAxis.Scale.Max = 100;
            PageFileZedGraph.GraphPane.YAxis.Scale.Min = 0;
            PageFileZedGraph.GraphPane.XAxis.Scale.Max = 50;
            PageFileZedGraph.GraphPane.XAxis.Scale.Min = 0;

            PhysMemZedGraph = PhysMemWFH.Child as ZedGraphControl;
            PhysMemZedGraph.GraphPane.Title.Text = "Phys. Memory Usage";
            PhysMemZedGraph.GraphPane.XAxis.Title.Text = "";
            PhysMemZedGraph.GraphPane.YAxis.Title.Text = "%";
            PhysMemZedGraph.GraphPane.YAxis.Scale.Max = 100;
            PhysMemZedGraph.GraphPane.YAxis.Scale.Min = 0;
            PhysMemZedGraph.GraphPane.XAxis.Scale.Max = 50;
            PhysMemZedGraph.GraphPane.XAxis.Scale.Min = 0;

            _bwList.WorkerReportsProgress = true;
            _bwList.WorkerSupportsCancellation = true;
            _bwList.DoWork += new DoWorkEventHandler(bwList_DoWork);
            _bwList.ProgressChanged += new ProgressChangedEventHandler(bwList_ProgressChanged);

            _bwStat.WorkerReportsProgress = true;
            _bwStat.WorkerSupportsCancellation = true;
            _bwStat.DoWork += new DoWorkEventHandler(bwStat_DoWork);
            _bwStat.ProgressChanged += new ProgressChangedEventHandler(bwStat_ProgressChanged);

            _bwNet.WorkerReportsProgress = true;
            _bwNet.WorkerSupportsCancellation = true;
            _bwNet.DoWork += new DoWorkEventHandler(bwNet_DoWork);
            _bwNet.ProgressChanged += new ProgressChangedEventHandler(bwNet_ProgressChanged);

            _bwList.RunWorkerAsync();
            _bwStat.RunWorkerAsync();
            _bwNet.RunWorkerAsync();

            // Grab all local interfaces to this computer
            _niArr = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in _niArr)
            {
                ConnectComboBox.Items.Add(ni.Name);
            }
        }

        private void bwList_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                Thread.Sleep(5000);
                _procColl = SystemInfo.GetProcessList();
                _servColl = SystemInfo.GetServiceList();
                _appColl = SystemInfo.GetAppList();
                _socketColl = GetTcpUdp.GetTcpUdpList();
                _virtUsage = SystemInfo.GetVirtualUsage(); // процент используемой вирт. памяти
                worker.ReportProgress(0);
            }
        }

        private void bwList_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProcCountLabel.Content = _procColl.Count.ToString(); // кол-во процессов
            ProcListView.ItemsSource = _procColl;       // указываем источник ListView
            AppListView.ItemsSource = _appColl;
            ServListView.ItemsSource = _servColl;
            SocketListView.ItemsSource = _socketColl;
            if (_procPIDSelect != null)
            {
                foreach (Proc it in ProcListView.Items)
                {
                    if (it.Id == _procPIDSelect)
                    {
                        ProcListView.SelectedValue = it;
                        break;
                    }
                }
            }
            if (_appPIDSelect != null)
            {
                foreach (CApp it in AppListView.Items)
                {
                    if (it.Id == _appPIDSelect)
                    {
                        AppListView.SelectedValue = it;
                        break;
                    }
                }
            }
            PhMemLabel.Content = _physUsage.ToString() + "%"; // процент используемой физ. памяти
            VirtMemLabel.Content = _virtUsage.ToString() + "%"; // процент используемой вирт. памяти
        }

        private void bwStat_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                Thread.Sleep(1000);
                _CPUsage = SystemInfo.GetCPU();                 // процент загрузки ЦП
                _pageFileSize = SystemInfo.GetPageFileSize();   // размер файла подкачки
                _curPageFileSize = SystemInfo.GetPageFileCurUsage(); // текущее использование файла подкачки 
                _physUsage = SystemInfo.GetPhysicalUsage(); // процент используемой физ. памяти

                for (int i = 0; i < NUM_POINTS - 1; i++)
                {
                    _masCPU[i] = _masCPU[i + 1];
                    _masPageFile[i] = _masPageFile[i + 1];
                    _masPhysMem[i] = _masPhysMem[i + 1];
                }
                _masCPU[NUM_POINTS - 1] = _CPUsage;
                PrintCPUGraph();

                _masPageFile[NUM_POINTS - 1] = 100 * _curPageFileSize / _pageFileSize;
                PageFileZedGraph.GraphPane.Title.Text = "Page File Usage\nSize: " + _pageFileSize.ToString() + " Mb\nCurrent usage: " + _curPageFileSize.ToString() + " Mb";
                PrintPageFileGraph();

                _masPhysMem[NUM_POINTS - 1] = _physUsage;
                PhysMemZedGraph.GraphPane.Title.Text = "Phys. Memory Usage\nSize: " + _physMemSize.ToString() + " Mb\nCurrent usage: " + (_physMemSize / 100 * _physUsage).ToString() + " Mb";
                PrintPhysMemGraph();


                worker.ReportProgress(0);
            }
        }

        private void bwStat_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CPUPercentLabel.Content = _CPUsage.ToString() + "%"; // процент CPU
            // Обновляем график
            CPUZedGraph.Invalidate();
            PageFileZedGraph.Invalidate();
            PhysMemZedGraph.Invalidate();
        }

        private void bwNet_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                Thread.Sleep(1000);
                worker.ReportProgress(0);
            }
        }

        private void bwNet_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ConnectComboBox.SelectedValue != null)
            {
                string niName = ConnectComboBox.SelectedValue.ToString();
                for (int i = 0; i < _niArr.Length; i++)
                {
                    if (niName == _niArr[i].Name)
                    {
                        _niToView = _niArr[i];
                        break;
                    }
                }
                InterfaceLabel.Content = _niToView.NetworkInterfaceType.ToString();
                SpeedLabel.Content = _niToView.Speed;
                IPv4InterfaceStatistics stat = _niToView.GetIPv4Statistics();
                BSentLabel.Content = stat.BytesSent;
                BReceivLabel.Content = stat.BytesReceived;
                if (_flagShowNetSpeed == true)
                {
                    UploadLabel.Content = ((stat.BytesSent - _prevBSent) / 1024).ToString() + " KB/s";
                    DownloadLabel.Content = ((stat.BytesReceived - _prevBReceived) / 1024).ToString() + " KB/s";
                }
                else
                {
                    UploadLabel.Content = "";
                    DownloadLabel.Content = "";
                    _flagShowNetSpeed = true;
                }
                _prevBSent = stat.BytesSent;
                _prevBReceived = stat.BytesReceived;
            }
        }

        private void ShowDllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process ProcToView = Process.GetProcessById((ProcListView.SelectedItem as Proc).Id);
                if (ProcToView != null)
                {
                    ShowDLLs ShowwProcDll = new ShowDLLs(ProcToView.Id);
                    ShowwProcDll.ShowDialog();
                }
            }
            catch (Exception)
            {
            }
        }

        private void SuspResProcButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Proc selectProc = (ProcListView.SelectedItem as Proc);
                if (selectProc != null)
                {
                    if (selectProc.Status == true)
                        SuspendProcess(selectProc.Id);
                    else
                        ResumeProcess(selectProc.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void PrintCPUGraph()
        {
            GraphPane pane = CPUZedGraph.GraphPane;
            pane.CurveList.Clear();
            PointPairList list0 = new PointPairList();
            // Заполняем список точек
            for (int i = 0; i < NUM_POINTS; i++)
            {
                list0.Add(i, _masCPU[i]);
            }
            LineItem Curve0 = pane.AddCurve("", list0, System.Drawing.Color.Green, SymbolType.None);
            CPUZedGraph.AxisChange();
        }

        void PrintPageFileGraph()
        {
            GraphPane pane = PageFileZedGraph.GraphPane;
            pane.CurveList.Clear();
            PointPairList list0 = new PointPairList();
            // Заполняем список точек
            for (int i = 0; i < NUM_POINTS; i++)
            {
                list0.Add(i, _masPageFile[i]);
            }
            LineItem Curve0 = pane.AddCurve("", list0, System.Drawing.Color.Green, SymbolType.None);
            PageFileZedGraph.AxisChange();
        }

        void PrintPhysMemGraph()
        {
            GraphPane pane = PhysMemZedGraph.GraphPane;
            pane.CurveList.Clear();
            PointPairList list0 = new PointPairList();
            // Заполняем список точек
            for (int i = 0; i < NUM_POINTS; i++)
            {
                list0.Add(i, _masPhysMem[i]);
            }
            LineItem Curve0 = pane.AddCurve("", list0, System.Drawing.Color.Green, SymbolType.None);
            PhysMemZedGraph.AxisChange();
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        private void SuspendProcess(int PID)
        {
            Process proc = Process.GetProcessById(PID);
            if (proc.ProcessName == string.Empty)
                return;
            foreach (ProcessThread pT in proc.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
            }
        }

        public void ResumeProcess(int PID)
        {
            Process proc = Process.GetProcessById(PID);
            if (proc.ProcessName == string.Empty)
                return;
            foreach (ProcessThread pT in proc.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
            }
        }

        private void ConnectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _prevBSent = 0;
            _flagShowNetSpeed = false;
        }

        private void RefreshConnectionsButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectComboBox.Items.Clear();
            _niArr = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in _niArr)
            {
                ConnectComboBox.Items.Add(ni.Name);
            }
        }

        private void DumpButton_Click(object sender, RoutedEventArgs e)
        {
            Proc procToDump = (ProcListView.SelectedValue as Proc);
            if (procToDump != null)
            {
                MiniDump.MakeDump(procToDump.Id);
            }
        }

        private void ProcListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Proc procSelect = (ProcListView.SelectedValue as Proc);
            if (procSelect != null)
                _procPIDSelect = procSelect.Id;
        }

        private void AppListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CApp appSelect = (AppListView.SelectedValue as CApp);
            if (appSelect != null)
                _appPIDSelect = appSelect.Id;
        }
    }
}
