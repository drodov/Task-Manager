using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Management;


namespace TaskManager
{
    class CApp
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public bool Status { set; get; }
        public BitmapImage AppIcon { set; get; }
        public CApp(Process proc)
        {
            Id = proc.Id;
            Name = proc.MainWindowTitle;
            Status = proc.Responding;
            AppIcon = Icon.ExtractAssociatedIcon(proc.MainModule.FileName).ToBitmap().ToBitmapImage();
        }
    }
}
