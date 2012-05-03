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
    public class Proc
    {
        /// <summary>
        /// Initializes a new instance of TaskManager.Proc class.
        /// </summary>
        public Proc()
        {
            Id = 0;
            Name = "";
            ThreadsCount = 0;
            Priority = 0;
            Description = "";
            Status = false;
/*            OldCpuUsage = 0;
            CpuUsage = 0;*/
        }
        /// <summary>
        /// Initializes a new instance of TaskManager.Proc class to the value of System.Diagnostics.Process object.
        /// </summary>
        /// <param name="proc">Object of System.Diagnostics.Process class of which base TaskManager.Proc object is created.</param>
        public Proc(Process proc)
        {
            try
            {
                Id = proc.Id;
                Name = proc.ProcessName;
                ThreadsCount = proc.Threads.Count;
                Priority = proc.BasePriority;
                Description = proc.MainModule.FileVersionInfo.FileDescription;
                Status = proc.Responding;
 /*               OldCpuUsage = 0;
                CpuUsage = 0;*/
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
        /// Gets description of the associated process.
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// Gets the responding status for the associated Proc.
        /// </summary>
        public bool Status { set; get; }
/*
        public long OldCpuUsage { set; get; }

        public long CpuUsage { set; get; }*/

        /// <summary>
        /// Initializes a instance of TaskManager.Proc class to the value of System.Diagnostics.Process object.
        /// </summary>
        /// <param name="proc">Object of System.Diagnostics.Process class for initialization.</param>
        public void CopyFromProcess(Process proc)
        {
            try
            {
                Id = proc.Id;
                Name = proc.ProcessName;
                ThreadsCount = proc.Threads.Count;
                Priority = proc.BasePriority;
                Description = proc.MainModule.FileVersionInfo.FileDescription;
                Status = proc.Responding;
            }
            catch (Exception)
            {
                Description = "";
            }
        }
    }
}
