using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mobile.Communication;
using Mobile.Util.Controls;
using Mobile.Language;

namespace Mobile
{
    /// <summary>
    /// Shows the forms that has been downloaded and not still filled
    /// </summary>
    public partial class RecentScreen : LocalizedForm
    {
        #region Translate
        private string cancelButton = ResourceManager.Instance.GetString("RecentScreen_Delete");
        private string closeButton = ResourceManager.Instance.GetString("CloseButton");
        private int leftbutton;
        private enum ActionButton { Cancel, Close };
        public enum FormAction { Delete, Fill };
        #endregion
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<FormSelectionEventArgs> FormSelected;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> CancelFilling;

        #endregion
        #region Protected
        protected override void AddMenusToDictionary()
        {
            // this is necessary because the menu items do not
            // derive from Control and have no Name property or
            // any other way to uniquely identify them at runtime
            //
            // the fact that these string values happen to match
            // the initial display text of the menus is incidental
            // and is not necessary for the localization functionality
            // to perform as expected
            // this.AddMenuToDictionary(this.backButton, "backButton");
            this.AddMenuToDictionary(this.fillButton, "fillButton");
            this.AddMenuToDictionary(this.Delete, "Delete");
            this.AddMenuToDictionary(this.optionButton, "optionButton");


        }
        #endregion
        #region Private

        private static readonly int baseOffset = 0;
        private int offset;
        private int selected;
        private FormInfo[] dataSets;

        private void OnRowClick(object sender, EventArgs e)
        {
            for (int i = 0; i < contentPanel.Controls.Count; i++)
            {
                RoundedControl child = contentPanel.Controls[i] as RoundedControl;
                if (sender == child)
                {
                    if (selected != i)
                    {
                        AdornUnselectedRow(selected);
                        AdornSelectedRow(i);
                        selected = i;
                    }
                }
            }
        }

        private void AdornSelectedRow(int index)
        {
            (contentPanel.Controls[index] as RoundedControl).BorderSize = 1;
            //(contentPanel.Controls[index] as EnhancedControl).BackColor = Color.FromArgb(232, 232, 232);
        }

        private void AdornUnselectedRow(int index)
        {
            (contentPanel.Controls[index] as RoundedControl).BorderSize = 0;
            (contentPanel.Controls[index] as RoundedControl).BackColor = Color.Transparent;
        }

        private void OnFillButtonClick(object sender, EventArgs e)
        {
            optionButton.Enabled = false;
            leftbutton = (int)ActionButton.Cancel;
            backButton.Text = cancelButton;
            if (FormSelected != null)
                FormSelected(this, new FormSelectionEventArgs(FormAction.Fill, dataSets[selected].RequestInfo));
        }

        private void OnCloseAdviceButtonClick(object sender, EventArgs e)
        {
            if (leftbutton == (int)ActionButton.Cancel)
            {
                CancelFilling(this, EventArgs.Empty);
                leftbutton = (int)ActionButton.Close;
                backButton.Text = closeButton;
                optionButton.Enabled = true;
            }
            else
                this.Hide();
        }

        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            if (FormSelected != null)
                FormSelected(this, new FormSelectionEventArgs(FormAction.Delete, dataSets[selected].RequestInfo));
        }

