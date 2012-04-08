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
        Process ProcToView;
        public ShowProcInfoWindow(Process SelectedProc)
        {
            InitializeComponent();
            ProcToView = SelectedProc;
            try
            {
                foreach (ComboBoxItem val in PriorComboBox.Items)
                {
                    if (val.Content.ToString() == ProcToView.PriorityClass.ToString())
                        PriorComboBox.SelectedItem = val;
                }
                UserTimeLabel.Content = ProcToView.UserProcessorTime;
                TotalTimeLabel.Content = ProcToView.TotalProcessorTime;
                NonPagedSystMemLabel.Content = ProcToView.NonpagedSystemMemorySize64;
                PagedSystMemLabel.Content = ProcToView.PagedSystemMemorySize64;
                PagedMemLabel.Content = ProcToView.PagedMemorySize64;
                PeakPagedMemLabel.Content = ProcToView.PeakPagedMemorySize64;
                PeakVirtMemLabel.Content = ProcToView.PeakVirtualMemorySize64;
                PeakWorkingSetLabel.Content = ProcToView.PeakWorkingSet64;
                PrivateMemorySizeLabel.Content = ProcToView.PrivateMemorySize64;
                VirtualMemorySizeLabel.Content = ProcToView.VirtualMemorySize64;
                WorkingSetLabel.Content = ProcToView.WorkingSet64;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message + "\n.Запустите программу\nс правами администратора.");
            }
        }

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