using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Mobile.Fields;
using Mobile.Util;
using Mobile.Util.Controls;

namespace Mobile.Fields.Base
{
    /// <summary>
    /// This class represent the control related to the <see cref="CalendarBox"/> object
    /// and all it's features
    /// </summary>
    [ControllerAttribute(typeof(CalendarBox))]
    public class CalendarBoxController : FieldController
    {
        /// <summary>
        /// Instantiate all variables used to represent a <see cref="CalendarBoxController"/>
        /// </summary>
        #region Controls

        private CalendarBox element;
        private DateTimePicker datePicker;

        #endregion

        /// <summary>
        /// Overloaded contructor
        /// </summary>
        /// <param name="element">The element representig the related <see cref="CalendarBox"/>
        public CalendarBoxController(CalendarBox element) : this(element, true) { }

        /// <summary>
        /// Create a new instance of the <see cref="CalendarBoxController"/> class initializing
        /// the object state
        /// </summary>
        /// <param name="element">The element representig the related <see cref="CalendarBox"/></param>
        /// <param name="enabled">A boolean value, DatePicker is enabled if true, disabled otherwise</param>
        public CalendarBoxController(CalendarBox element, bool enabled)
            : base(enabled)
        {
            this.element = element;

            datePicker = new DateTimePicker();
            if (element.Value != null)
                datePicker.Value = element.Value.Value;

            datePicker.Font = new Font("Tahoma", 8, FontStyle.Regular);
            datePicker.CalendarFont = new Font("Tahoma", 8, FontStyle.Regular);
            datePicker.Format = DateTimePickerFormat.Short;
            //datePicker.CustomFormat = "dd/MM/yyyy";

            datePicker.Enabled = enabled;

            Content = datePicker;
            Title = element.Name;
            Description = element.Description;
        }

        /// <summary>
        /// Set the focus on the related element's DatePicker
        /// </summary>
        public override void SetFocus()
        {
            datePicker.Focus();
        }

        /// <summary>
        /// Reinitialize the state of the <see cref="CalendarBoxController"/> in order to 
        /// update it
        /// </summary>


        /// <summary>
        /// Check the constraints of the current element catching possibile
        /// exceptions related to this operation highlightning the error
        /// </summary>
        public override void CheckConstraints()
        {
            element.Value = datePicker.Value;
            try
            {
                element.CheckConstraints();
            }
            catch (FieldException exc)
            {
                ShowErrorState();
                throw exc;
            }
        }
    }
}
