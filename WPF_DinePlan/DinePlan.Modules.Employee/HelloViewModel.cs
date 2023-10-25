using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinePlan.Modules.BioMetricModule.Interfaces;
using DinePlan.Modules.BioMetricModule.Persona;
using DinePlan.Modules.Employee.Persona;
using DinePlan.Services;
using DinePlan.Services.Common.Device;

namespace DinePlan.Modules.BioMetricModule
{
   public class HelloViewModel
   {
       private IFingerPrintScannerAdapter adapter;
       public HelloViewModel(IDeviceService deviceService)
       {
           
       }
    }
}
