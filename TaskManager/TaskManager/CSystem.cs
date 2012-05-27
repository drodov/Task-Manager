using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager
{
    public class CSystem
    {
        public CSystem()
        {
        }
        public CSystem(CSystem syst)
        {
            Name = syst.Name;
            BuildNum = syst.BuildNum;
            CSDVersion = syst.CSDVersion;
            CSName = syst.CSName;
            InstallDate = syst.InstallDate;
            LastBootUpTime = syst.LastBootUpTime;
            OSArchitecture = syst.OSArchitecture;
            SerialNumber = syst.SerialNumber;
            this.Version = syst.Version;
            ProcName = SystemInfo.GetProcName();
            PhysMemory = (SystemInfo.GetTotalPhysicalMemory() / 1024 / 1024).ToString() + " Mb";
        }
        public string Name { set; get; }
        public string BuildNum { set; get; }
        public string CSDVersion { set; get; }
        public string CSName { set; get; }
        public string InstallDate { set; get; }
        public string LastBootUpTime { set; get; }
        public string OSArchitecture { set; get; }
        public string SerialNumber { set; get; }
        public string Version { set; get; }
        public string ProcName { set; get; }
        public string PhysMemory { set; get; }
    }
}
