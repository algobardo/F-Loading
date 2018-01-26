using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.WindowsMobile.Status;
using Mobile.Workflow;
using Mobile.Fields;
using Mobile.Util.Controls;
using Microsoft.WindowsCE.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Mobile.Language;

namespace Mobile
{
    /// <summary>
    /// The visible screen during a form compilation
    /// </summary>
    public partial class FormScreen : LocalizedForm
    {
        #region Translate
        private String exitButton = ResourceManager.Instance.GetString("ExitButton");
        private String nextButton = ResourceManager.Instance.GetString("NextButton");
        private String backButton = ResourceManager.Instance.GetString("BackButton");
        private String submitButton = ResourceManager.Instance.GetString("SubmitButton");
        private String closeButton = ResourceManager.Instance.GetString("CloseButton");
        private String doneButton = ResourceManager.Instance.GetString("DoneButton");
        #endregion
        #region Events

        /// <summary>
        /// Occurs when the user has finished to fill
        /// </summary>
        public event EventHandler<FillingEventArgs> FillingFinished;

        /// <summary>
        /// Occurs at specified intervals to check the battery status
        /// </summary>
        public event EventHandler<BatteryEventArgs> BatteryLevelNotification;

        #endregion

        #region Private

        private WorkflowManager workflowManager;
        private Thread batteryManager;
        private List<List<Control>> pages;
        private int currentPageIndex;
        private String currentNodeID;
        private String currentNodeTitle;
        private int maxContentHeight;
        private int controlPadding = 8;
        private bool saveOnBatteryLow;
        private Stack<Panel> separatedPanels;
        private enum leftActionButtons { Back, Exit, Close };
        private enum rightActionButtons { Next, Submit, Done };
        private int leftbutton, rightbutton;

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
            this.AddMenuToDictionary(this.nextMenuItem, "nextMenuItem");
            this.AddMenuToDictionary(this.previousMenuItem, "previousMenuItem");


        }
        #endregion
        /// <summary>
        /// If false this form is closing itself, 
        /// othewise another form is closing this
        /// </summary>
        public bool ExternalClosing
        {
            get;
            set;
        }

        /// <summary>
        /// The visible screen during a form compilation
        /// </summary>
        /// <param name="workflowManager"></param>
        /// <param name="saveOnBatteryLow">If true the status of the compilation is saved on battery low level</param>
        public FormScreen(WorkflowManager workflowManager, bool saveOnBatteryLow)
        {
            InitializeComponent();
            this.saveOnBatteryLow = saveOnBatteryLow;
            ExternalClosing = false;

            maxContentHeight = mainPanel.Height;

            separatedPanels = new Stack<Panel>();

            backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;
            backgroundPanel.SizeXMode = SizeMode.Stretch;
            backgroundPanel.SizeYMode = SizeMode.Stretch;
            
            this.workflowManager = workflowManager;
            progressBar.Load();
            progressBar.Enabled = false;

            pages = new List<List<Control>>();
            rightbutton = (int)rightActionButtons.Next;
            leftbutton = (int)leftActionButtons.Back;
            previousMenuItem.Text = backButton;
            nextMenuItem.Text = nextButton;
            if (workflowManager.Current == null)
            {
                //scrive exit nel tasto
                leftbutton = (int)leftActionButtons.Exit;
                previousMenuItem.Text = ResourceManager.Instance.GetString("ExitButton");
            }
            
            if(saveOnBatteryLow) 
            {
                batteryManager = new Thread(new ThreadStart(CheckBatteryStatus));
                batteryManager.Start();
            }

            NodeController node = workflowManager.Next();

            progressBar.NumCircles = workflowManager.EstimatedRemainingNodesCount + workflowManager.CurrentNodeCount - 1;
            progressBar.Status = workflowManager.CurrentNodeCount;

            if (node != null)
            {
                currentNodeID = node.ID;
                currentNodeTitle = node.Name;
                progressBar.Title = currentNodeTitle;
                BuildPages(node);

                if (pages.Count >= 0)
                    ShowPage(0);
            }
            else
            {
                //scrive submit nel tasto

                ShowSummary();
                rightbutton = (int)rightActionButtons.Submit;
                nextMenuItem.Text = submitButton;
            }
        }

