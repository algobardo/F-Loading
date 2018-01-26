using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using Mobile.Fields;
using Mobile.Util;
using Mobile.Util.Controls;
using System.Runtime.InteropServices;
using Mobile.Language;

namespace Mobile.Fields.Group
{
    /// <summary>
    /// This class represents a controller for the type <see cref="SequenceBox"/>.
    /// </summary>
    [ControllerAttribute(typeof(SequenceBox))]
    public partial class SequenceBoxController : FieldController
    {
        #region Fields

        private SequenceBox element;
        private SequenceBox cache;

        private Panel buttonPanel;
        private RoundedButton createButton;
        private RoundedButton deleteButton;
        private ListView list;
        private RoundedControl mainPanel;
        private static Color errorColor = Color.FromArgb(224, 0, 0);

        private static readonly int rowHeight = 20;

        #endregion
        #region Translate
        private String ExitString = ResourceManager.Instance.GetString("ExitButton");

        private String MoveString = ResourceManager.Instance.GetString("Move");
        private String UpString = ResourceManager.Instance.GetString("Up");
        private String DownString = ResourceManager.Instance.GetString("Down");
        private String ViewString = ResourceManager.Instance.GetString("View");
        private String EditString = ResourceManager.Instance.GetString("Edit");
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceBoxController2"/> class.
        /// </summary>
        /// <param name="element">The element representig the related <see cref="SequenceBoxController2"/></param>
        /// <param name="enabled">True if the <see cref="SequenceBoxController2"/> is modifiable, false otherwise</param>
        public SequenceBoxController(SequenceBox element, bool enabled)
            : base(enabled)
        {
            this.element = element;
            cache = new SequenceBox(element.TypeName, element.FieldName, element.Schema);
            cache.ParsePresentation(element.FieldsPresentation);

            mainPanel = new RoundedControl()
            {
                BorderColor = Color.FromArgb(146, 146, 146),
                BorderSize = 1,
                CornerRadius = 4,
                BackColor = Color.FromArgb(235, 235, 235)
            };

            list = new ListView()
            {
                View = View.Details,
                CheckBoxes = enabled,
                Height = 120,
                Font = new Font("Tahoma", 8, FontStyle.Regular)
            };
            list.Columns.Add("Item", 150, System.Windows.Forms.HorizontalAlignment.Left);
            list.FullRowSelect = true;
            list.HeaderStyle = ColumnHeaderStyle.None;
            list.Activation = ItemActivation.OneClick;

            foreach (Field child in element.Fields)
            {
                cache.AddNew();
                cache.Fields[cache.Fields.Length - 1].FromXml(child.ToXml());
                ((cache.Fields[cache.Fields.Length - 1]) as GroupBox).Annotation = (child as GroupBox).Annotation;

                list.Items.Add(new ListViewItem(((cache.Fields[cache.Fields.Length - 1]) as GroupBox).Annotation));
            }

            if (enabled)
            {
                createButton = new RoundedButton()
                {
                    Width = 38,
                    Height = 18,
                    Top = 0,
                    Left = 3,
                    BackColor = Color.FromArgb(236, 236, 236),
                    BorderColor = Color.FromArgb(146, 146, 146),
                    ClickedBackColor = Color.FromArgb(186, 186, 186),
                    Text = ResourceManager.Instance.GetString("New"),
                    Font = new Font("Tahoma", 8, FontStyle.Regular),
                    ForeColor = Color.FromArgb(90, 90, 90),
                    BorderSize = 1,
                    CornerRadius = 4
                };
                createButton.Click += OnNewButtonClick;

                deleteButton = new RoundedButton()
                {
                    Width = 44,
                    Height = 18,
                    Top = 0,
                    Left = 1,
                    BackColor = Color.FromArgb(236, 236, 236),
                    BorderColor = Color.FromArgb(146, 146, 146),
                    ClickedBackColor = Color.FromArgb(186, 186, 186),
                    Text = ResourceManager.Instance.GetString("delete"),
                    Font = new Font("Tahoma", 8, FontStyle.Regular),
                    ForeColor = Color.FromArgb(90, 90, 90),
                    BorderSize = 1,
                    CornerRadius = 4
                };
                deleteButton.Click += OnDeleteButtonClick;

                buttonPanel = new Panel()
                {
                    Height = deleteButton.Height,
                    BackColor = Color.FromArgb(228, 228, 228)
                };
                buttonPanel.Controls.Add(deleteButton);
                buttonPanel.Controls.Add(createButton);
                buttonPanel.Location = new Point(mainPanel.BorderSize + mainPanel.CornerRadius, mainPanel.BorderSize + 4);

                createButton.Location = new Point(0, 0);
                deleteButton.Location = new Point(createButton.Width + 3, 0);

                mainPanel.Controls.Add(buttonPanel);
            }

            if (enabled)
                list.Location = new Point(mainPanel.BorderSize + mainPanel.CornerRadius, buttonPanel.Top + buttonPanel.Height + 4);
            else
                list.Location = new Point(mainPanel.BorderSize + mainPanel.CornerRadius, mainPanel.BorderSize + mainPanel.CornerRadius);

            mainPanel.Controls.Add(list);

            //Create context menu
            ContextMenu menu = new ContextMenu();
            if (enabled)
            {
                MenuItem deleteItem = new MenuItem()
                {
                    Text = ExitString
                };
                MenuItem editItem = new MenuItem()
                {
                    Text = EditString
                };
                MenuItem sortItem = new MenuItem()
                {
                    Text = MoveString
                };
                MenuItem sortUpItem = new MenuItem()
                {
                    Text = UpString
                };
                MenuItem sortDownItem = new MenuItem()
                {
                    Text = DownString
                };

                sortItem.MenuItems.Add(sortUpItem);
                sortItem.MenuItems.Add(sortDownItem);
                menu.MenuItems.Add(editItem);
                menu.MenuItems.Add(deleteItem);
                menu.MenuItems.Add(sortItem);

                editItem.Click += OnEditMenuItemClick;
                deleteItem.Click += OnDeleteMenuItemClick;
                sortUpItem.Click += OnSortUpMenuItemClick;
                sortDownItem.Click += OnSortDownMenuItemClick;

            }
            else
            {
                MenuItem viewItem = new MenuItem()
                {
                    Text = ViewString
                };
                menu.MenuItems.Add(viewItem);

                viewItem.Click += OnViewMenuItemClick;
            }

            mainPanel.Top = mainPanel.BorderSize;
            mainPanel.Height = list.Height + (buttonPanel == null ? 0 : buttonPanel.Height) + (2 * mainPanel.BorderSize) + 12;
            this.Height = mainPanel.Height + (2 * mainPanel.BorderSize);
            mainPanel.Width = this.Width;
            list.Width = mainPanel.Width - 2 * (mainPanel.BorderSize + mainPanel.CornerRadius);

            list.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;

            if (buttonPanel != null)
            {
                buttonPanel.Width = mainPanel.Width - 2 * (mainPanel.BorderSize + mainPanel.CornerRadius);
                buttonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }

            list.ContextMenu = menu;
            list.SelectedIndexChanged += OnSelectedIndexChange;
            list.Columns[0].Width = list.Width - 4;
            mainPanel.Resize += OnPanelSizeChanged;

            Content = mainPanel;
            Title = element.Name;
            Description = element.Description;
        }

