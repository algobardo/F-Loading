using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Mobile.Language
{
    /// <summary>ResourceManager provides the means for custom localization of the user interface. 
    /// Localizable texts are loaded from external resource files using <see cref="System.Resources.ResourceManager"/>.</summary>
    /// <remarks>
    /// <note>Most of the code is implemented by Tony Selke. Comments added by Marius Rybelis.</note></remarks>
    /// <seealso cref="GeneralElectric.DigitalYard.HandHeld.UserInterface.Common.LocalizedForm"/>
    /// <seealso cref="System.Resources.ResourceManager"/>
    public sealed class ResourceManager
    {
        /// <summary>The reference to the executing assembly. The assembly is used to load resource files.</summary>
        private Assembly _assy;

        /// <summary>The reference to the current culture.</summary>
        private CultureInfo _ci = CultureInfo.CurrentCulture;

        /// <summary>The reference to the <see cref="System.Resources.ResourceManager"/> that is used to 
        /// load the localized texts.</summary>
        /// <seealso cref="System.Resources.ResourceManager"/>
        private System.Resources.ResourceManager _rm = null;

        /// <summary>The event is fired when the current culture is changed.</summary>
        /// <permission cref="System.Security.PermissionSet">Everyone can access this method.</permission>
        public event EventHandler CultureChanged;

        // you can't instantiate this class
        private ResourceManager()
        { }

        #region Singleton Pattern

        /// <summary>This is our gatekeeper to the single instance.</summary>
        /// <permission cref="System.Security.PermissionSet">Everyone can access this method.</permission>
        /// <value>The reference to the single instance of this class.</value>
        public static ResourceManager Instance
        {
            get { return Nested.instance; }
        }

        /// <summary>This is hidden, internal instance wrapper.</summary>
        private class Nested
        {
            /// <summary>We need this explicit static constructor to tell C# compiler not to mark 
            /// type as 'beforefieldinit'</summary>
            static Nested()
            { }

            /// <summary>Because our parent is sealed, this is as good as private and can only be 
            /// set/instantiated once.</summary>
            internal static readonly ResourceManager instance = new ResourceManager();
        }

        #endregion //Singleton Pattern

        /// <summary>The current culture that is used to load the localized texts from the resource files.</summary>
        /// <permission cref="System.Security.PermissionSet">Everyone can access this method.</permission>
        /// <value>The reference to <see cref="CultureInfo"/> object.</value>
        /// <event cref="CultureChanged">The event is fired when culture is changed.</event>
        public Assembly CallingAssembly
        {
            get { return _assy; }
            set
            {
                try
                {
                    _assy = value;
                    _rm = new System.Resources.ResourceManager(string.Format("{0}.{1}.{1}", _assy.GetName().Name, @"Resources"), _assy);
                    // see if we succeeded
                    if (_rm == null)
                        throw new InvalidOperationException("Unable to create the ResourceManager.");
                }
                catch
                {
                    _assy = null;
                    _rm = null;
                    throw;
                }
            }
        }
        
        /// <summary>The current culture that is used to load the localized texts from the resource files.</summary>
        /// <permission cref="System.Security.PermissionSet">Everyone can access this method.</permission>
        /// <value>The reference to <see cref="CultureInfo"/> object.</value>
        /// <event cref="CultureChanged">The event is fired when culture is changed.</event>
        public CultureInfo Culture
        {
            get { return _ci; }
            set
            {
                // don't process the update unless
                // there was an actual culture change
                if (!_ci.Equals(value))
                {
                    _ci = value;
                    // raise the event so that any listeners can
                    // refresh/reload their resource values
                    if (this.CultureChanged != null)
                        this.CultureChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>Gets the string from the resource files using specified name.</summary>
        /// <param name='name'>The unique name of the localized text.</param>
        /// <returns>The text from the resouce file that is associated with custom name.</returns>
        public string GetString(string name)
        {
            if (_rm != null)
                return _rm.GetString(name, _ci);
            else
                throw new InvalidOperationException("The internal System.Resources.ResourceManager is null.  Make sure that the CallingAssembly property has a valid reference to the assembly being localized prior to calling this method.");
        }

        /// <summary>Gets the string from the resource files using the name of the specified control.</summary>
        /// <param name='c'>The reference to the control. The control name will be used to retrieve the 
        /// localized text.</param>
        /// <returns>The text from the resouce file that is associated with control name.</returns>
        public string GetString(Control c)
        {
            return GetString(GetResourceName(c));
        }

        /// <summary>Gets the object (typically <see cref="System.Drawing.Image"/>) from the resource files 
        /// using specified name.</summary>
        /// <param name='name'>The unique name of the localized object.</param>
        /// <returns>The object from the resouce file that is associated with custom name.</returns>
        public object GetObject(string name)
        {
            if (_rm != null)
                return _rm.GetObject(name, _ci);
            else
                throw new InvalidOperationException("The internal System.Resources.ResourceManager is null.  Make sure that the CallingAssembly property has a valid reference to the assembly being localized prior to calling this method.");
        }

        /// <summary>Gets the object (typically <see cref="System.Drawing.Image"/>) from the resource files 
        /// using the name of the specified control.</summary>
        /// <param name='c'>The reference to the control. The control name will be used to retrieve the 
        /// localized object.</param>
        /// <returns>The object from the resouce file that is associated with control name.</returns>
        public object GetObject(Control c)
        {
            return GetObject(GetResourceName(c));
        }

        /// <summary>Gets unique name for the control in the format -  Namespace_FormName_ControlName.</summary>
        /// <param name='c'>The reference to the control. </param>
        /// <returns>The unique name for the control.</returns>
        public string GetResourceName(Control c)
        {
            return GetResourceName(c.TopLevelControl.ToString(), c.Name);
        }

        /// <summary>Gets unique name for the custom name in the format -  Namespace_FormName_CustomName.</summary>
        /// <param name='topLevel'>The reference to the top level control. This control is used to retrieve 
        /// the namespace and form name parts of the returned name.</param>
        /// <param name="name">The custom name.</param>
        /// <returns>The unique name for the custom name.</returns>
        public string GetResourceName(Control topLevel, string name)
        {
            return GetResourceName(topLevel.ToString(), name);
        }

        /// <summary>Concatenates two parts of the unique name and replaces '.' charecters with '_'.</summary>
        /// <param name='topLevelName'>The first part of the unique name - typically holds namespace and form name.</param>
        /// <param name="name">The second part of the unique name - typically holds control name.</param>
        /// <returns>The unique name concatenated from two parts.</returns>
        public string GetResourceName(string topLevelName, string name)
        {
            // Get only last part from topLevelName
            // So the resulting name will be: FormName_ControlName
            string str = String.Empty;
            if (topLevelName.LastIndexOf('.') != -1)
                str = topLevelName.Substring((topLevelName.LastIndexOf('.') + 1), (topLevelName.Length - (topLevelName.LastIndexOf('.') + 1)));
            else
                str = String.Copy(topLevelName);

            return (string.Format("{0}.{1}", str, name)).Replace(".", "_");
        }
    }
}
