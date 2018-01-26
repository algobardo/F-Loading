using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Mobile.Settings;
using System.Collections;
using Mobile.Language;

namespace Mobile.GUI
{
    /// <summary>
    /// Show the settings of the application
    /// </summary>
    public partial class SettingsScreen : LocalizedForm
    {
        #region Private

        private ConfigurationHelper helper;

        private void BackButton_Click(object sender, EventArgs e)
        {
            Hide();

            wsaTextbox.Text = helper.Configuration.Host;
            wspTextbox.Text = helper.Configuration.Port.ToString();
            detectSms.Checked = helper.Configuration.AutoDetectMessages;
            autoSaveLowCharge.Checked = helper.Configuration.SaveOnLowBattery;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            String lanFromFile= helper.Configuration.Language;
            Configuration settings = new Configuration()
            {
                Host = wsaTextbox.Text,
                Port = Int32.Parse(wspTextbox.Text),
                AutoDetectMessages = detectSms.Checked,
                SaveOnLowBattery = autoSaveLowCharge.Checked,
                Language = lanBox.SelectedItem.ToString(),
            };
            helper.Configuration = settings;

            if (ConfigurationChanged != null)
                ConfigurationChanged(this, new ConfigurationChangedEventArgs(settings));
          //  if(!lanBox.SelectedItem.ToString().Equals(lanFromFile))
            // ReloadLanguageSettings();
            
            Hide();
        }

        protected override void AddMenusToDictionary()
        {
            // this is necessary because the menu items do not
            // derive from Control and have no Name property or
            // any other way to uniquely identify them at runtime
            //
            // the fact that these string values happen to match
            // the initial display text of the menus is incidental
            // and is not necessary for the localization functionality
            // to perform as expected
            this.AddMenuToDictionary(this.SaveButton, "SaveButton");
            this.AddMenuToDictionary(this.BackButton, "BackButton");


        }

        private void ReloadLanguageSettings()
        {
            backgroundPanel.Name = ResourceManager.Instance.GetString("SettingScreen");
            this.Text = ResourceManager.Instance.GetString("SettingScreen");
            hostnameLabel.Text = ResourceManager.Instance.GetString("ws_address");
            this.PortLabel.Text = ResourceManager.Instance.GetString("ws_port");
            BackButton.Text = ResourceManager.Instance.GetString("BackButton");
            SaveButton.Text = ResourceManager.Instance.GetString("SaveButton");
            LanguageLabel.Text = ResourceManager.Instance.GetString("language");
            autosaveLabel.Text = ResourceManager.Instance.GetString("autoSave");
        }

        private void lanBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResourceManager.Instance.Culture = new System.Globalization.CultureInfo(((ComboBox)sender).SelectedItem.ToString());
        }

        #endregion
        #region Events

        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

        #endregion

        /// <summary>
        /// Show the settings of the application
        /// </summary>
        /// <param name="helper"></param>
        public SettingsScreen(ConfigurationHelper helper)
        {
            InitializeComponent();
             switch (ResourceManager.Instance.Culture.Name)
            {
                case "en-US": lanBox.SelectedIndex = 0;
                    break;
                case "it-IT": lanBox.SelectedIndex = 1;
                    break;
            }
           
            this.helper = helper;

            backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;
            backgroundPanel.SizeXMode = Mobile.Util.Controls.SizeMode.Stretch;
            backgroundPanel.VerticalAlignment = Mobile.Util.Controls.VerticalAlignment.Bottom;
          //  header.BackgroundImage = Mobile.Properties.Resources.SettingsHeader;
            infoPanel.BackgroundImage = Mobile.Properties.Resources.InformationPanel;
            infoPanel.IconImage = Mobile.Properties.Resources.SettingH;

            Configuration settings = helper.Configuration;
            wsaTextbox.Text = settings.Host;
            wspTextbox.Text = settings.Port.ToString();
            detectSms.Checked = settings.AutoDetectMessages;
            autoSaveLowCharge.Checked = settings.SaveOnLowBattery;

            IEnumerator enumerator = lanBox.Items.GetEnumerator();
            enumerator.Reset();
            int count = 0;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Equals(settings.Language))
                    lanBox.SelectedIndex = count;
                count++;
            }
           // ReloadLanguageSettings();
           this.AddMenusToDictionary();
            
            this.UpdateResources();
        }
    }

    /// <summary>
    /// Informations about the configuration changed
    /// </summary>
    public class ConfigurationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Informations about the configuration changed
        /// </summary>
        /// <param name="settings"></param>
        public ConfigurationChangedEventArgs(Configuration settings)
            : base()
        {
            this.Configuration = settings;
        }

        /// <summary>
        /// Rappresents wich setting has been changed by the user
        /// </summary>
        public Configuration Configuration
        {
            get;
            private set;
        }
    }
}

