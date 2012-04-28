using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;

namespace TaskManager
{
    /// <summary>
    /// Represents processes.
    /// </summary>
    class Proc
    {
        /// <summary>
        /// Initializes a new instance of TaskManager.Proc class to the value of System.Diagnostics.Process object.
        /// </summary>
        /// <param name="proc">Object of System.Diagnostics.Process class of witch base TaskManager.Proc object is created.</param>
        public Proc(Process proc)
        {
            try
            {
                Id = proc.Id;
                Name = proc.ProcessName;
                ThreadsCount = proc.Threads.Count;
                Priority = proc.BasePriority;

                ObjectQuery sq = new ObjectQuery
                    ("Select * from Win32_Process Where ProcessID = '" + proc.Id.ToString() + "'"); // получаем владельца процесса
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);
                foreach (ManagementObject oReturn in searcher.Get())
                {
                    string[] o = new String[2];
                    oReturn.InvokeMethod("GetOwner", (object[])o);
                    //User = o[0] != null ? o[0] : "";
                    User = o[0];
                }

                Description = proc.MainModule.FileVersionInfo.FileDescription;
            }
            catch (Exception)
            {
                Description = "";
            }
        }

        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// Gets name of the associated process.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Gets threads count of the associated process.
        /// </summary>
        public int ThreadsCount { set; get; }

        /// <summary>
        /// Gets priority of the associated process.
        /// </summary>
        public int Priority { set; get; }

        /// <summary>
        /// Gets owner of the associated process.
        /// </summary>
        public string User { set; get; }

        /// <summary>
        /// Gets description of the associated process.
        /// </summary>
        public string Description { set; get; }
        /*public static explicit operator Process(Proc p)
        {
            return Process.GetProcessById(p.Id);
        }*/
    }
}
