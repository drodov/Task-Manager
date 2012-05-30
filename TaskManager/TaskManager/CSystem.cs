using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager
{
    /// <summary>
    /// Represents some information about OS and PC configuration.
    /// </summary>
    public class CSystem
    {

        /// <summary>
        /// Initializes a instance of TaskManager.CSystem class.
        /// </summary>
        public CSystem()
        {
        }

        /// <summary>
        /// Initializes a new instance of TaskManager.CSystem class to the value of TaskManager.CSystem object.
        /// </summary>
        /// <param name="syst"></param>
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

        /// <summary>
        /// OS name.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// OS build number.
        /// </summary>
        public string BuildNum { set; get; }

        /// <summary>
        /// OS CSD version.
        /// </summary>
        public string CSDVersion { set; get; }

        /// <summary>
        /// OS CS name.
        /// </summary>
        public string CSName { set; get; }

        /// <summary>
        /// OS install date.
        /// </summary>
        public string InstallDate { set; get; }

        /// <summary>
        /// Last OS boot up time.
        /// </summary>
        public string LastBootUpTime { set; get; }

        /// <summary>
        /// OS Architecture.
        /// </summary>
        public string OSArchitecture { set; get; }

        /// <summary>
        /// OS serial number.
        /// </summary>
        public string SerialNumber { set; get; }

        /// <summary>
        /// OS version.
        /// </summary>
        public string Version { set; get; }

        /// <summary>
        /// Processor.
        /// </summary>
        public string ProcName { set; get; }

        /// <summary>
        /// Physical memory in Mb.
        /// </summary>
        public string PhysMemory { set; get; }
    }
}
