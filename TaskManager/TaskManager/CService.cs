using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Management;

namespace TaskManager
{
    /// <summary>
    /// Represents services.
    /// </summary>
    public class CService
    {
        /// <summary>
        /// Initializes a new instance of TaskManager.CService class to the value of System.ServiceProcess.ServiceController object.
        /// </summary>
        /// <param name="Srvc">Object of System.ServiceProcess.ServiceController class of witch base TaskManager.CService object is created</param>
        public CService(ServiceController Srvc)
        {
            Name = Srvc.ServiceName;
            Description = Srvc.DisplayName;
            Status = Srvc.Status;
            ManagementObject MO = new ManagementObject(@"Win32_service.Name='" + Srvc.ServiceName + "'");
            Id = Int32.Parse(MO.GetPropertyValue("ProcessID").ToString());
        }
        /// <summary>
        /// Gets name of the associated service.
        /// </summary>
        public string Name{set; get;}
        /// <summary>
        /// Gets process's Id using the associated service.
        /// </summary>
        public int Id{set; get;}
        /// <summary>
        /// Gets descriprion of the associated service.
        /// </summary>
        public string Description{set; get;}
        /// <summary>
        /// Gets status of the associated service.
        /// </summary>
        public ServiceControllerStatus Status{set; get;}
    }
}
