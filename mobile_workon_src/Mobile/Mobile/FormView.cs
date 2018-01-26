using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Mobile.Communication;
using Mobile.Workflow;
using Mobile.Fields;
using Microsoft.WindowsCE.Forms;
namespace Mobile
{

    public partial class FormView : Form
    {
        private const string NextText = "Next";
        private const string SendText = "Send";

        private const int controlDistance = 10;
        private const int TopOffset = 20;

        private WorkflowManager.Node currentNode;

        private int steps = 0;

        private WorkflowManager workflowManager;

        private FieldsManager fieldsManager;

        private FloadingForm menuList;

        private CommunicationManager communicationManager;

        private Timer timer;

        private bool forwardAnimation;

        private Notification notification;

        #region Inizializers

        public FormView(CommunicationManager cm, FloadingForm list, FieldsManager fm, WorkflowManager wm, int n)
        {
            InitializeComponent();

            #region ProgressBar

            // 
            // progressBar
            // 
            this.progressBar = new Mobile.ProgressBar(n);
            this.progressBar.BackColor = System.Drawing.Color.Black;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(240, 35);
            this.progressBar.TabIndex = 0;
            this.progressBar.Text = "progressBar1";
            this.progressBar.Load();
            this.Controls.Add(this.progressBar);

            #endregion

            fieldsManager = fm;
            workflowManager = wm;
            communicationManager = cm;
            menuList = list;
            timer = new Timer();
            timer.Interval = 1;
            timer.Tick += new EventHandler(Timer_Tick);
            notification = new Notification();
            //notification.BalloonChanged += new Microsoft.WindowsCE.Forms.BalloonChangedEventHandler(n_BalloonChanged);
            notification.Caption = "Error";
            notification.Critical = true;
        }

        private void FormView_Load(object sender, EventArgs e)
        {
            DoStep();
        }

        #endregion

        private void Back_Click(object sender, EventArgs e)
        {
            if (steps == 0)
            {
                menuList.Show();
                Dispose();
                return;
            }
            for (int i = 0; i < Panel.Controls.Count; i++)
                currentNode.Data[i].Update(Panel.Controls[i]);

            StartLoader();
            DoStepBack();
        }
       
        private void Next_Click(object sender, EventArgs e)
        {
            StartLoader();

            switch (Next.Text)
            {
                case SendText:
                    communicationManager.SubmitData(workflowManager.Result);  //Send the results to the server
                    menuList.Show();
                    Dispose();
                    break;

                case NextText:

                    for (int i = 0; i < Panel.Controls.Count; i++)
                    {
                        //tmList[i].ToXml(Panel.Controls[i], currentNode.ChildNodes[i] as XmlAnnotatedElement);
                        try
                        {
                            currentNode.Data[i].Update(Panel.Controls[i]);
                            currentNode.Data[i].CheckConstraints();
                            Panel.Controls[i].BackColor = Color.White;
                        }
                        catch (FieldsException fe)
                        {
                            //Panel.Controls[i].BackColor = Color.Red;
                            currentNode.Data[i].RenderAdorner(Panel.Controls[i], AdornerType.Error);
                            notification.Text = fe.CustomMessage;
                            notification.Visible = true;
                            StopLoader();
                            Refresh();
                            return;
                        }
                    }

                    DoStep();
                    break;
            }
        }

        /// <summary>
        /// Obtains the previou workflow node and disposes the elemens on the screen
        /// </summary>
        /// <param name="current">An XmlNode rappresenting the current state</param>

        private void DoStepBack()
        {
            currentNode = workflowManager.PreviousNode(currentNode);

            StopLoader();

            if (currentNode != null)
            {
                PrepareTransition();
                Panel.Controls.Clear();

                #region Rendering

                List<Control> controls = new List<Control>();
                for (int i = 0; i < currentNode.Data.Length; i++)
                    controls.Add(currentNode.Data[i].Render(this.Bounds));

                //Controls disposition

                int offset = TopOffset;
                foreach (Control c in controls)
                {
                    c.LostFocus += new EventHandler(OnLostFocus);
                    c.Top = offset;
                    offset += c.Height + controlDistance;
                    this.Panel.Controls.Add(c);
                    this.Controls.Add(this.Panel);
                }

                #endregion

                StartBackwardTransition();
            }
        }

