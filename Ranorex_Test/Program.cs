using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Ranorex;
using Ranorex.Core.Repository;
using Ranorex.Core;
using Ranorex.Core.Testing;
using System.Reflection;
//using Microsoft.Extensions.Logging;
using NLog;  //This is for logging using Ranorex API, also added Ranorex.Libs for this
//comment

namespace Ranorex_Test
{
    internal class Program
    {
        //private const string RxDemoAppFilePath = "E:\\\\applications\\RxDemoApp.exe";                
        
        private const string RelativePathToWelcomeMessage = "./?/?/tabpage[@controlname='RxTabIntroduction']/text[@controlname='lblWelcomeMessage']";

        [STAThread]
        static void Main(string[] args)
        {
            InitResolver();
            RanorexInit();
            Run();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitResolver()
        {
            Ranorex.Core.Resolver.AssemblyLoader.Initialize();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RanorexInit()
        {
            TestingBootstrapper.SetupCore();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Run()
        {
            test();  //This is for logging using Ranorex API, also added Ranorex.Libs for this

            Assembly asm = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(asm.Location);

            string RxDemoAppFilePath = path + "\\RxDemoApp.exe";

            Host.Local.RunApplication(RxDemoAppFilePath); // Runs the Ranorex Demo Application
            var timeoutToWaitForAppToBeStarted = Duration.FromMilliseconds(10000);

            Ranorex.Form frm = null;

            bool result = Host.Local.TryFindSingle<Form>("/form[@processname='RxDemoApp']", 2000, out frm);
            Report.Log(ReportLevel.Info, result.ToString());
            Console.WriteLine(result.ToString());

            //var demoApp = Host.Local.FindSingle<Form>("/form[@processname='RxDemoApp']", timeoutToWaitForAppToBeStarted);
           var demoApp = Host.Local.FindSingle<Form>("/form[@automationid='RxMainFrame']", 10000);

            var userName = demoApp.FindSingle<Text>("./?/?/tabpage[@controlname='RxTabIntroduction']/text[@controlname='txtUserName']");
            userName.Click(); // Clicks the name input field
            userName.PressKeys("Harry"); // type Harry into the input field

            var submitBtn = demoApp.FindSingle<Button>("./?/?/tabpage[@controlname='RxTabIntroduction']/button[@controlname='btnSubmitUserName']");
            submitBtn.Click(); // Clicks the submit button

            var welcomeMsg = demoApp.FindSingle(RelativePathToWelcomeMessage); // you can also put RanoreXPaths into constants or resources
            Validate.AttributeEqual(welcomeMsg, "Text", "Welcome, Harry!"); // Verify that the welcome message changes accordingly
            var resetMsg = demoApp.FindSingle<Link>("./?/?/tabpage[@controlname='RxTabIntroduction']/link[@controlname='RxLinkBtnReset']/?/?/link[@accessiblename='Reset']");
            resetMsg.Click(); // Reset the welcome message
            demoApp.Close(); //Close the application
            
        }

        //This is for logging using Ranorex API, also added Ranorex.Libs for this
        public static void test()
        {

            //FileTarget filename = NLog.
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config
            NLog.LogManager.Configuration = config;
        }


    }
}
