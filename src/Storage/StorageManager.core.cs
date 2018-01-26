using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;

namespace Storage
{
    public partial class StorageManager
    {
        private FloadingDataContext context;

        # region Environment variables

        private static Dictionary<String, String> ev;


        public static String getEnvValue(String EnvName)
        {
            String ret;
            if (ev == null) // first-time loading
                if (!rehashEnv()) return null;
            ev.TryGetValue(EnvName, out ret);
            return ret;  // KeyNotFound => ret = null
        }

        public static Boolean rehashEnv()
        {
                using (FloadingDataContext ctx = new FloadingDataContext())
                {
                    ev = ctx.EnvVariables.ToDictionary(n => n.nameVar, n => n.value);
                }
                return true;           
        }


        public static Boolean setEnvValue(String EnvName, String EnvValue)
        {
            try
            {
                if (ev == null) // first-time loading
                    if (!rehashEnv()) return false;
                using (FloadingDataContext ctx = new FloadingDataContext())
                {
                    EnvVariables evx = ctx.EnvVariables.FirstOrDefault(x => x.nameVar == EnvName);
                    if (evx == null)
                    {
                        ctx.EnvVariables.InsertOnSubmit(new EnvVariables() { nameVar = EnvName, value = EnvValue });
                        ctx.SubmitChanges();
                        ev.Add(EnvName, EnvValue);
                    }
                    else
                    {
                        evx.value = EnvValue;
                        ctx.SubmitChanges();
                        ev[EnvName] = EnvValue;
                    }
                }
            }
            catch (Exception) { return false; }
            return true;
        }

        #endregion

        public class DebugTextWriter : System.IO.TextWriter
        {
            private static UnicodeEncoding encoding;

            public override Encoding Encoding
            {
                get
                {
                    if (encoding == null)
                    {
                        encoding = new UnicodeEncoding(false, false);
                    }
                    return encoding;
                }

            }

            public override void Write(string value)
            {
                Debug.Write(value);
            }

            public override void WriteLine(string value)
            {
                Debug.WriteLine(value);
            }


        }


        public StorageManager()
        {
            context = new FloadingDataContext();
            context.Log = new DebugTextWriter(); //Comment this line if you WONT see SQL output in VS Debug Console
        }

        public Boolean commit()
        {
            try
            {
                context.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