        #endregion
        #region Public Methods
        
        public override void SetFocus()
        {
        }

        protected void OnPanelSizeChanged(object source, EventArgs e)
        {
            list.Columns[0].Width = list.Width - 4;
        }

        /// <summary>
        /// Checks if the <see cref="SequenceBox"/> that the controller manages
        /// respects the constraints defined into its own XML Schema catching 
        /// possibile exceptions. In this latter case, it highlights the 
        /// elements that they are not filled correctly.
        /// </summary>
        public override void CheckConstraints()
        {
            element.Clear();
            foreach (Field field in cache.Fields)
            {
                element.AddNew();
                element.Fields[element.Fields.Length - 1].FromXml(field.ToXml());
                (element.Fields[element.Fields.Length - 1] as GroupBox).Annotation = String.Copy((field as GroupBox).Annotation);
            }

            int indexItem = -1;
            try
            {
                for (int i = 0; i < element.Fields.Length; i++)
                {
                    indexItem = i;
                    element.Fields[i].CheckConstraints();
                    RestoreLayout(indexItem);
                }

                element.CheckConstraints();
            }
            catch (FieldException exc)
            {
                ShowErrorState();

                if (indexItem >= 0 && indexItem < element.Fields.Length)
                    (list.Items[indexItem] as ListViewItem).ForeColor = errorColor;

                Refresh();
                throw exc;
            }
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Restore the default layout of the <see cref="SequenceBox"/> that the controller manages.
        /// </summary>
        protected void RestoreLayout(int index)
        {
            ShowNormalState();

            if (index >= 0 && index < element.Fields.Length)
                (list.Items[index] as ListViewItem).ForeColor = Color.Black;

            Refresh();
        }


        /// <summary>
        /// Manages the event raised when the user adds a new element to the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnNewButtonClick(object sender, EventArgs e)
        {
            cache.AddNew();
            (cache.Fields[cache.Fields.Length - 1] as GroupBox).Annotation = element.Name + " " + (list.Items.Count + 1).ToString("00");

            list.Items.Add(new ListViewItem((cache.Fields[cache.Fields.Length - 1] as GroupBox).Annotation));
        }


        /// <summary>
        /// Manages the event raised when the user deletes an element from the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            for (int i = list.Items.Count - 1; i >= 0; i--)
            {
                if (list.Items[i].Checked)
                {
                    cache.Remove(i);
                    list.Items.RemoveAt(i);
                }
            }
        }

