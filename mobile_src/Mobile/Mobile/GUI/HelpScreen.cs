using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mobile.Util.Controls;

namespace Mobile
{
    /// <summary>
    /// The form that contains informations about the usage of the application
    /// </summary>
    public partial class HelpScreen : Form
    {
        #region Private

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        #endregion

        /// <summary>
        /// The form that contains informations about the usage of the application
        /// </summary>
        public HelpScreen()
        {
            InitializeComponent();
            backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;
            backgroundPanel.SizeXMode = SizeMode.Stretch;
            backgroundPanel.VerticalAlignment = VerticalAlignment.Bottom;
            infoPanel.BackgroundImage = Mobile.Properties.Resources.InformationPanel;
            header.BackgroundImage = Mobile.Properties.Resources.HelpHeader;
        }
    }
}