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
    /// <summary>
    /// Represents apps (processes with windows).
    /// </summary>
    public class CApp
    {
        /// <summary>
        /// Gets the unique identifier for the process of the associated App.
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// Gets the name for the associated App.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Gets the responding status for the associated App.
        /// </summary>
        public bool Status { set; get; }

        /// <summary>
        /// Gets the icon for the associated App.
        /// </summary>
        public BitmapImage AppIcon { set; get; }

        /// <summary>
        /// Initializes a new instance of TaskManager.CApp class to the value of System.Diagnostics.Process object.
        /// </summary>
        /// <param name="proc">Object of System.Diagnostics.Process class of which base TaskManager.CApp object is created.</param>
        public CApp(Process proc)
        {
            Id = proc.Id;
            Name = proc.MainWindowTitle;
            Status = proc.Responding;
            AppIcon = Icon.ExtractAssociatedIcon(proc.MainModule.FileName).ToBitmap().ToBitmapImage();
        }

        /// <summary>
        /// Initializes a instance of TaskManager.CApp class.
        /// </summary>
        public CApp()
        {
            Id = 0;
            Name = "";
            Status = false;
        }

        /// <summary>
        /// Initializes a instance of TaskManager.CApp class to the value of System.Diagnostics.Process object.
        /// </summary>
        /// <param name="proc">Object of System.Diagnostics.Process class for initialization.</param>
        public void CopyFromProcess(Process proc)
        {
            Id = proc.Id;
            Name = proc.MainWindowTitle;
            Status = proc.Responding;
            AppIcon = Icon.ExtractAssociatedIcon(proc.MainModule.FileName).ToBitmap().ToBitmapImage();
        }
    }
}