        /// <summary>
        /// Enable and disable menu buttons
        /// </summary>
        /// <param name="enable"></param>
        public void EnableButtons(bool enable)
        {
            nextMenuItem.Enabled = enable;
            previousMenuItem.Enabled = enable;
        }

        /// <summary>
        /// Sets the picture background
        /// </summary>
        /// <param name="b"></param>
        public void SetBackground(Bitmap b)
        {
            backgroundPanel.BackgroundImage = b;
        }

        /// <summary>
        /// Change the orientation of the childs controls
        /// </summary>
        /// <param name="angle"></param>
        public void ChangeOrientation(ScreenOrientation angle)
        {
            progressBar.ChangeOrientation();
        }

        #region Private Methods

        private void BuildPages(NodeController node)
        {
            ClearPages();
            currentPageIndex = 0;

            int offset = 0;
            int currentIndex = 0;

            for (int i = 0; i < node.Controllers.Length; i++)
            {
                //Register to the event raised when a controller whants to be displayed in a single isolated page
                node.Controllers[i].SeparatedEditingRequest += OnSeparatedEditingRequest;

                Control container = CreateControllerContainer(node.Controllers[i] as Control);

                container.GotFocus += OnContainerFocus;
                container.TabIndex = Mobile.Util.TabIndexGenerator.NextTabIndex();

                if (offset + container.Height > maxContentHeight)
                {
                    if (pages.Count > currentIndex)
                    {
                        //Remove the bottom offset from the last container of the current page
                        Control lastContainer = pages[currentIndex].Last();
                        lastContainer.Height = lastContainer.Controls[0].Height;

                        offset = 0;
                        currentIndex++;
                    }
                }

                if (pages.Count == currentIndex)
                    pages.Add(new List<Control>());

                pages[currentIndex].Add(container);
                offset += container.Height;
            }

            if (node.Controllers.Length >= 0)
            {
                //Remove the bottom offset from the last container of the last page
                Control lastContainer = pages[currentIndex].Last();
                lastContainer.Height = lastContainer.Controls[0].Height;
            }
        }

        private void ClearPages()
        {
            foreach (List<Control> containers in pages)
            {
                foreach (Control container in containers)
                {
                    (container.Controls[0] as FieldController).SeparatedEditingRequest -= OnSeparatedEditingRequest;
                    container.Dispose();
                }

                containers.Clear();
            }
            pages.Clear();
        }

        private bool CheckContraints()
        {
            for (int i = 0; i < pages.Count; i++)
            {
                try
                {
                    foreach (Control container in pages[i])
                        ((container.Controls[0]) as FieldController).CheckConstraints();
                }
                catch (FieldException exc)
                {
                    currentPageIndex = i;
                    ShowPage(currentPageIndex);

                    NotificationHelper.ShowNotification("Error", exc.CustomMessage);

                    return false;
                }
            }
            return true;
        }

        private bool CheckContraints(FieldController controller)
        {
            try
            {
                controller.CheckConstraints();
            }
            catch (FieldException exc)
            {
                NotificationHelper.ShowNotification("Error", exc.CustomMessage);

                return false;
            }
            return true;
        }

        private Control CreateControllerContainer(Control controller, int paddingUp, int paddingDown)
        {
            Control container = new Control()
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top,
                Dock = DockStyle.Top,
                Width = mainPanel.Width,
                Height = controller.Height + paddingDown + paddingUp
            };

            controller.Width = container.Width;
            controller.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            controller.Top = paddingUp;

