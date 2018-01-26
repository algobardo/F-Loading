using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mobile.Communication;
using Mobile.Util.Controls;
using Mobile.Util;
using Mobile.Language;
using Mobile.Settings;

namespace Mobile
{
    /// <summary>
    /// Manage the window to show the new inbox form
    /// </summary>
    public partial class NotificationScreen : LocalizedForm
    {
        /// <summary>
        ///  Event that manage the notification received
        /// </summary>
        public event EventHandler<NotificationEventArgs> NotificationApproved;
        #region Protected
        /// <summary>
        ///  Add Translation to Buttons control
        /// </summary>
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
            this.AddMenuToDictionary(this.fillButton, "fillButton");
            this.AddMenuToDictionary(this.ignoreButton, "ignoreButton");


        }
        #endregion
        #region Private

        private CommunicationManager communicationManager;
        private FormRequestInfo formInfo;
        private ConfigurationHelper helper;
        private int pid;
        private String requestInfo;

        /// <summary>
        /// Manage the Fill button when the user clicks on it
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnFillButtonClick(object sender, EventArgs e)
        {
            if (helper != null)
            {
                FormInfo info = new FormInfo()
                {
                    Status = FormStatus.Notified,
                    RequestInfo = formInfo,
                    Source = formInfo.From,
                    NotificationTime = formInfo.Time
                };

                MainScreen screen = new MainScreen(helper, requestInfo, pid, communicationManager);
                screen.Show();
                screen.OnNotificationApproved(this, new NotificationEventArgs(FormAction.Fill, formInfo));
                this.Hide();
            }
            else
            {
                if (NotificationApproved != null)
                    NotificationApproved(this, new NotificationEventArgs(FormAction.Fill, formInfo));
            }
        }
        /// <summary>
        /// Manages the event raised when the user clicks on ignore button.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnIgnoreButtonClick(object sender, EventArgs e)
        {
            if (helper != null)
                Application.Exit();
            else
            {
                if (NotificationApproved != null)
                    NotificationApproved(this, new NotificationEventArgs(FormAction.Ignore, formInfo));
            }
        }

        #endregion

        /// <summary>
        /// The Enumeration that manage its possible action  
        /// </summary>
        public enum FormAction { Ignore, Fill }

        /// <summary>
        /// The arguments of the event raised when the mobile
        /// received a notification
        /// </summary>
        public class NotificationEventArgs : EventArgs
        {
           
            public NotificationEventArgs(FormAction action, FormRequestInfo info)
                : base()
            {
                this.Action = action;
                this.Info = info;
            }

            /// <summary>
            /// The <see cref="FormAction"/> that represents the Action 
            /// </summary>
            public FormAction Action
            {
                get;
                private set;
            }

            /// <summary>
            /// The <see cref="FormInfo"/> that represents the Info
            /// </summary>
            public FormRequestInfo Info
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// The Constructor of the class
        /// </summary>
        public NotificationScreen(ConfigurationHelper configuration, string requestInfo, int pid, CommunicationManager communicationManager)
        {
            InitializeComponent(); 
            this.communicationManager = communicationManager;

            this.helper = configuration;
            this.requestInfo = requestInfo;
            this.pid = pid;

            backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;
            backgroundPanel.SizeXMode = SizeMode.Stretch;
            backgroundPanel.VerticalAlignment = VerticalAlignment.Bottom;
            //header.BackgroundImage = Mobile.Properties.Resources.NotificationHeader;
            contentPanel.BackgroundImage = Mobile.Properties.Resources.InformationPanel;
            contentPanel.IconImage = Mobile.Properties.Resources.NotificationHeader;
            this.AddMenusToDictionary();
            this.UpdateResources();
        }

        /// <summary>
        /// Show the notification on the screen
        /// </summary>
        /// <param name="info">the <see cref="FormInfo"/> object that contein information about the form</param>
        public void ShowNotification(FormRequestInfo info)
        {
            this.formInfo = info;

            name.Text = info.CompilationRequestId.ToString();
            notified.Text = info.Time.ToString(ResourceManager.Instance.Culture);
            from.Text = info.From;
            fromPanel.Height = Math.Max(StringMeasure.Measure(from, info.From == null? "" : info.From, from.Bounds).Height, fromLabel.Height);

            if (info.Username == null || info.Username == "")
            {
                type.Text = "Public";
                usernamePanel.Hide();
                servicePanel.Hide();
                tokenPanel.Hide();
                contentPanel.Height = 146;
            }
            else
            {
                type.Text = "Private";
                username.Text = info.Username;
                usernamePanel.Show();
                service.Text = info.Service;
                servicePanel.Show();
                token.Text = info.Token;
                tokenPanel.Show();
                contentPanel.Height = 212;
            }
        }
    }
}