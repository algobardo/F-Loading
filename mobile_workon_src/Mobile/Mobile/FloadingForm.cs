/*
 *  Authors Stefano Paganucci, Alessandro Poggi
 */

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
using Mobile.Fields;
using Mobile.Workflow;
using Microsoft.WindowsMobile.Status;
using Microsoft.WindowsCE.Forms;
using Microsoft.Win32;
namespace Mobile
{
    public partial class FloadingForm : Form
    {

        private CommunicationManager comManager;

        private int selectedForm = 1;
        private SystemState _orientationWatcher;
        private Menu mainMenu;

        public FloadingForm()
        {
            InitializeComponent();
            
            
            comManager = new CommunicationManager();
             mainMenu = new Menu();
            comManager.FormFetched += OnFormFetched;
            //screen rotation event
            _orientationWatcher = new SystemState(SystemProperty.DisplayRotation);
            _orientationWatcher.Changed += new ChangeEventHandler(OrientationWatcher_Changed);
            
        }

       


        public void CreateForm(object sender, FormFetchEventArgs args)
        {
            

            FieldsManager fm = new FieldsManager(args.Types);
            new FormView(comManager, 
                        this,
                        fm,
                        new WorkflowManager(fm, args.WorkflowNodes, args.WorkflowEdges, args.Types),
                        args.WorkflowNodes.ChildNodes[1].ChildNodes.Count).Show();
        }

        private void OnFormFetched(object source, FormFetchEventArgs args)
        {
            if (args.Result == CommunicationResult.Success)
            {
                Invoke(new EventHandler<FormFetchEventArgs>(CreateForm), source, args);
            }
            else
            {
                //Show Error
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            switch (this.OkButton.Text)
            {
                case "Login":
                   
                    this.Controls.Clear();
                   
                    this.Controls.Add(mainMenu);
                    this.OkButton.Text = "Fill";
                    break;
                case "Fill":
                   
                    Cursor.Current = Cursors.WaitCursor;
                   
                    comManager.FetchForm(selectedForm);
                    break;
            }
            Refresh();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FloadingForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }

        }
        void OrientationWatcher_Changed(object sender, ChangeEventArgs args)
{
      int newOrientation = (int)args.NewValue;
      //System.Diagnostics.Debug.WriteLine("Orientation: " + newOrientation);
      if (newOrientation == 90 || newOrientation == 270)
      {
          ChangeToLandscape(90);
          
          //System.Diagnostics.Debug.WriteLine("login panel size: ["+ loginpanel.Size.Width + "," + loginpanel.Size.Height + "]");
          //System.Diagnostics.Debug.WriteLine("screen  size: [" + Screen.PrimaryScreen.WorkingArea.Size.Width + "," + Screen.PrimaryScreen.WorkingArea.Size.Height + "]");
          //dosn't work how i suppose...
         // this.loginpanel.Width = Screen.PrimaryScreen.Bounds.Width; 
          
          this.loginpanel.Width = 307;
          this.mainMenu.Width = 268;
          this.mainMenu.Height = 240;
          System.Diagnostics.Debug.WriteLine("after rotating: login panel size: [" + loginpanel.Size.Width + "," + loginpanel.Size.Height + "]");
          
      }
      if (newOrientation == 0 || newOrientation == 180)
      {
          ChangeToLandscape(0);
          this.loginpanel.Width = Screen.PrimaryScreen.Bounds.Width;
          this.mainMenu.Width = 240;
          this.mainMenu.Height = 268;

      }
      

}
        public static void ChangeToLandscape(int angle)
        {
            switch (angle)
            {
                case 0:
                    SystemSettings.ScreenOrientation = ScreenOrientation.Angle0;
                    break;
                case 90: SystemSettings.ScreenOrientation = ScreenOrientation.Angle90;
                    break;
                case 180: SystemSettings.ScreenOrientation = ScreenOrientation.Angle180;
                    break;
                case 270: SystemSettings.ScreenOrientation = ScreenOrientation.Angle270;
                    break;
            }
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\GDI\ROTATION",
                             "Angle", angle, RegistryValueKind.DWord);
            
           
        }

       

 
    }
}