            container.Controls.Add(controller);
            return container;
        }

        private Control CreateControllerContainer(Control controller)
        {
            return CreateControllerContainer(controller, 0, controlPadding);
        }

        private void SendFillingFinishMessage(bool finished)
        {
            if (FillingFinished != null)
                FillingFinished(this, new FillingEventArgs(finished, workflowManager.ToXml()));
        }

        private void ShowPage(int index)
        {
            mainPanel.Controls.Clear();

            int offset = 0;
            for (int i = pages[index].Count - 1; i >= 0; i--)
            {
                mainPanel.Controls.Add(pages[index][i]);
                offset += pages[index][i].Height;
            }
            mainPanel.Height = offset;
            containerPanel.Height = mainPanel.Height + (2 * mainPanel.Top);
            containerPanel.Invalidate();
        }

        private void ShowSummary()
        {
            ClearPages();

            currentPageIndex = 0;
            currentNodeTitle = "Summary";
            progressBar.Title = currentNodeTitle;

            mainPanel.Controls.Clear();

            int offset = 0;
            NodeController[] nodes = workflowManager.Summary;
            for (int i = nodes.Length - 1; i >= 0; i--)
            {
                if (i < nodes.Length - 1)
                {
                    //Add separator line
                    Control control = new Control()
                    {
                        BackColor = Color.FromArgb(146, 146, 146),
                        Width = mainPanel.Width,
                        Height = 1,
                        Dock = DockStyle.Top
                    };
                    Control container = CreateControllerContainer(control, 10, 10);
                    mainPanel.Controls.Add(container);

                    offset += container.Height;
                }
                for (int j = nodes[i].Controllers.Length - 1; j >= 0; j--)
                {
                    Control container = CreateControllerContainer(nodes[i].Controllers[j] as Control);
                    mainPanel.Controls.Add(container);

                    if (j == nodes[i].Controllers.Length - 1)
                        container.Height = container.Controls[0].Height;

                    offset += container.Height;

                    nodes[i].Controllers[j].SeparatedEditingRequest += OnSeparatedEditingRequest;
                }
            }
            mainPanel.Height = offset;
            containerPanel.Height = mainPanel.Height + (2 * mainPanel.Top);
            containerPanel.Invalidate();

            nextMenuItem.Enabled = true;
        }

        private void OnContainerFocus(object sender, EventArgs args)
        {
            ((sender as Control).Controls[0] as FieldController).SetFocus();
        }

        private void UpdateMenuItems(bool separatedEditing)
        {
            if (separatedEditing)
            {
                //imposta nei pulsanti sin e destro rispettivamente Close e Done
                leftbutton = (int)leftActionButtons.Close;
                rightbutton = (int)rightActionButtons.Done;
                previousMenuItem.Text = closeButton;
                nextMenuItem.Text = doneButton;
            }
            else
            {
                if (currentPageIndex != 0 || currentNodeID != workflowManager.FirstNodeID)
                {
                    //imposta back al tasto sinistro
                    leftbutton = (int)leftActionButtons.Back;
                    previousMenuItem.Text = backButton;
                }
                else
                {
                    // imposta exit al tasto sinistro
                    leftbutton = (int)leftActionButtons.Exit;
                    previousMenuItem.Text = exitButton;
                }

                if (currentNodeID == null)
                {
                    //imposta submit al pulsante destro
                    rightbutton = (int)rightActionButtons.Submit;
                    nextMenuItem.Text = submitButton;
                }

                else
                {
                    //imposta next al pulsante destro
                    rightbutton = (int)rightActionButtons.Next;
                    nextMenuItem.Text = nextButton;
                }
            }
        }

        private void CheckBatteryStatus()
        {
            while (true)
            {
                Thread.Sleep(Int32.Parse(Mobile.Properties.Resources.BatterySaveDelay));
                SystemProperty batteryInfo = SystemProperty.PowerBatteryStrength;
                BatteryLevel level = (BatteryLevel)SystemState.GetValue(batteryInfo);
                if (BatteryLevelNotification != null)
                    BatteryLevelNotification(this, new BatteryEventArgs(level, workflowManager.ToXml()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!ExternalClosing)
            {
                if (batteryManager != null)
                    batteryManager.Abort();

                SendFillingFinishMessage(false);
            }
        }

        private void OnNextButtonClick(object sender, EventArgs e)
        {
            if (rightbutton == (int)rightActionButtons.Next)
            {
                if (currentPageIndex == pages.Count - 1)
                {
                    if (CheckContraints())
                    {
                        NodeController node = workflowManager.Next();

                        if (node != null)
                        {
                            if (node.ID == currentNodeID)
                                NotificationHelper.ShowNotification(ExceptionManager.Error, ExceptionManager.InformationSubmitted);
                            else
                            {
                                currentNodeID = node.ID;
                                currentNodeTitle = node.Name;

                                BuildPages(node);

                                if (pages.Count >= 0)
                                    ShowPage(0);

                                progressBar.NumCircles = workflowManager.EstimatedRemainingNodesCount + workflowManager.CurrentNodeCount - 1;
                                progressBar.Status = workflowManager.CurrentNodeCount;
                                progressBar.Title = currentNodeTitle;
                            }
                        }
                        else
                        {
                            ShowSummary();
                            //imposta submit come tasto destro
                            rightbutton = (int)rightActionButtons.Submit;
                            nextMenuItem.Text = submitButton;
                        }
                    }
                }
                else
                {
                    currentPageIndex++;
                    ShowPage(currentPageIndex);
                }
            }
            else if (rightbutton == (int)rightActionButtons.Done)
            {
                bool checkedContraints = true;
                for (int i = separatedPanels.Peek().Controls.Count - 1; i >= 0; i--)
                    checkedContraints = checkedContraints && CheckContraints(separatedPanels.Peek().Controls[i].Controls[0] as FieldController);

                if (checkedContraints)
                {
                    progressBar.Title = currentNodeTitle;

                    containerPanel.Controls.Remove(separatedPanels.Pop());
                    if (separatedPanels.Count == 0)
                    {
                        backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;
                        mainPanel.Show();
                        containerPanel.Height = mainPanel.Height + (2 * mainPanel.Top);

                        if (currentNodeID != workflowManager.FirstNodeID || currentPageIndex > 0)
                        {
                            leftbutton = (int)leftActionButtons.Back;
                            previousMenuItem.Text = backButton;
                        }
                        else
                        {
                            leftbutton = (int)leftActionButtons.Exit;
                            previousMenuItem.Text = exitButton;
                        }
                        if (currentNodeTitle.Equals("Summary"))
                        {
                            rightbutton = (int)rightActionButtons.Submit;
                            nextMenuItem.Text = submitButton;
                        }
                        else
                        {
                            rightbutton = (int)rightActionButtons.Next;
                            nextMenuItem.Text = nextButton;
                        }
                    }
                    else
                        containerPanel.Height = separatedPanels.Peek().Height + (2 * separatedPanels.Peek().Top);

                    containerPanel.Invalidate();
                }
                return;
            }
            else if (rightbutton == (int)rightActionButtons.Submit)
            {
                nextMenuItem.Enabled = false;
                SendFillingFinishMessage(true);
            }

            if (currentPageIndex != 0 || currentNodeID != workflowManager.FirstNodeID)
            {
                leftbutton = (int)leftActionButtons.Back;
                previousMenuItem.Text = backButton;
            }
        }

        private void OnPreviousButtonClick(object sender, EventArgs e)
        {
            if (leftbutton == (int)leftActionButtons.Back)
            {
                if (currentPageIndex == 0)
                {
                    if (CheckContraints())
                    {
                        NodeController node = workflowManager.Previous();
                        if (node != null)
                        {
                            currentNodeID = node.ID;
                            currentNodeTitle = node.Name;

                            BuildPages(node);

                            if (pages.Count >= 0)
                                ShowPage(pages.Count - 1);
                            currentPageIndex = pages.Count - 1;

                            if (node.ID == workflowManager.FirstNodeID && currentPageIndex == 0)
                            {
                                leftbutton = (int)leftActionButtons.Exit;
                                previousMenuItem.Text = exitButton;
                            }
                            if (rightbutton == (int)rightActionButtons.Submit)
                            {
                                rightbutton = (int)rightActionButtons.Next;
                                nextMenuItem.Text = nextButton;
                            }
                            progressBar.Status = workflowManager.CurrentNodeCount;
                            progressBar.Title = currentNodeTitle;
                        }
                    }
                }
                else
                {
                    currentPageIndex--;
                    ShowPage(currentPageIndex);
                    if (currentNodeID == workflowManager.FirstNodeID && currentPageIndex == 0)
                    {
                        leftbutton = (int)leftActionButtons.Exit;
                        previousMenuItem.Text = exitButton;
                    }
                }
            }
            else if (leftbutton == (int)leftActionButtons.Close)
            {
                progressBar.Title = currentNodeTitle;

                containerPanel.Controls.Remove(separatedPanels.Pop());
                if (separatedPanels.Count == 0)
                {
                    backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;

                    mainPanel.Show();
                    containerPanel.Height = mainPanel.Height + (2 * mainPanel.Top);
                    leftbutton = (int)leftActionButtons.Back;
                    previousMenuItem.Text = backButton;
                    if (currentNodeTitle.Equals("Summary"))
                    {
                        rightbutton = (int)rightActionButtons.Submit;
                        nextMenuItem.Text = submitButton;
                    }
                    else
                    {
                        rightbutton = (int)rightActionButtons.Next;
                        nextMenuItem.Text = nextButton;
                    }
                }
                else
                    containerPanel.Height = separatedPanels.Peek().Height + (2 * separatedPanels.Peek().Top);

                containerPanel.Invalidate();
            }
            else if (leftbutton == (int)leftActionButtons.Exit)
            {
                this.Close();
            }
        }

        private void OnSeparatedEditingRequest(object sender, SeparatedFieldEditingEventArgs e)
        {
            if (separatedPanels.Count == 0)
                backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackgroundDark;

            Panel separatedPanel = new Panel()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Size = mainPanel.Size,
                Location = mainPanel.Location
            };
            separatedPanel.Visible = false;
            containerPanel.Controls.Add(separatedPanel);
            separatedPanels.Push(separatedPanel);

            int offset = 0;
            for (int i = e.Controllers.Length - 1; i >= 0; i--)
            {
                Control container = CreateControllerContainer(e.Controllers[i] as Control);
                separatedPanel.Controls.Add(container);
                offset += container.Height;
            }
            if (e.Controllers.Length > 0)
            {
                offset -= separatedPanel.Controls[e.Controllers.Length - 1].Height;
                offset += separatedPanel.Controls[e.Controllers.Length - 1].Controls[0].Height;
                separatedPanel.Controls[e.Controllers.Length - 1].Height = separatedPanel.Controls[e.Controllers.Length - 1].Controls[0].Height;
            }

            separatedPanel.Height = offset;
            containerPanel.Height = separatedPanel.Height + (2 * separatedPanel.Top);
            mainPanel.Hide();
            separatedPanel.Show();
            leftbutton = (int)leftActionButtons.Close;
            previousMenuItem.Text = closeButton;
            rightbutton = (int)rightActionButtons.Done;
            nextMenuItem.Text = doneButton;

            containerPanel.Invalidate();
            Refresh();

            if (e.DisplayInfo != null)
                progressBar.Title = e.DisplayInfo;
        }

        #endregion
    }

    /// <summary>
    /// Event class for filling events
    /// </summary>
    public class FillingEventArgs : EventArgs
    {
        /// <summary>
        /// Event class for filling events
        /// </summary>
        /// <param name="completed"></param>
        /// <param name="result"></param>
        public FillingEventArgs(bool completed, XmlDocument result)
            : base()
        {
            this.Completed = completed;
            this.Result = result;
        }

        /// <summary>
        /// True if the form is completely filled
        /// </summary>
        public bool Completed
        {
            get;
            private set;
        }

        /// <summary>
        /// The XML rappresentation of the filled form
        /// </summary>
        public XmlDocument Result
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Event class for battery checking
    /// </summary>
    public class BatteryEventArgs : EventArgs
    {
        /// <summary>
        /// Event class for battery checking
        /// </summary>
        /// <param name="level"></param>
        /// <param name="result"></param>
        public BatteryEventArgs(BatteryLevel level, XmlDocument result)
            : base()
        {
            this.Level = level;
            this.Partial = result;
        }

        /// <summary>
        /// The current level of the battery
        /// </summary>
        public BatteryLevel Level
        {
            get;
            private set;
        }

        /// <summary>
        /// The partial XML rappresentation of the filled form
        /// </summary>
        public XmlDocument Partial
        {
            get;
            private set;
        }
    }
}