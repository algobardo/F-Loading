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
    /// This class represent the control related to the <see cref="RatingBox"/> object
    /// and all it's features
    /// </summary>
    [ControllerAttribute(typeof(RatingBox))]
    public partial class RatingBoxController : FieldController
    {
        /// <summary>
        /// Instantiates all variables used to represent a <see cref="RatingBox"/>
        /// </summary>
        #region Controls

        private StarRating starRating;
        private RatingBox element;
        
        #endregion

        ///<summary>
        /// Instantiates variables which represents the corrects spaces between the controller's elements
        /// </summary>
        #region Layout Variables

        private static Color unfocusColor = Color.FromArgb(106, 106, 106);
        private static Color errorColor = Color.FromArgb(224, 0, 0);

        #endregion

        
        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="element">The element representig the related <see cref="RatingBox"/></param>
        public RatingBoxController(RatingBox element)
            : this(element, true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingBoxController"/> class.
        /// </summary>
        /// <param name="element">The element representig the related <see cref="RatingBox"/></param>
        /// <param name="enabled">True if the <see cref="RatingBox"/> is modifiable, false otherwise</param>
        public RatingBoxController(RatingBox element, bool enabled)
            : base(enabled)
        {
            this.element = element;

            starRating = new StarRating(element.Values.Length, enabled);
            starRating.SelectedIndex = element.SelectedIndex - 1;

            Content = starRating;
            Title = element.Name;
            Description = element.Description;
        }

        /// <summary>
        /// Sets the focus on the related StarRating control
        /// </summary>
        public override void SetFocus()
        {
            starRating.Focus();
        }

        /// <summary>
        /// Checks the constraints of the current element catching possibile
        /// exceptions related to this operation and highlightning the error
        /// </summary>
        public override void CheckConstraints()
        {
            element.SelectedIndex = starRating.SelectedIndex + 1;
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

    }
}
