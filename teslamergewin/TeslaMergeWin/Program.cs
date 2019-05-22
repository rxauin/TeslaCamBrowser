using System;
using System.ServiceProcess;

using System.Threading;
using System.Reflection;
using System.Configuration.Install;

namespace TeslaMergeWin
{
    class Program
    {
        static int Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                runAsService();
            }
            else
            {
                showHeader();

                TeslaMergeConfig config = TeslaMergeConfig.GetConfig();

                try
                {
                    if (args[0] == "-install")
                    {
                        installService();

                        return SUCCESS;
                    }
                    else if (args[0] == "-uninstall")
                    {
                        uninstallService();

                        return SUCCESS;
                    }
                    else if (args[0] == "-console")
                    {
                        runAsConsoleApplication(config);
                    }
                    else if (args.Length > 0)
                    {
                        showUsage();

                        return SUCCESS;
                    }

                }
                catch
                {
                    Console.WriteLine("An unrecoverable problem was encountered - see event log for details.");

                    throw;
                }
            }

            return SUCCESS;
        }


        private static void showHeader()
        {
            Console.WriteLine("\r\nTesla Merge - Combines Tesla Dashcam files into a single video with preivew and timelapse");
            Console.WriteLine("(C) 2019 https://github.com/rxauin/TeslaCamBrowser \r\n");
        }


        /// <summary>
        /// Displays usage info on the console to the 
        /// end user
        /// </summary>
        private static void showUsage()
        {
            Console.WriteLine("\r\nUsage:");
            Console.WriteLine("\r\noptional");
            Console.WriteLine("  -install   installs as an NT Service");
            Console.WriteLine("  -uninstall uninstalls as an NT Service");
            Console.WriteLine("  -console   run as a console application");
            Console.WriteLine("  -help  shows this screen");
            Console.WriteLine("  /?     shows this screen");

            Console.WriteLine("\r\nExample 1 - install the TeslaMerge as an Service");
            Console.WriteLine("TeslaMergeWin.exe -install");
        }

        /// <summary>
        /// Sets up the process to run as an
        /// NT service
        /// </summary>
        private static void runAsService()
        {
            ServiceBase[] services;
            services = new ServiceBase[] { new Service() };
            ServiceBase.Run(services);
        }

        /// <summary>
        /// Attempts to install/register the executable
        /// with the Service Control Manager (SCM).
        /// </summary>
        private static void installService()
        {
            string executable = Assembly.GetExecutingAssembly().Location;

            ManagedInstallerClass.InstallHelper(
                new string[] { executable }
                );
        }

        /// <summary>
        /// Attempts to uninstall/unregister the executable
        /// with the Service Control Manager (SCM).
        /// </summary>
        private static void uninstallService()
        {
            string executable = Assembly.GetExecutingAssembly().Location;

            ManagedInstallerClass.InstallHelper(
                new string[] { "/u", executable }
                );
        }

        /// <summary>
        /// This method drives/orchestrates the desired
        /// functionality based on the action specified
        /// from the command line.
        /// </summary>
        /// <param name="parameters"></param>
        private static void runAsConsoleApplication(TeslaMergeConfig config)
        {
            try
            {
                Console.WriteLine("Press any key to terminate...");

                MergeThread thread = new MergeThread(config);
                thread.RunOnce();

                // This will prevent the program
                // from terminating, while still 
                // allowing this primary thread to
                // be responsive and display console
                // i/o generated from the trace
                bool done = false;
                do
                {
                    done = Console.KeyAvailable;
                    Thread.Sleep(200);
                } while (done == false);

                Console.WriteLine("Stopping background worker threads...");

                // Stop Threads
                thread.Stop();


                Console.WriteLine("Finished.");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }


        const int FAILED = -1;
        const int NO_CMD_LINE = 0;
        const int SUCCESS = 1;
    }
}
