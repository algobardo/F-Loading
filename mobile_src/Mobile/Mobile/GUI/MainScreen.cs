using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mobile.Communication;
using Mobile.Fields;
using Mobile.Workflow;
using Microsoft.WindowsCE.Forms;
using Microsoft.WindowsMobile.Status;
using Microsoft.Win32;
using Mobile.Settings;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.WindowsMobile.PocketOutlook;
using Mobile.GUI;
using Mobile.Language;

namespace Mobile
{
    /// <summary>
    /// The Main form of the application
    /// </summary>
    public partial class MainScreen : LocalizedForm
    {
        #region Proteced
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
            this.AddMenuToDictionary(this.exitButton, "exitButton");

        }
        #endregion
        #region Private

        //Sub-screens
        private SettingsScreen settingsScreen;
        private HelpScreen helpScreen;
        private RecentScreen recentScreen;
        private NotificationScreen notificationScreen;
        private FormFetchScreen loginScreen;
        private FormScreen formScreen;

        private CommunicationManager communicationManager;
        private ConfigurationHelper configuration;
        private NotificationReceiver notificationReceiver;

        private SystemState orientationWatcher;
        private int currentFormId;

        private int daemonPid;
        private bool daemonStartup;

        private void StartForm(FormFetchEventArgs e)
        {
            currentFormId = e.Info.RequestInfo.CompilationRequestId;

            FieldFactory.Types = e.Types;
            WorkflowManager wm = null;
            try
            {
                wm = new WorkflowManager(e.Info, e.WorkflowNodes, e.WorkflowEdges, e.Types, e.Presentation, e.Data);

                formScreen = new FormScreen(wm, configuration.Configuration.SaveOnLowBattery);
                formScreen.FillingFinished += OnFormFillingFinished;
                formScreen.BatteryLevelNotification += OnBatteryLevelNotification;
                formScreen.Closed += OnFormScreenClosed;

                if (recentScreen != null)
                    recentScreen.Hide();
                if (loginScreen != null)
                    loginScreen.Hide();

                formScreen.Text = e.Info.Name;
                //formScreen.ChangeOrientation(SystemSettings.ScreenOrientation);
                formScreen.Show();
            }
            catch (WorkflowException w)
            {
                NotificationHelper.ShowNotification(ExceptionManager.Error, w.Message);
            }
        }

        private void ChangeToLandscape(int angle)
        {
            switch (angle)
            {
                case 0:
                    SystemSettings.ScreenOrientation = ScreenOrientation.Angle0;
                    break;
                case 90:
                    SystemSettings.ScreenOrientation = ScreenOrientation.Angle90;
                    break;
                case 180:
                    SystemSettings.ScreenOrientation = ScreenOrientation.Angle180;
                    break;
                case 270:
                    SystemSettings.ScreenOrientation = ScreenOrientation.Angle270;
                    break;
            }
            if (formScreen != null && !formScreen.IsDisposed)
                formScreen.ChangeOrientation(SystemSettings.ScreenOrientation);
            backgroundPanel.Height = Screen.PrimaryScreen.WorkingArea.Height - 26;
            backgroundPanel.Width = Screen.PrimaryScreen.WorkingArea.Width;
        }

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private bool UpdateRecentForms()
        {
            FormInfo[] localForms = communicationManager.ListLocalForms();
            if (localForms.Length == 0)
                warningButton.Visible = false;
            else
                warningButton.Visible = true;

            recentScreen.ShowRecentForms(localForms);

            return localForms.Length > 0;
        }

        private void OnFillingCanceled(object sender, EventArgs args)
        {
            communicationManager.AbortFormFetcher();

            if (loginScreen != null)
            {
                loginScreen.DataSubmitted += OnLoginDataSubmitted;
                loginScreen.CancelConnection += OnConnectionCanceled;
                loginScreen.Close();
            }
        }

        private void OnConnectionCanceled(object sender, FormFetchScreen.CancelConnectionEventArgs args)
        {
            communicationManager.RemoveForm(args.ID);
            OnFillingCanceled(sender, EventArgs.Empty);
        }

