using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace YOUTRACK_NOTICE
{
    static class Program
    {
     
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new YouTrack()
            };
            ServiceBase.Run(ServicesToRun);
        }
        //        static void Main()
        //        {
        //#if DEBUG
        //            //While debugging this section is used.
        //            YouTrack myService = new YouTrack();
        //            myService.onDebug();
        //            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

//#else
        //In Release this section is used. This is the "normal" way.
        //ServiceBase[] ServicesToRun;
        //ServicesToRun = new ServiceBase[] 
        //{ 
        //    new Service1() 
        //};
        //ServiceBase.Run(ServicesToRun);
//#endif
    //}
    }
}
