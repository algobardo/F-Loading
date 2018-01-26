using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mobile.Util.Controls;
using Mobile.Settings;

namespace Mobile.Language
{
    /// <summary>LocalizedForm provides methods and properties for custom localization of the user interface. 
    /// Localizable texts are loaded from external resource files using <see cref="ResourceManager"/>.</summary>
    /// <remarks>All forms in the application should be derived from this class. The subclasses should 
    /// call <see cref="AddMenusToDictionary"/> method first and then <see cref="UpdateResources()"/> method
    /// in the form constructor. The calls to these methods should be made after initializing the control on
    /// the form. The subclasses also must override <see cref="AddMenusToDictionary"/> method if they have
    /// custome menu items.</remarks>
    /// <seealso cref="ResourceManager"/>
    public partial class LocalizedForm : Form
    {
        /// <summary>The dictionary that binds menu items to the custom names that are used retrieving
        /// localized texts.</summary>
        private Dictionary<MenuItem, string> _menus = new Dictionary<MenuItem, string>();
        
        public LocalizedForm()
        {
            // make sure we get notification of any culture change
            ResourceManager.Instance.CultureChanged += this.OnCultureChanged;
            InitializeComponent();
        }

        /// <summary>Clears the menu item names dictionary.</summary>
        protected void ClearMenuDictionary()
        {
            _menus.Clear();
        }

        /// <summary>Associates menu item with the custom name.</summary>
        /// <remarks>This is necessary because MenuItem does not
        /// derive from Control and has no Name property or
        /// any other way to uniquely identify them at runtime</remarks>
        /// <param name="menuItem">The menu item on the form.</param>
        /// <param name="name">The custom name of the menu item that is used to retrieve localizable text.</param>
        protected void AddMenuToDictionary(MenuItem menuItem, string name)
        {
            // remove the old item if there is one
            if (_menus.ContainsKey(menuItem))
                _menus.Remove(menuItem);
            // add the item to the collection
            _menus.Add(menuItem, ResourceManager.Instance.GetResourceName(this.TopLevelControl, name));
        }

        /// <summary>The subclasses of the LocalizedForm should override this method and use this method to
        /// associate all menu items with custom names with the help of <see cref="AddMenuToDictionary"/>
        /// method.</summary>
        protected virtual void AddMenusToDictionary()
        {
            //do nothing
        }

        /// <summary>The method is called when culture is changed in the <see cref="ResourceManager"/>.
        /// Reloads all forms resources from the <see cref="ResourceManager"/>.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        protected void OnCultureChanged(object sender, EventArgs e)
        {
            UpdateResources();
        }

        /// <summary>Updates the forms resources - the text of the controls, images and the menu item texts.</summary>
        protected void UpdateResources()
        {
            UpdateControls(this);
            if (this.Menu != null)
                UpdateMenus(this.Menu.MenuItems);
            
        }

        /// <summary>Updates the menu item texts. First custom name for the menu item is retrieved from
        /// the <see cref="_menus"/> dictionary. The custom name is used to retrieve the localized text
        /// from the <see cref="ResourceManager"/>.</summary>
        /// <remarks>This method uses recursion to update all possible menu hierarchy.</remarks>
        protected void UpdateMenus(System.Windows.Forms.Menu.MenuItemCollection mc)
        {
            foreach (MenuItem m in mc)
            {                
                try
                {
                    // get the control name from our dictionary
                    string name = string.Empty;
                    _menus.TryGetValue(m, out name);
                    // retreive the localized resource
                    m.Text = ResourceManager.Instance.GetString(name);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    // log the error if desired
                }
                // work recursivly
                UpdateMenus(m.MenuItems);
            }
        }

        /// <summary>Updates text for the form control. The name of the control is used to retrieve the 
        /// localized text from the <see cref="ResourceManager"/>.</summary>
        /// <remarks>This method uses recursion to update all possible control hierarchy.
        /// <note>The method currently supports these controls:
        /// <list type="bullet">
        /// <item><description>TextBox</description></item>
        /// <item><description>Label</description></item>
        /// <item><description>Button</description></item>
        /// <item><description>CheckBox</description></item>
        /// <item><description>TabPage</description></item>
        /// <item><description>Form</description></item>
        /// <item><description>RadioButton</description></item>
        /// <item><description>PictureBox</description></item></list>
        /// </note>
        /// </remarks>
        protected void UpdateControls(Control control)
        {
            // update the controls that we can do automatically
            if (control is TextBox ||
                control is Label ||
                control is Button ||
                control is CheckBox ||
                control is TabPage ||
                control is Form ||
                control is RadioButton||
                control is BackgroundedControl||
                control is BackgroundedButton|| 
                control is BackgroundedPanel)
            {
                try
                {
                    control.Text = ResourceManager.Instance.GetString(control);
                    if (control is BackgroundedControl||control is BackgroundedButton|| control is BackgroundedPanel)
                        control.Refresh();
                    
                  
                }
                catch (System.Resources.MissingManifestResourceException mmre)
                {

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    // log the error if desired
                }
            }
            /*
            else if (control is DataGrid)
            {
                you need to iterate through the columns on the 
                c.DataSource and change the column header text
            }
            */
            else if (control is PictureBox)
            {
                try
                {
                    ((PictureBox)control).Image = (Image)ResourceManager.Instance.GetObject(control);
                }
                catch
                {
                    // log the error if desired
                }
            }
            try
            {
                foreach (Control c in control.Controls)
                {
                    // get any nested controls
                    UpdateControls(c);
                }
            }
            catch (ObjectDisposedException e)
            {
                //System.Diagnostics.Debug.WriteLine(e.Message);
                //Come si risolve? Ho provato con un iteratore ma mi da un eccezione anche control.Controls.getEnumerator...
            }
        }
    }
}