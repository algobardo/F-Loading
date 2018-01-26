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
    /// This class represents a controller for the type <see cref="ChoiceBox"/>
    /// enables the user to select a single option from a group of choices.
    /// </summary>
    [ControllerAttribute(typeof(ChoiceBox))]
    public partial class ChoiceBoxController : FieldController
    {

        #region Layout Variables

        private int padding = 3;
        private static Color errorColor = Color.FromArgb(224, 0, 0);
        private bool enabledVal;
        
        #endregion
        
        #region Controls


        private ChoiceBox element;
        private Panel groupPanel;
        private RoundedPanel resultPanel;

        #endregion

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="element">The element representig the related <see cref="ChoiceBox"/></param>
        public ChoiceBoxController(ChoiceBox element)
            : this(element, true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceBoxController"/> class.
        /// </summary>
        /// <param name="element">The element representig the related <see cref="ChoiceBox"/></param>
        /// <param name="enabled">True if the <see cref="ChoiceBox"/> is modifiable, false otherwise</param>
        public ChoiceBoxController(ChoiceBox element, bool enabled) 
            : base(enabled)
        {
            this.element = element;
            enabledVal = enabled;

            if (enabled)
            {
                groupPanel = new Panel() { 
                    Height = 0,
                    Width = this.Width - (2 * padding)
                };
                Panel checksContainer = new Panel()
                {
                    Width = groupPanel.Width - 40,
                    Dock = DockStyle.Left
                };
                Panel editContainer = new Panel()
                {
                    Width = 40,
                    Dock = DockStyle.Right
                };

                groupPanel.Controls.Add(checksContainer);
                groupPanel.Controls.Add(editContainer);

                for (int i = 0; i < element.Fields.Length; i++)
                {
                    RadioButton button = new RadioButton()
                    {
                        Enabled = enabled,
                        Top = groupPanel.Height,
                        Font = new Font("Tahoma", 8, FontStyle.Regular),
                        Text = element.Fields[i].Name,
                        Width = checksContainer.Width
                    };
                    RoundedButton editButton = new RoundedButton()
                    {
                        Width = 38,
                        Height = button.Height - 2,
                        Top = groupPanel.Height,
                        Left = 1,
                        BackColor = Color.FromArgb(236, 236, 236),
                        BorderColor = Color.FromArgb(146, 146, 146),
                        ClickedBackColor = Color.FromArgb(186, 186, 186),
                        Text = "edit",
                        Font = new Font("Tahoma", 8, FontStyle.Regular),
                        ForeColor = Color.FromArgb(90, 90, 90),
                        BorderSize = 1,
                        CornerRadius = 4
                    };

                    groupPanel.Height += button.Height + padding;
                    
                    checksContainer.Controls.Add(button);
                    editContainer.Controls.Add(editButton);

                    button.CheckedChanged += OnCheckedChange;
                    editButton.Click += new EventHandler(OnEditButtonClick);

                    if (element.SelectedIndex == i)
                    {
                        button.Checked = true;
                        editButton.Visible = true;
                    }
                    else
                        editButton.Visible = false;
                }

                groupPanel.Height -= padding;

                Content = groupPanel;
                Title = element.Name;
                Description = element.Description;
            }
            else 
            {
                resultPanel = new RoundedPanel { 
                    BackColor = Color.Transparent,
                    BorderColor = Color.FromArgb(90, 90, 90),
                    BorderSize = 1,
                    CornerRadius = 4,
                    Dock = DockStyle.None,
                    Height = 0
                };

                int buttonMargin = padding;
                
                FieldController controller = FieldFactory.CreateController(element.Fields[element.SelectedIndex], false);
                
                RadioButton button = new RadioButton()
                {
                    Checked = true,
                    Enabled = enabled,
                    Font = new Font("Tahoma", 8, FontStyle.Regular),
                    Left = resultPanel.CornerRadius + 1,
                    Text = element.Fields[element.SelectedIndex].Name,                            
                    Top = buttonMargin
                };
     
                buttonMargin += button.Height;
                resultPanel.Controls.Add(button);

                (controller as Control).Top = buttonMargin;
                (controller as Control).Left = resultPanel.CornerRadius + 1;
                (controller as Control).Width = resultPanel.Width - (2 * (resultPanel.CornerRadius + 1));
                (controller as Control).Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                
                buttonMargin += (controller as Control).Height + padding;
                        
                resultPanel.Controls.Add(controller as Control);
                resultPanel.Height += buttonMargin + (2 * padding);

                Content = resultPanel;
                Title = element.Name;
                Description = element.Description;
            }
        }


        /// <summary>
        /// Manages the event raised when the user clicks on edit button.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnEditButtonClick(object sender, EventArgs e)
        {
            FieldController controller = FieldFactory.CreateController(element.Fields[element.SelectedIndex], true);
            OnSeparatedRequest(new SeparatedFieldEditingEventArgs(element.Fields[element.SelectedIndex].Name, controller));
        }


        public override void SetFocus() { }


        /// <summary>
        /// Manages the event raised when the user changes the selected element.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnCheckedChange(object sender, EventArgs e)
        {
            for(int i = 0; i < groupPanel.Controls[0].Controls.Count; i++)
            {
                RadioButton button = groupPanel.Controls[0].Controls[i] as RadioButton;

                if (button.Checked) 
                {
                    element.SelectedIndex = i;
                    groupPanel.Controls[1].Controls[i].Visible = true;
                }
                else
                    groupPanel.Controls[1].Controls[i].Visible = false;
            }
        }

        /// <summary>
        /// Override Control's class method
        /// </summary>
        /// <param name="e">The raised <see cref="EventArgs"/> element</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        /// <summary>
        /// Checks if the <see cref="ChoiceBox"/> that the controller manages
        /// respects the constraints defined into its own XML Schema catching 
        /// possibile exceptions. In this latter case, it highlights the 
        /// elements that they are not filled correctly.
        /// </summary>
        public override void CheckConstraints()
        {
            try
            {
                element.Fields[element.SelectedIndex].CheckConstraints();
                RestoreLayout();
            }
            catch (FieldException exc)
            {
                ShowErrorState();
                groupPanel.Controls[0].Controls[element.SelectedIndex].ForeColor = errorColor;
                throw new FieldException(SeverityLevel.Critical, LogLevel.Debug, exc.InnerException, "The highlighted elements are not filled correctly.");
            }
        }

        /// <summary>
        /// Restore the default layout of the <see cref="ChoiceBox"/> that the controller manages.
        /// </summary>
        protected void RestoreLayout()
        {
            ShowNormalState();
            groupPanel.Controls[0].Controls[element.SelectedIndex].ForeColor = Color.Black;
            Refresh();
        }
    }
}
