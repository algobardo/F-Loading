using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Xml;
using System.Web.UI;
using System.Drawing;
using System.Xml.Schema;

namespace Fields
{
	/// <summary>
	/// This class can repeat an xml schema element.
	/// </summary>
	public class RepeaterPanel : Panel, INamingContainer
	{
		private int itemCount = 1;
		private XmlSchemaSet schemas;
		private XmlSchemaElement itemToRepeat;
		private XmlNode renderingToRepeat;
		private int min;
		private int max;
		private Button remBtn, addBtn;

		private List<XmlNode> values = null;

		public RepeaterPanel(XmlSchemaSet schemas, XmlNode renderingToRepeat, XmlSchemaElement itemObj, List<XmlNode> value, int min, int max)
		{
			this.schemas = schemas;
			this.itemToRepeat = itemObj;
			this.renderingToRepeat = renderingToRepeat;
			this.BorderStyle = BorderStyle.Dashed;
			this.BorderColor = Color.FromArgb(167, 167, 167);
			this.BorderWidth = 1;
			//parte aggiunta
			this.min = min;
			if (this.itemCount < this.min)
				this.itemCount = this.min;

			this.max = max;
			//parte aggiunta
			if (value != null)
				itemCount = value.Count;
			this.values = value;
		}
		private RepeaterPanel() { }


		protected override void OnInit(EventArgs e)
		{
			/*Add the "Add" button, here we still have not the controlstate*/
			Page.RegisterRequiresControlState(this);
			base.OnInit(e);

			this.addBtn = new Button();
			addBtn.Text = "Add";
			addBtn.CausesValidation = false;
			addBtn.Click += new EventHandler(addBtn_Click);

			this.remBtn = new Button();
			remBtn.Text = "Remove";
			remBtn.CausesValidation = false;
			remBtn.Click += new EventHandler(remBtn_Click);

			Controls.Add(new LiteralControl("<br />"));
			Controls.Add(addBtn);
			Controls.Add(remBtn);

			// enable button appropriate
			if (itemCount == min)
				remBtn.Enabled = false;
			else
				remBtn.Enabled = true;

			if (itemCount == max)
				addBtn.Enabled = false;
			else
				addBtn.Enabled = true;
		}

		public List<Control> RepeatedItems()
		{
			/*WARNING: this method is based on the assumption that each Repeated item add 2 controls to the panel.
			 and we have 2 controls at the end (the add button and a <br>)*/
			int count = 0;
			List<Control> ls = new List<Control>();
			foreach (Control c in this.Controls)
			{
				if (count < Controls.Count - 3)
				{
					if (count % 2 == 0)
						ls.Add(c);
				}

				count++;
			}
			return ls;
		}

		protected override void OnLoad(EventArgs e)
		{
			/* OnLoad we have itemCount restored from controlState */
			for (int i = 0; i < itemCount; i++)
			{
				var res = addItem(i);				
			}
			if (itemCount == min)
				remBtn.Enabled = false;
			else
				remBtn.Enabled = true;
			if (itemCount == max)
				addBtn.Enabled = false;
			else
				addBtn.Enabled = true;
			
			base.OnLoad(e);
		}

		protected override void LoadControlState(object savedState)
		{
			itemCount = (int)savedState;
			base.LoadControlState(savedState);
		}


		void addBtn_Click(object sender, EventArgs e)
		{
			// Button bt = (Button)sender;
			if (this.itemCount < this.max)
			{
				var res = addItem(itemCount);

				this.itemCount++;
			}

			if (itemCount == min)
				remBtn.Enabled = false;
			else
				remBtn.Enabled = true;

			if (itemCount == max)
				addBtn.Enabled = false;
			else
				addBtn.Enabled = true;

		}

		void remBtn_Click(object sender, EventArgs e)
		{
			// Button bt = (Button)sender;
			if (this.itemCount > this.min)
			{
				remItem();
				this.itemCount--;
				//values.RemoveAt(values.Count - 1);
			}

			if (itemCount == min)
				remBtn.Enabled = false;
			else
				remBtn.Enabled = true;
			
			if (itemCount == max)
				addBtn.Enabled = false;
			else
				addBtn.Enabled = true;
		}

		private Control addItem(int i)
		{
			var field = FieldsManager.GetInstance(renderingToRepeat, itemToRepeat, schemas);

			/* Assigning the i-esimo node to the i-esimo field, if presents */
			if (values != null && i < values.Count && (values[i].SchemaInfo.SchemaElement.Name == itemToRepeat.Name))
			{
				var tmp = new List<XmlNode>();
				tmp.Add(values[i]);
				((IField)field).SetValue(tmp);
			}

			var ctr = field.GetWebControl(Page.Server, this.renderingToRepeat);
            ctr.ID = ctr.ID + i.ToString();
			/* This part adds the new controls in the end, BUT before the two controls added OnInit*/
			this.Controls.AddAt(this.Controls.Count - 3, ctr);
			this.Controls.AddAt(this.Controls.Count - 3, new LiteralControl("<br />"));

			return ctr;
		}

		private void remItem()
		{
			this.Controls.RemoveAt(this.Controls.Count - 4);
			this.Controls.RemoveAt(this.Controls.Count - 4);
		}

		protected override object SaveControlState()
		{
			return itemCount;
		}

	}
}
