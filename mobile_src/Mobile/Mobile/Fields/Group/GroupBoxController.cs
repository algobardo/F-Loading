using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Mobile.Fields;
using Mobile.Util.Controls;

namespace Mobile.Fields.Group
{
    /// <summary>
    /// This class represents a controller for the type <see cref="GroupBox"/>.
    /// </summary>
    [ControllerAttribute(typeof(GroupBox))]
    public class GroupBoxController : FieldController
    {
        private GroupBox element;
        private List<FieldController> controllers;
        private EnhancedGroupBox groupPanel;


        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="element">The element representig the related <see cref="GroupBox"/></param>
        public GroupBoxController(GroupBox element)
            : this(element, true)
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBoxController"/> class.
        /// </summary>
        /// <param name="element">The element representig the related <see cref="GroupBox"/></param>
        /// <param name="enabled">True if the <see cref="GroupBox"/> is modifiable, false otherwise</param>
        public GroupBoxController(GroupBox element, bool enabled) 
            : base(enabled)
        {
            groupPanel = new EnhancedGroupBox
            {
                HeaderText = element.Name,
                HeaderFont = new Font("Tahoma", 8, FontStyle.Italic),
                BorderColor = Color.Black,
                CornerRadius = 5,
                BorderSize = 1
            };
            this.element = element;
            controllers = new List<FieldController>();

            if (!String.IsNullOrEmpty(element.Description))
                groupPanel.AddElement(new Label() { Text = element.Description, ForeColor = Color.FromArgb(90, 90, 90), Font = new Font("Tahoma", 8, FontStyle.Regular) });

            foreach (Field field in element.Fields)
            {
                FieldController controller = FieldFactory.CreateController(field, enabled);
                controllers.Add(controller);
                controller.SeparatedEditingRequest += OnInnerSeparatedRequest;
                groupPanel.AddElement(controller as Control);
            }

            Content = groupPanel;
        }

        /// <summary>
        /// Manages the event raised when the controllers must be display in 
        /// a different page.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnInnerSeparatedRequest(object sender, SeparatedFieldEditingEventArgs e)
        {
            OnSeparatedRequest(e);
        }


        /// <summary>
        /// 
        /// </summary>
        public override void SetFocus() { }


        /// <summary>
        /// Checks if the <see cref="GroupBox"/> that the controller manages
        /// respects the constraints defined into its own XML Schema catching 
        /// possibile exceptions. In this latter case, it highlights the 
        /// elements that they are not filled correctly.
        /// </summary>
        public override void CheckConstraints()
        {
            try
            {
                foreach (FieldController controller in controllers)
                    controller.CheckConstraints();

                element.CheckConstraints();
            }
            catch (FieldException ex)
            {
                throw new FieldException(SeverityLevel.Critical, LogLevel.Debug, ex.InnerException, "The highlighted elements are not filled right.");
            }
        }
    }
}
