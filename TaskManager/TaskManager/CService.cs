using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Management;

namespace TaskManager
{
    class CService
    {
        public CService(ServiceController Srvc)
        {
            Name = Srvc.ServiceName;
            Description = Srvc.DisplayName;
            Status = Srvc.Status;
            ManagementObject MO = new ManagementObject(@"Win32_service.Name='" + Srvc.ServiceName + "'");
            Id = Int32.Parse(MO.GetPropertyValue("ProcessID").ToString());
        }
        public string Name{set; get;}
        public int Id{set; get;}
        public string Description{set; get;}
        public ServiceControllerStatus Status{set; get;}
    }
}
