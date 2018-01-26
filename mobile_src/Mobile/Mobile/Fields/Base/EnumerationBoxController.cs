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
    /// This class represent the control related to the <see cref="EnumerationBox"/> object
    /// and all it's features
    /// </summary>
    [ControllerAttribute(typeof(EnumerationBox))]
    public partial class EnumerationBoxController : FieldController
    {
        /// <summary>
        /// Instantiate all variables used to represent a <see cref="EnumerationBoxController"/>
        /// </summary>
        #region Controls

        private ComboBox combo;

        #endregion
        ///<summary>
        /// Instantiate variables which represents the corrects spaces between the controller's elements
        /// </summary>
        #region Layout Variables

        private static Color focusColor = Color.FromArgb(0, 130, 221);
        private static Color unfocusColor = Color.FromArgb(106, 106, 106);
        private static Color errorColor = Color.FromArgb(224, 0, 0);

        #endregion

        private EnumerationBox element;

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="element">The element representig the related <see cref="EnumerationBox"/></param>
        public EnumerationBoxController(EnumerationBox element)
            : this(element, true)
        { }

        /// <summary>
        /// Create a new instance of the <see cref="EnumerationBoxController"/> class initializing
        /// the object state
        /// </summary>
        /// <param name="element">The element representig the related <see cref="EnumerationBox"/></param>
        /// <param name="enabled">A boolean value, ComboBox is enabled if true, disabled otherwise</param>
        public EnumerationBoxController(EnumerationBox element, bool enabled)
            : base(enabled)
        {
            this.element = element;
                      
            combo = new ComboBox();
            foreach (String item in element.Values)
                combo.Items.Add(item);

            combo.SelectedIndex = element.SelectedIndex;
            combo.Font = new Font("Tahoma", 8, FontStyle.Regular);
            combo.Enabled = enabled;

            Content = combo;
            Title = element.Name;
            Description = element.Description;
        }

        /// <summary>
        /// Set the focus on the related comboBox
        /// </summary>
        public override void SetFocus()
        {
            combo.Focus();
        }

        /// <summary>
        /// Check the constraints of the current element catching possibile
        /// exceptions related to this operation highlightning the error
        /// </summary>
        public override void CheckConstraints()
        {
            element.SelectedIndex = combo.SelectedIndex;
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

        #region Event Handlers

        /// <summary>
        /// Set the correct BackGround color of the control to evidentiate
        /// the lost of the focus
        /// </summary>
        /// <param name="sender">the object that raised the event</param>
        /// <param name="e">the <see cref="EventArgs"/> element</param>
        private void OnLostFocus(object sender, EventArgs e)
        {
            combo.BackColor = unfocusColor;
        }

        /// <summary>
        /// Set the correct BackGround color of the control to evidentiate
        /// the keep of the focus
        /// </summary>
        /// <param name="sender">the object that raised the event</param>
        /// <param name="e">the <see cref="EventArgs"/> element</param>
        private void OnGotFocus(object sender, EventArgs e)
        {
            combo.BackColor = focusColor;
        }

        #endregion
    }
}
