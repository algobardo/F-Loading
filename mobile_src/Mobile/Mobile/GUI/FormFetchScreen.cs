using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;
using Mobile.Communication;
using Mobile.Language;

namespace Mobile
{
    /// <summary>
    /// The form used for fetching forms
    /// </summary>
    public partial class FormFetchScreen : LocalizedForm
    {
        private const int BACK = 0;
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<FormAccessEventArgs> DataSubmitted;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<CancelConnectionEventArgs> CancelConnection;

        #endregion
        #region Protected
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
            this.AddMenuToDictionary(this.backButton, "backButton");
            this.AddMenuToDictionary(this.getFormButton, "getFormButton");


        }
        #endregion
        #region Private

        private bool isPublic;
        private bool connecting;

        private void OnBackButtonClick(object sender, EventArgs e)
        {
            if (BACK == 0 && connecting)
            {
                try
                {
                    CancelConnection(this, new CancelConnectionEventArgs(Int32.Parse(id.Text)));
                }
                catch (Exception)
                {
                }
                connecting = false;
            }
            else
            {
                id.Text = "";
                username.Text = "";
                token.Text = "";
                service.SelectedIndex = 0;

                this.Hide();
            }
        }

        private void OnGetFormButtonClick(object sender, EventArgs e)
        {
            if (CheckData())
            {
                FormRequestInfo info = null;
                if (!Public)
                {
                    info = new FormRequestInfo()
                    {
                        CompilationRequestId = Int32.Parse(id.Text),
                        Username = username.Text,
                        Service = service.SelectedItem.ToString(),
                        Token = token.Text
                    };
                }
                else
                {
                    info = new FormRequestInfo()
                    {
                        CompilationRequestId = Int32.Parse(id.Text),
                        Username = null,
                        Service = null,
                        Token = null
                    };
                }
                if (DataSubmitted != null)
                {
                    connecting = true;
                    DataSubmitted(this, new FormAccessEventArgs(info));
                }
            }
        }

        private bool CheckData()
        {
            try
            {
                int idInt = Int32.Parse(id.Text);
            }
            catch (FormatException)
            {
                NotificationHelper.ShowNotification(ExceptionManager.Error, ExceptionManager.CompilationRequestIdInteger);
                return false;
            }
            if (!Public)
            {
                if (username.Text == "" || token.Text == "")
                {
                    NotificationHelper.ShowNotification(ExceptionManager.Error, ExceptionManager.InformationSpecified);
                    return false;
                }
            }
            return true;
        }

        private void UpdateLoginMode()
        {
            if (Public)
            {
                idPanel.Show();
                usernamePanel.Hide();
                tokenPanel.Hide();
                servicePanel.Hide();
                contentPanel.Height -= 81;
            }
            else
            {
                idPanel.Show();
                usernamePanel.Show();
                tokenPanel.Show();
                servicePanel.Show();

                contentPanel.Height += 81;
            }
        }

        #endregion

        /// <summary>
        /// Contains informations about the form requested
        /// </summary>
        public class FormAccessEventArgs : EventArgs
        {
            /// <summary>
            /// This class contains informations about the form requested
            /// </summary>
            /// <param name="info"></param>
            public FormAccessEventArgs(FormRequestInfo info)
                : base()
            {
                this.RequestInfo = info;
            }

            /// <summary>
            /// The informations of the form requested
            /// </summary>
            public FormRequestInfo RequestInfo
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Contains informations about the connection that has been canceled
        /// </summary>
        public class CancelConnectionEventArgs : EventArgs
        {
            /// <summary>
            /// The ID of the form has been partially dowloaded to delete directories and datas
            /// </summary>
            public int ID
            {
                get;
                private set;
            }

            /// <summary>
            /// Contains informations about the connection that has been canceled
            /// </summary>
            /// <param name="id"></param>
            public CancelConnectionEventArgs(int id)
                : base()
            {
                this.ID = id;
            }
        }

        /// <summary>
        /// True if the request is for a public form instead of a private one
        /// </summary>
        public bool Public
        {
            get
            {
                return isPublic;
            }
            set
            {
                if (isPublic != value)
                {
                    this.isPublic = value; 
                    UpdateLoginMode();
                }
            }
        }

        /// <summary>
        /// The form used for fetching forms
        /// </summary>
        public FormFetchScreen()
        {
            InitializeComponent();
            
            backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;
            backgroundPanel.SizeXMode = Mobile.Util.Controls.SizeMode.Stretch;
            backgroundPanel.VerticalAlignment = Mobile.Util.Controls.VerticalAlignment.Bottom;
            contentPanel.BackgroundImage = Mobile.Properties.Resources.InformationPanel;
            contentPanel.IconImage = Mobile.Properties.Resources.LoginH;

            service.SelectedIndex = 0;
            isPublic = false;
            connecting = false;
            this.AddMenusToDictionary();
            this.UpdateResources();
        }
    }
}