        private void OnOrientationWatcherChanged(object sender, ChangeEventArgs args)
        {
            int newOrientation = (int)args.NewValue;
            ChangeToLandscape(newOrientation);
        }

        private void OnFormScreenClosed(object sender, EventArgs e)
        {
            formScreen.Dispose();
        }

        private void OnConfigurationChanged(object sender, ConfigurationChangedEventArgs args)
        {
            communicationManager.UpdateService(args.Configuration.Host, args.Configuration.Port);
            ResourceManager.Instance.Culture = new System.Globalization.CultureInfo(configuration.Configuration.Language);
            this.UpdateResources();
            //Kill daemon and remove from startup
            if (!args.Configuration.AutoDetectMessages)
            {
                RegistryKey key = Registry.LocalMachine.CreateSubKey("init");
                if (key.GetValue("Launch99") != null)
                    key.DeleteValue("Launch99");
                key.Close();

                if (daemonPid == -1)
                {
                    try
                    {
                        daemonPid = (int)Registry.LocalMachine.OpenSubKey(@"Floading\MobileListener\ListenerPid\").GetValue("Pid");
                    }
                    catch (Exception)
                    {
                    }
                }

                if (daemonPid != -1)
                {
                    //Kill the process, and store the "no-process" identificator in the registry
                    Process.GetProcessById(daemonPid).Kill();
                    daemonPid = -1;
                    Registry.LocalMachine.CreateSubKey(@"Floading\MobileListener\ListenerPid\").SetValue("Pid", -1);
                }

                checkNewButton.Visible = false;
            }
            else
            {
                if (daemonPid == -1)
                {
                    RegistryKey key = Registry.LocalMachine.CreateSubKey("init");
                    key.SetValue("Launch99", Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\" + Mobile.Properties.Resources.MobileListenerExecutable);
                    key.Close();

                    String mobilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                    String daemonAppPath = mobilePath + @"\\" + Mobile.Properties.Resources.MobileListenerExecutable;
                    Process proc = Process.Start(new ProcessStartInfo(daemonAppPath, ""));
                    daemonPid = proc.Id;
                }

                checkNewButton.Visible = true;
            }

            if (settingsScreen != null)
            {
                settingsScreen.ConfigurationChanged -= OnConfigurationChanged;
                settingsScreen.Close();
            }
        }

        private void OnLoginDataSubmitted(object sender, FormFetchScreen.FormAccessEventArgs e)
        {
            FormInfo info = new FormInfo()
            {
                Status = FormStatus.Notified,
                RequestInfo = e.RequestInfo,
                NotificationTime = DateTime.Now
            };

            Cursor.Current = Cursors.WaitCursor;

            communicationManager.StoreFormInfo(info);
            UpdateRecentForms();
            communicationManager.GetForm(e.RequestInfo);
        }

        private void OnDataSetFetched(object sender, FormFetchEventArgs e)
        {
            Invoke(new Action(delegate()
            {
                Cursor.Current = Cursors.Default;
                if (formScreen != null)
                    formScreen.EnableButtons(true);

                if (e.Result == CommunicationResult.Success)
                {
                    if (UpdateRecentForms())
                        warningButton.Visible = true;
                    else
                        warningButton.Visible = false;
                    StartForm(e);

                    if (loginScreen != null)
                    {
                        loginScreen.DataSubmitted -= OnLoginDataSubmitted;
                        loginScreen.CancelConnection -= OnConnectionCanceled;
                        loginScreen.Close();
                    }
                }
                else
                    NotificationHelper.ShowNotification(ExceptionManager.Error, e.Message, 10, OnNotificationMessageChanged);

                communicationManager.IsFetching = false;
            }));
        }

        private void OnDataSubmitted(object sender, FormEventArgs e)
        {
            Invoke(new Action(delegate()
            {
                Cursor.Current = Cursors.Default;

                if (e.Result == CommunicationResult.Success)
                {
                    communicationManager.RemoveForm(e.Info.RequestInfo.CompilationRequestId);
                    UpdateRecentForms();

                    NotificationHelper.ShowNotification(ExceptionManager.Success, ExceptionManager.FormSubmitted, 10, OnNotificationMessageChanged);
                }
                else
                    NotificationHelper.ShowNotification(ExceptionManager.Error, ExceptionManager.FormNotSubmitted);

                formScreen.EnableButtons(true);
            }));
        }

        private void OnNotificationMessageChanged(object sender, BalloonChangedEventArgs e)
        {
            if (e.Visible == false)
                Invoke(new Action(delegate()
                {
                    if (formScreen != null)
                    {
                        formScreen.ExternalClosing = true;
                        formScreen.Close();
                    }
                    if (UpdateRecentForms())
                        warningButton.Visible = true;
                    else
                        warningButton.Visible = false;
                }));
        }

        private void OnLoginNotificationMessageChanged(object sender, BalloonChangedEventArgs e)
        {
            if (e.Visible == false)
                Invoke(new Action(delegate()
                {
                    if (loginScreen != null)
                    {
                        loginScreen.DataSubmitted -= OnLoginDataSubmitted;
                        loginScreen.CancelConnection -= OnConnectionCanceled;
                        loginScreen.Close();
                    }
                }));
        }

        private void OnRecentFormSelected(object sender, RecentScreen.FormSelectionEventArgs e)
        {
            if (e.Action == RecentScreen.FormAction.Fill)
            {
                Cursor.Current = Cursors.WaitCursor;
                communicationManager.GetForm(e.RequestInfo);
            }
            else
            {
                communicationManager.RemoveForm(e.RequestInfo.CompilationRequestId);
                UpdateRecentForms();
            }
        }

        private void OnPrivateFormButtonClick(object sender, EventArgs e)
        {
            loginScreen = new FormFetchScreen();
            loginScreen.DataSubmitted += OnLoginDataSubmitted;
            loginScreen.CancelConnection += OnConnectionCanceled;
            loginScreen.Public = false;
            loginScreen.Show();
        }

        private void OnPublicFormButtonClick(object sender, EventArgs e)
        {
            loginScreen = new FormFetchScreen();
            loginScreen.DataSubmitted += OnLoginDataSubmitted;
            loginScreen.CancelConnection += OnConnectionCanceled;
            loginScreen.Public = true;
            loginScreen.Show();
        }

        private void OnResultsButtonClick(object sender, EventArgs e)
        {

        }

        private void OnHelpButtonClick(object sender, EventArgs e)
        {
            helpScreen = new HelpScreen() { MinimizeBox = false };
            helpScreen.Show();
        }

        private void OnSettingsButtonClick(object sender, EventArgs e)
        {
            settingsScreen = new SettingsScreen(configuration) { MinimizeBox = false };
            settingsScreen.ConfigurationChanged += OnConfigurationChanged;
            settingsScreen.Show();
        }

        private void OnWarningButtonClick(object sender, EventArgs e)
        {
            UpdateRecentForms();
            recentScreen.Show();
        }

        private void OnResultsMouseDown(object sender, MouseEventArgs e)
        {

        }

        private void OnResultsMouseUp(object sender, MouseEventArgs e)
        {

        }

        private void OnPublicFormMouseDown(object sender, MouseEventArgs e)
        {

        }

        private void OnPublicFormMouseUp(object sender, MouseEventArgs e)
        {

        }

        private void OnFormFillingFinished(object sender, FillingEventArgs e)
        {
            if (!e.Completed)
            {
                communicationManager.SaveForm(currentFormId, e.Result);
                NotificationHelper.ShowNotification(ExceptionManager.Warning, ExceptionManager.FormSaved);
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;
                communicationManager.SubmitForm(currentFormId, e.Result);
            }
        }

        public void OnNotificationReceived(object sender, NotificationReceivedEventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;

            FormInfo info = new FormInfo()
            {
                Status = FormStatus.Notified,
                RequestInfo = args.RequestInfo,
                Source = args.RequestInfo.From,
                NotificationTime = args.RequestInfo.Time
            };

            if (communicationManager.GetFormInfo(info.RequestInfo.CompilationRequestId) == null)
            {
                this.Invoke(new Action(delegate()
                {
                    communicationManager.StoreFormInfo(info);
                    UpdateRecentForms();
                    notificationScreen.ShowNotification(info.RequestInfo);
                    notificationScreen.Visible = true;
                }));
            }
            Cursor.Current = Cursors.Default;
        }

        public void OnNotificationApproved(object sender, NotificationScreen.NotificationEventArgs e)
        {
            if (e.Action == NotificationScreen.FormAction.Fill)
            {
                Cursor.Current = Cursors.WaitCursor;
                communicationManager.GetForm(e.Info);
            }
            else
            {
                notificationScreen.Hide();
                if (daemonStartup)
                    Application.Exit();
            }
        }

        private void OnBatteryLevelNotification(object sender, BatteryEventArgs e)
        {
            if (e.Level == BatteryLevel.VeryLow)
                communicationManager.SaveForm(currentFormId, e.Partial);
        }

        private void OnExitButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnCheckNowButtonClick(object sender, EventArgs e)
        {
            OutlookSession session = new OutlookSession();
            MessagingApplication.Synchronize();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (helpScreen != null)
                helpScreen.Close();
            if (recentScreen != null)
                recentScreen.Close();
            if (settingsScreen != null)
                settingsScreen.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            notificationReceiver.Stop();
            Application.Exit();
        }

        #endregion

        #endregion

        /// <summary>
        /// The Main form of the application
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="requestInfo"></param>
        /// <param name="pid"></param>
        /// <param name="communicationManager"></param>
        public MainScreen(ConfigurationHelper configuration, string requestInfo, int pid, CommunicationManager communicationManager)
        {
            InitializeComponent();
            this.communicationManager = communicationManager;

            currentFormId = -1;
            daemonPid = pid;

            this.configuration = configuration;

            recentScreen = new RecentScreen();
            recentScreen.FormSelected += OnRecentFormSelected;
            recentScreen.CancelFilling += OnFillingCanceled;

            notificationScreen = new NotificationScreen(null, null, -1, communicationManager);
            notificationScreen.Show();
            notificationScreen.Visible = false;
            notificationScreen.NotificationApproved += this.OnNotificationApproved;

            notificationReceiver = new NotificationReceiver();
            notificationReceiver.NotificationReceived += OnNotificationReceived;
            notificationReceiver.Start();

            Configuration conf = configuration.Configuration;
            communicationManager.FormFetched += OnDataSetFetched;
            communicationManager.DataSubmitted += OnDataSubmitted;

            backgroundPanel.BackgroundImage = Mobile.Properties.Resources.MainBackground;
            privateFormButton.BackgroundImage = Mobile.Properties.Resources.PrivateFormButton;
            privateFormButton.ClickedBackgroundImage = Mobile.Properties.Resources.PrivateFormButtonPressed;
            publicFormButton.BackgroundImage = Mobile.Properties.Resources.PublicFormButton;
            publicFormButton.ClickedBackgroundImage = Mobile.Properties.Resources.PublicFormButtonPressed;
            resultsButton.BackgroundImage = Mobile.Properties.Resources.ResultsButton;
            resultsButton.ClickedBackgroundImage = Mobile.Properties.Resources.ResultsButtonPressed;
            helpButton.BackgroundImage = Mobile.Properties.Resources.HelpButton;
            settingsButton.BackgroundImage = Mobile.Properties.Resources.SettingsButton;
            warningButton.BackgroundImage = Mobile.Properties.Resources.RecentButton;
            checkNewButton.BackgroundImage = Mobile.Properties.Resources.SynchButton;

            settingsButton.BackColor = helpButton.BackColor = warningButton.BackColor = Color.Transparent;
            privateFormButton.BackColor = publicFormButton.BackColor = resultsButton.BackColor = Color.Transparent;

            orientationWatcher = new SystemState(SystemProperty.DisplayRotation);
            orientationWatcher.Changed += OnOrientationWatcherChanged;

            UpdateRecentForms();

            if (pid != -1)
                daemonStartup = true;

            OnConfigurationChanged(this, new ConfigurationChangedEventArgs(configuration.Configuration));
            this.AddMenusToDictionary();
            this.UpdateResources();

        }
    }
}