        #endregion
        #region Event Handlers

        /// <summary>
        /// Manages the event raised when the user clicks on edit menu of an element of the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnEditMenuItemClick(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count > 0)
            {
                if(cache.Fields[list.SelectedIndices[0]] is GroupBox) 
                {
                    List<FieldController> controllers = new List<FieldController>();
                    foreach (Field subfield in (cache.Fields[list.SelectedIndices[0]] as FieldGroup).Fields)
                    {
                        FieldController controller = FieldFactory.CreateController(subfield, true);
                        controllers.Add(controller);
                    }
                    OnSeparatedRequest(new SeparatedFieldEditingEventArgs(Name, controllers.ToArray()));
                }
                else
                    OnSeparatedRequest(new SeparatedFieldEditingEventArgs(Name, FieldFactory.CreateController(cache.Fields[list.SelectedIndices[0]], true)));
            }
        }

        /// <summary>
        /// Manages the event raised when the user clicks on delete menu of an element of the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnDeleteMenuItemClick(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count > 0)
            {
                cache.Remove(list.SelectedIndices[0]);
                list.Items.RemoveAt(list.SelectedIndices[0]);
            }
        }

        /// <summary>
        /// Manages the event raised when the user clicks on up menu of an element of the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnSortUpMenuItemClick(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count > 0)
            {
                int index = list.SelectedIndices[0];
                if (index > 0)
                {
                    ListViewItem item = list.Items[index];
                    list.Items.RemoveAt(index);
                    list.Items.Insert(index - 1, item);

                    Field field = cache.Fields[index];
                    cache.Move(index, index - 1);
                }
            }
        }

        /// <summary>
        /// Manages the event raised when the user clicks on down menu of an element of the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnSortDownMenuItemClick(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count > 0)
            {
                int index = list.SelectedIndices[0];
                if (index < list.Items.Count - 1)
                {
                    ListViewItem item = list.Items[index];
                    list.Items.RemoveAt(index);
                    list.Items.Insert(index + 1, item);

                    Field field = cache.Fields[index];
                    cache.Move(index + 1, index);
                }
            }
        }

        /// <summary>
        /// Manages the event raised when the user selects an element of the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count > 0)
            {
                Point position = new Point(0, 0);
                if (GetCursorPos(ref position))
                    list.ContextMenu.Show(list, list.PointToClient(position));
                else
                    list.ContextMenu.Show(list, new Point(list.Width / 2, (list.SelectedIndices[0] + 1) * rowHeight - rowHeight / 2));
            }
            else
                list.ContextMenu.Dispose();

        }

        /// <summary>
        /// Manages the event raised when the user clicks on view menu of an element of the list.
        /// </summary>
        /// <param name="sender">The sender of the raised event.</param>
        /// <param name="e">The data of the raised event.</param>
        private void OnViewMenuItemClick(object sender, EventArgs e)
        {
            if (list.SelectedIndices.Count > 0)
            {
                if (cache.Fields[list.SelectedIndices[0]] is GroupBox)
                {
                    List<FieldController> controllers = new List<FieldController>();
                    foreach (Field subfield in (cache.Fields[list.SelectedIndices[0]] as FieldGroup).Fields)
                    {
                        FieldController controller = FieldFactory.CreateController(subfield, false);
                        controllers.Add(controller);
                    }
                    OnSeparatedRequest(new SeparatedFieldEditingEventArgs(Name, controllers.ToArray()));
                }
                else
                    OnSeparatedRequest(new SeparatedFieldEditingEventArgs(Name, FieldFactory.CreateController(cache.Fields[list.SelectedIndices[0]],false)));
            }
        }

        #endregion

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool GetCursorPos(ref Point lpPoint);
    }
}