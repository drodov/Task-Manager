﻿using System;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
  //          ListViewItem temp = new ListViewItem();
            List<Process> ProcColl = new List<Process>();
            foreach (Process proc in Process.GetProcesses())
            {
                ProcColl.Add(proc);
            }
            ProcListView.ItemsSource = ProcColl;
        }

        private void ProcRefreshButton_Click(object sender, RoutedEventArgs e)
        {
 //           ListViewItem temp = new ListViewItem();
            List<Process> ProcColl = new List<Process>();
            foreach (Process proc in Process.GetProcesses())
            {
                ProcColl.Add(proc);
            }
            ProcListView.ItemsSource = ProcColl;
        }

        private void ProcCreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreatProcWindow CrPrWind = new CreatProcWindow();
            CrPrWind.ShowDialog();
            ProcRefreshButton_Click(sender, e);
        }

        private void ProcKillButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process ProcToKill = (Process)ProcListView.SelectedItem;
                if (ProcToKill == null)
                    return;
                ProcToKill.Kill();
                ProcRefreshButton_Click(sender, e);
            }
            catch (Exception)
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            List<Process> ProcColl = new List<Process>(ProcListView.ItemsSource as List<Process>);
            ProcColl.Sort(new Comparison<Process>(delegate( Process a, Process b)
            {
                return String.Compare(a.ProcessName, b.ProcessName);
            }));
            ProcListView.ItemsSource = ProcColl;
            /*switch ((e.OriginalSource as GridViewColumnHeader).Content.ToString())
            {
                case "Name": ProcColl.ToArray
            }*/

        }
    }
}