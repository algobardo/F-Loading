using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace MobileListener
{
    class Program
    {
        private static NotificationSender sender;

        static void Main(string[] args)
        {
            //Insert the process id in the registry
            RegistryKey processIdKey = Registry.LocalMachine.CreateSubKey(@"Floading\MobileListener\ListenerPid\");
            processIdKey.SetValue("Pid", Process.GetCurrentProcess().Id);

            sender = new NotificationSender() { DetectEmail = true, DetectSms = true };
            while (true)
            {
                Thread.Sleep(1000);
                Application.DoEvents();
            }
        }
    }
}