        private void RecentScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                try
                {
                    OnRowClick(contentPanel.Controls[selected - 1], null);
                }
                catch { }
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                try
                {
                    OnRowClick(contentPanel.Controls[selected + 1], null);
                }
                catch { }
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                OnFillButtonClick(this, null);
            }

        }

        private void OnCheckMouseDown(object sender, MouseEventArgs e)
        {

        }

        #endregion

       
        /// <summary>
        /// Provides informations about the form selected
        /// </summary>
        public class FormSelectionEventArgs : EventArgs
        {
            /// <summary>
            /// Provides informations about the form selected
            /// </summary>
            /// <param name="action"></param>
            /// <param name="info"></param>
            public FormSelectionEventArgs(FormAction action, FormRequestInfo info)
                : base()
            {
                this.Action = action;
                this.RequestInfo = info;
            }

            /// <summary>
            /// Can be "Fill" or "Delete"
            /// </summary>
            public FormAction Action
            {
                get;
                private set;
            }

            /// <summary>
            /// Request informations 
            /// </summary>
            public FormRequestInfo RequestInfo
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Creates a new instance of RecentScreen
        /// </summary>
        public RecentScreen()
        {
            InitializeComponent();
            offset = baseOffset;
            selected = -1;
            leftbutton = (int)ActionButton.Close;
            backButton.Text = closeButton;
            backgroundPanel.BackgroundImage = Mobile.Properties.Resources.DefaultBackground;
            backgroundPanel.SizeXMode = Mobile.Util.Controls.SizeMode.Stretch;
            backgroundPanel.VerticalAlignment = Mobile.Util.Controls.VerticalAlignment.Bottom;
            //header.BackgroundImage = Mobile.Properties.Resources.RecentHeader;
            infoPanel.BackgroundImage = Mobile.Properties.Resources.InformationPanel;
            infoPanel.IconImage = Mobile.Properties.Resources.RecentFH;
            this.AddMenusToDictionary();
            this.UpdateResources();
        }

        /// <summary>
        /// Shows the forms recently downloaded
        /// </summary>
        /// <param name="infos"></param>
        public void ShowRecentForms(FormInfo[] infos)
        {
            //Clear rows
            foreach (Control child in contentPanel.Controls)
            {
                child.Click -= OnRowClick;
                child.Dispose();
            }
            contentPanel.Controls.Clear();
            offset = baseOffset;

            dataSets = infos;
            foreach (FormInfo info in dataSets)
            {
                RoundedControl row = new RoundedControl()
                {
                    Width = contentPanel.Width,
                    Height = 40,
                    CornerRadius = 4,
                    BorderSize = 0,
                    BorderColor = Color.FromArgb(146,146,146),
                    BackColor = Color.Transparent,
                    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
                };
                row.Top = offset;
                row.Click += OnRowClick;

                offset += row.Height;

                Image iconImage = Mobile.Properties.Resources.FormNotifiedIcon;
                if(info.Status == FormStatus.Downloaded)
                    iconImage = Mobile.Properties.Resources.FormDownloadedIcon;
                if(info.Status == FormStatus.Opened)
                    iconImage = Mobile.Properties.Resources.FormOpenedIcon;

                PictureBox icon = new PictureBox()
                {
                    Image = iconImage,
                    Width = 20,
                    Height = 20,
                    Location = new Point(5,10),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };
                Label nameLabel = new Label()
                {
                    Text = info.Name == null? "(" + info.RequestInfo.CompilationRequestId + ")" : info.Name,
                    Height = 16,
                    Width = 90,
                    Location = new Point(30, 5),
                    Font = new Font("Tahoma", 8, FontStyle.Bold)
                };
                Label sourceLabel = new Label()
                {
                    Text = info.Source == null ? "No source" : info.Source,
                    Height = 16,
                    Width = 90,
                    Location = new Point(30, 21),
                    Font = new Font("Tahoma", 8, FontStyle.Regular)
                };
                Label dateLabel = new Label()
                {
                    Text = info.DownloadTime.Year == 1 ? "" : info.DownloadTime.ToString(ResourceManager.Instance.Culture),
                    Height = 16,
                    Width = 80,
                    TextAlign = ContentAlignment.TopRight,
                    Location = new Point(row.Width - 85, 5),
                    Font = new Font("Tahoma", 8, FontStyle.Regular),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                row.Controls.Add(icon);
                row.Controls.Add(nameLabel);
                row.Controls.Add(sourceLabel);
                row.Controls.Add(dateLabel);

                contentPanel.Controls.Add(row);
            }

            if (infos.Length > 0)
            {
                selected = 0;
                AdornSelectedRow(0);
            }
        }
    }
}