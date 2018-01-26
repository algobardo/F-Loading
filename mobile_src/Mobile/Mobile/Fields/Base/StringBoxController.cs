using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Mobile.Fields;
using Mobile.Util;
using Mobile.Util.Controls;

namespace Mobile.Fields.Base
{
    /// <summary>
    /// This class represent the control related to the <see cref="StringBox"/> object
    /// and all it's features
    /// </summary>
    [ControllerAttribute(typeof(StringBox))]
    public class StringBoxController : FieldController
    {
        /// <summary>
        /// Instantiate all variables used to represent a <see cref="StringBoxController"/>
        /// </summary>
        #region Controls

        private TextBox textbox;

        #endregion

        private StringBox element;

        ///<summary>
        /// Create a new instance of the <see cref="IntBoxController"/> class initializing
        /// the object state
        /// </summary>
        /// <param name="element">The element representig the related <see cref="StringBox"/></param>
        public StringBoxController(StringBox element)
            : this(element, true)
        { }

        /// <summary>
        /// Create a new instance of the <see cref="StringBoxController"/> class initializing
        /// the object state
        /// </summary>
        /// <param name="element">The element representig the related <see cref="IntBox"/></param>
        /// <param name="enabled">A boolean value, textbox is editable if true, disabled otherwise</param>
        public StringBoxController(StringBox element, bool enabled)
            : base(enabled)
        {
            this.element = element;

            textbox = new TextBox();
            textbox.Text = element.Value;
            textbox.Font = new Font("Tahoma", 8, FontStyle.Regular);

            //Events 
            textbox.GotFocus += OnGotFocus;
            textbox.LostFocus += OnLostFocus;

            // Enable/disable control's textbox according to the "enable" variable value
            textbox.Enabled = enabled;
            if (!textbox.Enabled)
            {
                textbox.Text = element.Value;
            }

            Content = textbox;
            Title = element.Name;
            Description = element.Description;

        }

        /// <summary>
        /// Set the focus on the related textbox
        /// </summary>
        public override void SetFocus()
        {
            textbox.Focus();
        }

        ///<summary>
        /// Check the constraints of the current element catching possibile
        /// exceptions related to this operation highlightning the error
        /// </summary>
        public override void CheckConstraints()
        {
            element.Value = textbox.Text.Trim();
            try
            {
                element.CheckConstraints();
                ShowNormalState();
            }
            catch (FieldException exc)
            {
                ShowErrorState();
                throw exc;
            }
        }

        /// <summary>
        /// Reinitialize the state of the <see cref="StringBoxController"/> in order to 
        /// update it
        /// </summary>

        #region Event Handlers

        /// <summary>
        /// Override Control's class method to manage OnResize event
        /// </summary>
        /// <param name="e">the <see cref="EventArgs"/> element</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        /// <summary>
        /// Set the correct borderColor of the control to evidentiate
        /// the lost of the focus
        /// </summary>
        /// <param name="sender">the object that raised the event</param>
        /// <param name="e">the <see cref="EventArgs"/> element</param>
        private void OnLostFocus(object sender, EventArgs e)
        {
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Set the correct BorderColor of the control to evidentiate
        /// the keep of the focus
        /// </summary>
        /// <param name="sender">the object that raised the event</param>
        /// <param name="e">the <see cref="EventArgs"/> element</param>
        private void OnGotFocus(object sender, EventArgs e)
        {
            base.OnGotFocus(e);
        }

        #endregion
    }
}
