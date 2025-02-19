using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WinePOSReportService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

//#if DEBUG
//            Console.WriteLine("Debug mode - Running as a console app.");
//            Service1 service = new Service1();
//            service.OnStart(null);  // Simulate service start
//            Console.WriteLine("Press Enter to execute task...");
//            Console.ReadLine();
//            service.RunTask();  // Run the task manually
//            Console.WriteLine("Task executed. Press Enter to stop.");
//            Console.ReadLine();
//            service.OnStop();  // Simulate service stop
//#else
//                        ServiceBase.Run(new Service1());  // Runs as a Windows Service
//#endif
        }
    }
}
