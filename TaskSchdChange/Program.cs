using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;

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
