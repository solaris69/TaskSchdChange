﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;


namespace TaskSchdChange
{
    class Program
    {
      static string schemaJson = @"{
       '$schema': 'http://json-schema.org/draft-04/schema#',
       'description': 'Puppet Task Scheduler resource set',
       'type': 'array',
       'items': {
          'title' : 'Puppet Task Scheduler resource',
          'type'  :  'object',
          'properties' : {
              'puppet_jobname': {
                  'description': 'job name for puppet resource',
                  'type':'string',
                  'uniqueItems': true
              },
              'name': {
                  'description': 'job name for puppet resource',
                  'type':'string',
                  'uniqueItems': true
              },
              'file': {
                  'description': 'job name for puppet resource',
                  'type':'string'
              },
              'interval': {
                  'description': 'job name for puppet resource',
                  'type':'number',
                  'minimum': 1,
                  'exclusiveMinimum': true
              },
              'enabled': {
                  'description': 'job name for puppet resource',
                  'type':'boolean'
              }
          },
          'required': ['puppet_jobname','name','file','interval','enabled']
        }
      }";

      static void Main(string[] args)
      {
#if DEBUG
          args = new[] { "test1", "5", "1" };
//          args = new[] { "GoogleUpdateTaskMachineCore", "5", "1" };
#endif
        var appSettings = ConfigurationManager.AppSettings;
        if (Convert.ToBoolean(appSettings["switchPerTaskname"]))
        {
          /*INSTALL via InstallShield -> setup /V "/v/qn INSTALLDIR=C:\_vc\tasks /L*V c:\_vc\vc.log"
           * Must be run as Administrator dos command box. setup_0.1.2.exe /S /V "/v/qn INSTALLDIR=C:\_vc\tasks"
           */
          try
          {
            /*
            using (JsonReader sreader = new JsonTextReader(schemaJson))
            {
            }
            */
            // http://www.newtonsoft.com/jsonschema/help/html/CreateJsonSchemaManually.htm
            JSchema schema;
            Type type = Type.GetType("TaskSchdOBJ");
            Newtonsoft.Json.Schema.JsonSchemaGenerator jsonSchemaGenerator = new Newtonsoft.Json.Schema.JsonSchemaGenerator();
            jsonSchemaGenerator.UndefinedSchemaIdHandling = Newtonsoft.Json.Schema.UndefinedSchemaIdHandling.UseTypeName;
            Newtonsoft.Json.Schema.JsonSchema jsonSchema = jsonSchemaGenerator.Generate(type);
            using (StreamWriter wr = new StreamWriter(@"T:\_GitCodingSchemaA.json"))
            {
              using (JsonWriter writer = new JsonTextWriter(wr))
              {
                writer.Formatting = Formatting.Indented;

                jsonSchema.WriteTo(writer);
              }
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }

          using (var md5 = MD5.Create())
          {
            // Get config file

            if (!(File.Exists(appSettings["schdFileConfig"])))
            {
              Environment.Exit(2);
            }
            /* USE MD5 check diff file
             * - tasklist.conf contains data as puppet create
             * - tasklist.conf.md5 is check by this.*/
            bool diff = false;
            using (var stream = File.OpenRead(appSettings["schdFileConfig"]))
            {
              byte[] diff_file = md5.ComputeHash(stream);
            }
            if (diff)
            {
              using (StreamReader sr = new StreamReader(appSettings["schdFileConfig"]))
              {

              }

            }
          }
          using (TaskService tsMgr = new TaskService())
          {
            //Usage: [task scheduler name] [interval time to run task in MINUTES] [extend time to stop task in MINUTES]
            Task ts = tsMgr.GetTask(args[0]);
            //if (ts != null)
          }
        }
        else
        {
          //if (args.Length != 3)
          //{
          //  Console.WriteLine("Usage: [task scheduler name] [interval time to run task in MINUTES] [extend time to stop task in MINUTES]");
          //  Environment.Exit(1);
          //}
          //  
          using (TaskService tsMgr = new TaskService())
          {
            Task ts = tsMgr.GetTask(args[0]);
            if (ts != null)
            {
              switch (args[1])
              {
                case "stop_over":
                  {
                    ts.Definition.Settings.ExecutionTimeLimit = TimeSpan.FromMinutes(Convert.ToDouble(args[2]));
                    try
                    {
                      ts.RegisterChanges();
                    }
                    catch (Exception ex)
                    {
                      Console.WriteLine(ex.Message);
                    }
                    break;
                  }
                case "check_interval":
                  {
                    int result = 1;
                    if (ts.Definition.Triggers.Count > 0 && ts.Definition.Triggers[0] is DailyTrigger) // TimeTrigger is default but our pentaho only use DailyTrigger
                    {
                      result = TimeSpan.Compare(ts.Definition.Triggers[0].Repetition.Interval, TimeSpan.FromMinutes(Convert.ToDouble(args[2])));
                      if (result == 0)
                      {
                        Console.WriteLine("Got same interval {0} == {1}", ts.Definition.Triggers[0].Repetition.Interval, TimeSpan.FromMinutes(Convert.ToDouble(args[2])));
                        Environment.Exit(1);
                      }
                    }
                    else
                    {
                      Console.WriteLine("Count {0}", ts.Definition.Triggers.Count);
                      Console.WriteLine("Trigger??? {0}",
                        (ts.Definition.Triggers[0] is TimeTrigger) ? "YES" : String.Format("NO. it is {0}", ts.Definition.Triggers[0].TriggerType.ToString()));
                      Environment.Exit(1); // Force Puppet to not run by onlyif
                    }
                    break;
                  }
                case "check_stop":
                  {
                    int result = TimeSpan.Compare(ts.Definition.Settings.ExecutionTimeLimit,TimeSpan.FromMinutes(Convert.ToDouble(args[2])));
                    if (result == 0)
                    {
                      Console.WriteLine("Got same stop_over {0}", ts.Definition.Settings.ExecutionTimeLimit);
                      Environment.Exit(1);
                    }
                    Console.WriteLine("Got diff stop_over origin[{0}] {1} new[{2}]\n", ts.Definition.Settings.ExecutionTimeLimit, result == 1 ? ">" : "<", TimeSpan.FromMinutes(Convert.ToDouble(args[2])));
                    Environment.Exit(0);
                    break;
                  }
                default:
                  {
                    Console.WriteLine("Usage: [task scheduler name] [option] ");
                    Environment.Exit(2);
                    break;
                  }
              }
              //ts.Definition.Settings.DeleteExpiredTaskAfter = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              //ts.Definition.Triggers[0].ExecutionTimeLimit = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              //ts.Definition.Settings.ExecutionTimeLimit = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              //try
              //{
              //  ts.RegisterChanges();
              //}
              //catch (Exception ex)
              //{
              //  Console.WriteLine(ex.Message);
              //}
              /*TaskDefinition td = ts.Definition;
              td.Settings.DeleteExpiredTaskAfter = TimeSpan.FromMinutes(Convert.ToDouble(args[2]));
              //td.Triggers[0].ExecutionTimeLimit = TimeSpan.FromMinutes(Convert.ToDouble(args[1]) + Convert.ToDouble(args[2]));
              tsMgr.RootFolder.RegisterTaskDefinition(args[0], td);*/
            }
            else
            {
              Console.WriteLine("Not found task name: {0}",args[0]);
            }
          }
        }
      }
    }
}
