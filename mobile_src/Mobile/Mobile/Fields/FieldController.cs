using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Mobile.Util.Controls;
using Mobile.Util;

namespace Mobile.Fields
{
    public class ControllerAttribute : Attribute
    {
        public Type FieldType
        {
            get;
            private set;
        }

        public ControllerAttribute(Type fieldType)
            : base()
        {
            this.FieldType = fieldType;
        }
    }

    public class SeparatedFieldEditingEventArgs : EventArgs
    {
        public SeparatedFieldEditingEventArgs(String displayInfo, params FieldController[] controllers)
            : base()
        {
            this.Controllers = controllers;
            this.DisplayInfo = displayInfo;
        }

        public FieldController[] Controllers
        {
            get;
            private set;
        }

        public String DisplayInfo
        {
            get;
            private set;
        }
    }

    public class FieldController : Control
    {
        #region private variables

        private static readonly Color errorColor = Color.FromArgb(224, 0, 0);

        private Label titleLabel, descriptionLabel;
        private RoundedPanel errorPanel;

        private bool enabled;
        private String title;
        private String description;
        private Control content;

        #endregion

        public event EventHandler<SeparatedFieldEditingEventArgs> SeparatedEditingRequest;

        /// <summary>
        /// The container of the Field that the controller manages.
        /// </summary>
        protected Control Content
        {
            get
            {
                return content;
            }
            set
            {
                if (content != null)
                    errorPanel.Controls.Remove(content);

                content = value;
                if (content != null)
                    errorPanel.Controls.Add(content);

                UpdateLayout();
            }
        }

        /// <summary>
        /// The label of the Field to display.
        /// </summary>
        protected String Title
        {
            get 
            {
                return title;
            }
            set 
            {
                title = value;
                titleLabel.Text = title;
                UpdateLayout();
            }
        }

        /// <summary>
        /// The description of the Field to display.
        /// </summary>
        protected String Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                if(enabled)
                    descriptionLabel.Text = description;
                UpdateLayout();
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FieldController"/> class.
        /// </summary>
        /// <param name="enabled">True if the <see cref="FieldController"/> is modifiable, false otherwise</param>
        public FieldController(bool enabled) : base()
        {
            this.enabled = enabled;

            titleLabel = new Label() 
            {
                Text = Title,
                BackColor = Color.Transparent,
                Font = new Font("Tahoma", 8, FontStyle.Bold)
            };

            errorPanel = new RoundedPanel()
            {
                BorderColor = Color.White,
                BorderSize = 1,
                BackColor = Color.Transparent
            };

            errorPanel.Controls.Add(titleLabel);
            errorPanel.Controls.Add(Content);
            Controls.Add(errorPanel);

            if (enabled)
            {
                descriptionLabel = new Label()
                {
                    Text = Description,
                    Font = new Font("Tahoma", 8, FontStyle.Regular),
                    ForeColor = Color.FromArgb(90, 90, 90)
                };
                Controls.Add(descriptionLabel);
            }

            UpdateLayout();
        }


        /// <summary>
        /// Updates the layout of the Field that the controller manages.
        /// </summary>
        protected virtual void UpdateLayout()
        {
            titleLabel.Top = 3;
            titleLabel.Width = this.Width - (2 * 3);
            titleLabel.Left = 3;
            titleLabel.Height = StringMeasure.Measure(titleLabel, titleLabel.Text, titleLabel.ClientRectangle).Height;

            if (Content != null)
            {
                Content.Width = this.Width - (2 * 3);
                Content.Left = 3;
                Content.Top = titleLabel.Top + titleLabel.Height + 3;
            }

            errorPanel.Width = this.Width;
            errorPanel.Height = Content != null? Content.Top + Content.Height + 3 : 0;

            if (enabled)
            {
                descriptionLabel.Width = this.Width - (2 * 3);
                descriptionLabel.Left = 3;
                descriptionLabel.Top = errorPanel.Top + errorPanel.Height + 2;
                descriptionLabel.Height = StringMeasure.Measure(descriptionLabel, descriptionLabel.Text, descriptionLabel.ClientRectangle).Height;

                this.Height = descriptionLabel.Height + errorPanel.Height + 3;
                if (String.IsNullOrEmpty(descriptionLabel.Text))
                    this.Height -= 3;
            }
            else
                this.Height = errorPanel.Height;
        }


        /// <summary>
        /// Manages the <see cref="SeparatedFieldEditingEventArgs"/> event
        /// to display and edit the control separately
        /// </summary>
        /// <param name="e">The raised event.</param>
        protected void OnSeparatedRequest(SeparatedFieldEditingEventArgs e)
        {
            if (SeparatedEditingRequest != null)
                SeparatedEditingRequest(this, e);
        }

        
        /// <summary>
        /// 
        /// </summary>
        public virtual void CheckConstraints()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void SetFocus()
        {
        }


        /// <summary>
        /// Override Control's class method
        /// </summary>
        /// <param name="e">the <see cref="EventArgs"/> element</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }


        /// <summary>
        /// Highlights the element that it's not filled correctly.
        /// </summary>
        protected virtual void ShowErrorState()
        {
            titleLabel.ForeColor = errorColor;
            errorPanel.BorderColor = errorColor;
            errorPanel.BackColor = Color.White;
            errorPanel.Refresh();
        }


        /// <summary>
        /// Restore the color state of the element.
        /// </summary>
        protected virtual void ShowNormalState()
        {
            titleLabel.ForeColor = Color.Black;
            errorPanel.BackColor = Color.Transparent;
            errorPanel.BorderColor = Color.Transparent;
            errorPanel.Refresh();
        }
    }

}