        private void DoStep()
        {
            currentNode = workflowManager.NextNode(currentNode);

            StopLoader();

            if (currentNode == null)
            {
                #region Filling Complete

                //result.Push(current.OuterXml);

                //Modify this part
                Panel.Controls.Clear();
                
                Label l = new Label();
                l.Width = 1000;
                StringBuilder message = new StringBuilder();
                message.Insert(0, workflowManager.Result.OuterXml);

                l.Text = message.ToString();
                l.Height = CFMeasureString.MeasureString(l, message.ToString(), l.ClientRectangle).Height;
                l.Location = new Point(30, 100);
                Panel.Controls.Add(l);
                Next.Text = "Send";
                Back.Enabled = false;
                Back.Text = "";
                Refresh();

                #endregion

                return;
            }
/*
            if (currentNode != null)
                if (!currentNode.Name.Equals(currentNode.Name))
                    currentNode.GetType();//result.Push(current.OuterXml + " ");
                else
                {
                    notification.Text = "Error";
                    notification.Visible = true;
                    return;
                }
            */
            PrepareTransition();
            Panel.Controls.Clear();

            #region Rendering

            List<Control> controls = new List<Control>();
            for (int i = 0; i < currentNode.Data.Length; i++)
                controls.Add(currentNode.Data[i].Render(this.Bounds));
            //Controls disposition

            int offset = TopOffset;
            foreach (Control c in controls)
            {
                c.LostFocus += new EventHandler(OnLostFocus);
                c.Top = offset;
                offset += c.Height + controlDistance;
                this.Panel.Controls.Add(c);
                this.Controls.Add(this.Panel);
            }

            #endregion

            if (currentNode != null)
                StartForwardTransition();
                    
        }

        private void OnLostFocus(object sender, EventArgs args)
        {
        }

        private void n_BalloonChanged(object sender, BalloonChangedEventArgs e)
        {
            // The Visible property indicates the current state of the
            // popup notification balloon
            if (e.Visible == false)
            {
                // If the balloon has now been hidden, display a message box
                // to the user.
                // MessageBox.Show("The balloon has been closed", "Status");
                notification.Dispose();
            }
        }

        #region Loader

        private void StartLoader()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        private void StopLoader()
        {
            Cursor.Current = Cursors.Default;
        }

        #endregion

        #region Transition Animation

        private void PrepareTransition()
        {
            foreach (Control c in Panel.Controls)
                transPanel.Controls.Add(c);
        }

        private void StartForwardTransition()
        {
            forwardAnimation = true;
            transPanel.Left = 0;
            timer.Enabled = true;
            Panel.Left = this.Width + 10;
            transPanel.Visible = true;
            Panel.AutoScroll = false;
            this.AutoScroll = false;
        }

        private void StartBackwardTransition()
        {
            forwardAnimation = false;
            transPanel.Left = 0;
            timer.Enabled = true;
            Panel.Left = -this.Width - 10;
            transPanel.Visible = true;
            Panel.AutoScroll = false;
            this.AutoScroll = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (forwardAnimation)
                if (Panel.Location.X <= 0)
                {
                    timer.Enabled = false;
                    transPanel.Visible = false;
                    this.AutoScroll = true;
                    Panel.AutoScroll = true;
                    transPanel.Controls.Clear();
                    progressBar.SetProgress(++steps);
                }
                else
                {
                    transPanel.Left -= 5;
                    Panel.Left -= 5;
                }
            else
                if (Panel.Location.X >= 0)
                {
                    timer.Enabled = false;
                    transPanel.Visible = false;
                    Panel.AutoScroll = true;
                    this.AutoScroll = true;
                    transPanel.Controls.Clear();
                    progressBar.SetProgress(--steps);
                }
                else
                {
                    transPanel.Left += 5;
                    Panel.Left += 5;
                }
            Refresh();
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.Black), e.ClipRectangle);
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        #endregion
    }
}