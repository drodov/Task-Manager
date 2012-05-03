﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Diagnostics;
using System.ServiceProcess;

namespace TaskManager
{
    /// <summary>
    /// Provides access to some system info.
    /// </summary>
    public static class SystemInfo
    {
        /// <summary>
        /// Gets total size of physical memory.
        /// </summary>
        /// <returns>Total size of physical memory in B.</returns>
        public static long GetTotalPhysicalMemory()
        {
            long totalPhysicalMemory = 0;
            ManagementObjectSearcher man = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory"); // получаем объем физ. памяти
            foreach (ManagementObject obj in man.Get())
                totalPhysicalMemory += long.Parse(obj["Capacity"].ToString());
            return totalPhysicalMemory;
        }

        /// <summary>
        /// Gets total size of virtual memory.
        /// </summary>
        /// <returns>Total size of virtual memory in B.</returns>
        public static long GetTotalVirtualMemory()
        {
            long totalVirtualMemory = 0;
            ManagementObjectSearcher man = new ManagementObjectSearcher("SELECT TotalVirtualMemorySize FROM Win32_OperatingSystem"); // получаеем размер вирт.памяти
            foreach (ManagementObject obj in man.Get())
                totalVirtualMemory = long.Parse(obj["TotalVirtualMemorySize"].ToString());
            return totalVirtualMemory;
        }

        /// <summary>
        /// Gets available physical memory.
        /// </summary>
        /// <returns>Available physical memory in B.</returns>
        public static long GetAvailablePhysicalMemory()
        {
            PerformanceCounter pCntr = new PerformanceCounter("Memory", "Available Bytes");
            return ((long)pCntr.NextValue());
        }

        /// <summary>
        /// Gets free physical memory.
        /// </summary>
        /// <returns>Free physical memory in B.</returns>
        public static long GetFreePhysicalMemory()
        {
            long freePhysicalMemory = 0;
            ManagementObjectSearcher man = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem"); // получаеем размер свободной физ.памяти
            foreach (ManagementObject obj in man.Get())
                freePhysicalMemory = long.Parse(obj["FreePhysicalMemory"].ToString());
            return freePhysicalMemory;
        }

        /// <summary>
        /// Gets free virtual memory.
        /// </summary>
        /// <returns>Free virtual memory in B.</returns>
        public static long GetFreeVirtualMemory()
        {
            long freeVirtualMemory = 0;
            ManagementObjectSearcher man = new ManagementObjectSearcher("SELECT FreeVirtualMemory FROM Win32_OperatingSystem"); // получаеем размер свободной вирт.памяти
            foreach (ManagementObject obj in man.Get())
                freeVirtualMemory = long.Parse(obj["FreeVirtualMemory"].ToString());
            return freeVirtualMemory;
        }

        /// <summary>
        /// Gets list of processes.
        /// </summary>
        /// <returns>List of processes.</returns>
        public static List<Proc> GetProcessList()
        {
            List<Proc> procColl = new List<Proc>();
            foreach (Process proc in Process.GetProcesses())    // получаем процессы
            {
/*                long id = prevList.First(p =>
                    {
                        return (p.Id == proc.Id);
                    }).OldCpuUsage;
                procColl.Add(new Proc(proc, id));*/
                procColl.Add(new Proc(proc));
            }
            return procColl;
        }

        /// <summary>
        /// Gets list of apps.
        /// </summary>
        /// <returns>List of apps.</returns>
        public static List<CApp> GetAppList()
        {
            List<CApp> appColl = new List<CApp>();
            foreach (Process proc in Process.GetProcesses()) // выбираем процессы, у которых есть окна
            {
                if (proc.MainWindowTitle != "")
                {
                    appColl.Add(new CApp(proc));
                }
            }
            return appColl;
        }

        /// <summary>
        /// Gets list of services.
        /// </summary>
        /// <returns>List of services.</returns>
        public static List<CService> GetServiceList()
        {
            List<CService> servColl = new List<CService>();
            foreach (ServiceController srvc in ServiceController.GetServices()) // получаем список служб
            {
                CService s = new CService(srvc);
                servColl.Add(s);
            }
            return servColl;
        }

        /// <summary>
        /// Gets CPU in %.
        /// </summary>
        /// <returns>CPU in %.</returns>
        public static int GetCPU()
        {
            int CPU = 0;
            ManagementObjectSearcher man = new ManagementObjectSearcher("SELECT LoadPercentage  FROM Win32_Processor"); // CPU
            foreach (ManagementObject obj in man.Get())
                CPU =  Int32.Parse(obj["LoadPercentage"].ToString());
            return CPU;
        }

        /// <summary>
        /// Gets physical usage in %.
        /// </summary>
        /// <returns>Physical usage in %.</returns>
        public static int GetPhysicalUsage()
        {
            long _curPhysicalMemory = GetTotalPhysicalMemory() - GetAvailablePhysicalMemory();
            return (int)(100.0 * _curPhysicalMemory / GetTotalPhysicalMemory()); // процент используемой физ. памяти
        }

        /// <summary>
        /// Gets virtual memory usage in %.
        /// </summary>
        /// <returns>Virtual memory usage in %.</returns>
        public static int GetVirtualUsage()
        {
            long _curVirtualMemory = GetTotalVirtualMemory() - GetFreeVirtualMemory();
            return (int)(100.0 * _curVirtualMemory / GetTotalVirtualMemory()); // процент используемой вирт. памяти
        }

        /// <summary>
        /// Gets process owner's name.
        /// </summary>
        /// <param name="proc">Process class object for defining its owner's name.</param>
        /// <returns>Process onwer's name.</returns>
        public static string GetProcessOwnerName(Process proc)
        {
            string User = "";
            ObjectQuery sq = new ObjectQuery
                ("Select * from Win32_Process Where ProcessID = '" + proc.Id.ToString() + "'"); // получаем владельца процесса
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);
            foreach (ManagementObject oReturn in searcher.Get())
            {
                string[] o = new String[2];
                oReturn.InvokeMethod("GetOwner", (object[])o);
                User = o[0] != null ? o[0] : "";
            }
            return User;
        }
    }
}
