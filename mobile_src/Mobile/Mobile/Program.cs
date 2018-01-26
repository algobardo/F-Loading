using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Mobile.Settings;
using System.Reflection;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using Mobile.Language;
using Mobile.Communication;
using System.Xml;
using System.Xml.Serialization;

namespace Mobile
{
    static class Program
    {
        /// <summary>
        /// Il punto di ingresso principale dell'applicazione.
        /// </summary>
        /// 
        [MTAThread]
        static void Main(String[] args)
        {
            TestResources();
            ResourceManager.Instance.CallingAssembly = Assembly.GetExecutingAssembly();
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string confDir = appDir + @"\Configuration";
            string dataDir = appDir + @"\" + Mobile.Properties.Resources.FormDataSetFolder;

            DirectoryInfo confInfo = new DirectoryInfo(confDir);
            if (!confInfo.Exists)
                confInfo.Create();

            DirectoryInfo dataInfo = new DirectoryInfo(dataDir);
            if (!dataInfo.Exists)
                dataInfo.Create();

            ConfigurationHelper configuration = new ConfigurationHelper(confDir + @"\configuration.xml");
            CommunicationManager communicationManager = new CommunicationManager(configuration.Configuration.Host, configuration.Configuration.Port);

            int pid = -1;
            String requestInfo = "";
            if (args.Length > 0)
            {
                requestInfo = args[0];
                pid = (Int32)Registry.LocalMachine.OpenSubKey(@"Floading\MobileListener\ListenerPid\").GetValue("Pid");
                
                NotificationScreen notificationScreen = new NotificationScreen(configuration, requestInfo, pid, communicationManager);

                //Parse notification request
                String data = XmlConvert.DecodeName(requestInfo);
                using (StringReader reader = new StringReader(data))
                {
                    FormRequestInfo request = new XmlSerializer(typeof(FormRequestInfo)).Deserialize(reader) as FormRequestInfo;
                    if (communicationManager.GetFormInfo(request.CompilationRequestId) == null)
                    {
                        communicationManager.StoreFormInfo(new FormInfo() { RequestInfo = request, NotificationTime = request.Time, Source = request.From });
                        notificationScreen.ShowNotification(request);
                        Application.Run(notificationScreen);
                    }
                    else
                        Application.Exit();
                }
            }
            else
                Application.Run(new MainScreen(configuration, requestInfo, pid, communicationManager));
        }
        static void TestResources()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder(256);
            str.Append(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName));
            int baseLen = str.Length;

            string baseRes = str.Append(string.Format("{0}en-US{0}Mobile.resources_en.dll", Path.DirectorySeparatorChar)).ToString();

            if (File.Exists(baseRes))
            {
                string destRes = str.Remove(str.Length - 7, 7).Append(".dll").ToString();
                File.Move(baseRes, destRes);
            }

            str.Remove(baseLen, str.Length - baseLen);
            baseRes = str.Append(string.Format("{0}it-IT{0}Mobile.resources_it.dll", Path.DirectorySeparatorChar)).ToString();
            if (File.Exists(baseRes))
            {
                //string destRes = baseRes.Insert(baseRes.Length - 7, ".dll");
                string destRes = str.Remove(str.Length - 7,7).Append(".dll").ToString();
                File.Move(baseRes, destRes);
            }
        }
    }
}