using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Mobile
{
    public partial class LoginPanel : UserControl
    {
        public LoginPanel()
        {
            InitializeComponent();
        }

        

        private void LoginPanel_Resize(object sender, EventArgs e)
        {
    //        Control control = (Control)sender;
      //      System.Diagnostics.Debug.WriteLine("evento resize login");
            // Ensure the Form remains square (Height = Width).
        //    System.Diagnostics.Debug.WriteLine("Size Screen: [" + Screen.PrimaryScreen.WorkingArea.Size.Width + "," + Screen.PrimaryScreen.WorkingArea.Size.Height+"]");
          //  control.Size = Screen.PrimaryScreen.WorkingArea.Size;
            
            
        }
    }
    
}



