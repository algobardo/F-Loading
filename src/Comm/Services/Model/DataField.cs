using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comm.Services.Model
{
    public class DataField
    {
        private string name;
        private string regexp;
        private string text;

        public string Name
        {
            get { return name; }
        }

        public string Type
        {
            get { return regexp; }
        }

        public string Text 
        {
            set { this.text = value; }
        }

        public DataField(string name, string regexp)
        {
            this.name = name;
            this.regexp = regexp;
        }
        
        public DataField(string name)
        {
            this.name = name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            DataField other = obj as DataField;
            if (other == null)
                return false;

            return other.name.Equals(this.name);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public override string ToString()
        {
            return name + " = " + text + " (type: " + regexp + ")";
        }
    }
}
