using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Mobile.Settings
{
    public class ConfigurationHelper
    {
        #region Fields

        private XmlSerializer serializer;
        private string path;

        #endregion
        #region Properties 

        public Configuration Configuration
        {
            get
            {
                Configuration settings;

                if (!File.Exists(path))
                {
                    settings = new Configuration()
                    {
                        Host = "floading.di.unipi.it",
                        Port = 80,
                        Language = "en-US",
                        SaveOnLowBattery = true
                    };

                    using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
                        serializer.Serialize(writer, settings);

                    return settings;
                }

                using (FileStream reader = new FileStream(path, FileMode.Open))
                    settings = serializer.Deserialize(reader) as Configuration;

                return settings;
            }
            set
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.ReadWrite)))
                    serializer.Serialize(writer, value);
            }
        }

        #endregion
        #region Constructors

        public ConfigurationHelper(string path)
        {
            serializer = new XmlSerializer(typeof(Configuration));
            this.path = path;
        }

        #endregion
    }
}
