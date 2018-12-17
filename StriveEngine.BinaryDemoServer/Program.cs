using Atms.Common;
using PdaTester;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;


namespace StriveEngine.BinaryDemoServer
{
    static class Program
    {
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            log4net.Config.XmlConfigurator.Configure();
            LogHelper.Log("程序启动");
           // string str = "OK7,1A_STATION=SENTRY-T2-2";
            //string fct1ACode = str.Substring(str.IndexOf("=") + 1);
           
            Application.ThreadException+=Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException+=CurrentDomain_UnhandledException;
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog()==DialogResult.OK)
            {
                RobotApp.Start();
                Application.Run(new Form2());
            }
          
          //Application.Run(new MesForm());
        }

        static void Application_ThreadException(object sender,ThreadExceptionEventArgs e)
        { 
             LogHelper.Log(e.Exception.Message);
        }

        static void CurrentDomain_UnhandledException(object sender,UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                LogHelper.Log("UnhandledException:" + ex.Message);
            }
            catch (Exception ex)
            {

                LogHelper.Log("CurrentDomain_UnhandledException"+ex.Message);
            }
        }
    }
}
