using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;
using System.Security.Cryptography;
using System.IO;

namespace TaskSchdChange
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
          args = new[] { "test1", "5", "1" };
//          args = new[] { "GoogleUpdateTaskMachineCore", "5", "1" };
#endif
          if (args.Length != 3) {
            Console.WriteLine("Usage: [task scheduler name] [interval time to run task in MINUTES] [extend time to stop task in MINUTES]");
            Environment.Exit(1);
          }

          /*INSTALL via InstallShield -> setup /V "/v/qn INSTALLDIR=C:\_vc\tasks /L*V c:\_vc\vc.log"
           * Must be run as Administrator dos command box. setup_0.1.2.exe /S /V "/v/qn INSTALLDIR=C:\_vc\tasks"
           */

          using (var md5 = MD5.Create())
          {
            /* USE MD5 check diff file
             * - tasklist.conf contains data as puppet create
             * - tasklist.conf.md5 is check by this.
            using (var stream = File.OpenRead(filename))
            {
              return md5.ComputeHash(stream);
            }
             */ 
          }

          using (TaskService tsMgr = new TaskService() )
          {
            Task ts = tsMgr.GetTask(args[0]);
            if (ts != null)
            {
              //ts.Definition.Settings.DeleteExpiredTaskAfter = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              //ts.Definition.Triggers[0].ExecutionTimeLimit = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              ts.Definition.Settings.ExecutionTimeLimit = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              try
              {
                ts.RegisterChanges();
              }
              catch (Exception ex)
              {
                Console.WriteLine(ex.Message);
              }
              /*TaskDefinition td = ts.Definition;
              td.Settings.DeleteExpiredTaskAfter = TimeSpan.FromMinutes(Convert.ToDouble(args[2]));
              //td.Triggers[0].ExecutionTimeLimit = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              tsMgr.RootFolder.RegisterTaskDefinition(args[0], td);*/
            }
          }
        }
    }
}
