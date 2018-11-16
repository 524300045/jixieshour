using System;
using System.Collections.Generic;
using System.Windows.Forms;


/*
 * 
 * 
 */
namespace StriveEngine.BinaryDemo
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
            LogHelper.Log("1");
          //  Application.Run(new TcpClientForm());
            Application.Run(new Form1());
        }
    }
}
