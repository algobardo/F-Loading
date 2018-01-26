using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Mobile.Fields;
using Mobile.Util.Controls;
using Mobile.Util;

namespace Mobile.Fields.Base
{
    /// <summary>
    /// This class represent the control related to the <see cref="BoolBox"/> object
    /// and all it's features
    /// </summary>
    [ControllerAttribute(typeof(BoolBox))]
    public class BoolBoxController : FieldController
    {
        /// <summary>
        /// Instantiate all variables used to represent a <see cref="BoolBoxController"/>
        /// </summary>
        #region Controls

        private CheckBox checkbox;

        #endregion

        private BoolBox element;

        /// <summary>
        /// Create a new instance of the <see cref="BoolBoxController"/> class initializing
        /// the object state
        /// </summary>
        /// <param name="element">The element representig the related <see cref="BoolBox"/></param>
        /// <param name="enabled">A boolean value, check box is editable if true, disabled otherwise</param>
        public BoolBoxController(BoolBox element, bool enabled)
            : base(enabled)
        {
            this.element = element;

            checkbox = new CheckBox();
            checkbox.Checked = element.Value == null ? false : element.Value.Value;
                        
            // Enable/disable control's textbox according to the "enable" variable value
            checkbox.Enabled = enabled;
            
            Content = checkbox;
            Title = element.Name;
            Description = element.Description;
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="element">The element representig the related <see cref="BoolBox"/></param>
        public BoolBoxController(BoolBox element)
            : this(element, true)
        { }

        /// <summary>
        /// Set the focus on the related checkbox
        /// </summary>
        public override void SetFocus()
        {
            checkbox.Focus();
        }

        /// <summary>
        /// Reinitialize the state of the <see cref="BoolBoxController"/> in order to 
        /// update it
        /// </summary>
        

        /// <summary>
        /// Check the constraints of the current element catching possibile
        /// exceptions related to this operation highlightning the error
        /// </summary>
        public override void CheckConstraints()
        {
            try
            {
                element.Value = checkbox.Checked;
                element.CheckConstraints();
            }
            catch (FieldException exc)
            {
                ShowErrorState();
                throw exc;
            }
        }

        #region Event Handlers

        /// <summary>
        /// Override Control's class method
        /// </summary>
        /// <param name="e">the <see cref="EventArgs"/> element</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        #endregion
    }
}
