using System;
using System.Collections.Generic;
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
            Application.Run(new Form1());
        }
    }
}
