using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;

namespace TaskManager
{
    class Proc
    {
        public Proc(Process proc)
        {
            try
            {
                Id = proc.Id;
                Name = proc.ProcessName;
                ThreadsCount = proc.Threads.Count;
                Priority = proc.BasePriority;

                ObjectQuery sq = new ObjectQuery
                    ("Select * from Win32_Process Where ProcessID = '" + proc.Id.ToString() + "'");
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
        public int Id { set; get; }
        public string Name { set; get; }
        public int ThreadsCount { set; get; }
        public int Priority { set; get; }
        public string User { set; get; }
        public string Description { set; get; }
        public static explicit operator Process(Proc p)
        {
            return Process.GetProcessById(p.Id);
        }
    }
}
