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
    /// Interaction logic for ShowProcInfoWindow.xaml
    /// </summary>
    public partial class ShowProcInfoWindow : Window
    {
        /// <summary>
        /// Selected process to view additional information. 
        /// </summary>
        Process ProcToView;

        /// <summary>
        /// Window creation for viewing selected process.
        /// </summary>
        /// <param name="SelectedProc">Selected process.</param>
        public ShowProcInfoWindow(Process SelectedProc)
        {
            //try
            //{
                InitializeComponent();
                ProcToView = SelectedProc;
                foreach (ComboBoxItem val in PriorComboBox.Items)
                {
                    if (val.Content.ToString() == ProcToView.PriorityClass.ToString())
                        PriorComboBox.SelectedItem = val;
                }
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
            //}
            //catch (Exception Ex)
            //{
            //    MessageBox.Show(Ex.Message + ".\nОтсуствуют права.");
            //}
        }

        /// <summary>
        /// Call changing priority of selected process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PriorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch((PriorComboBox.SelectedItem as ComboBoxItem).Content.ToString().ToLower())
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
}