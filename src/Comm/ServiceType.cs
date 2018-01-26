
using System;

namespace Comm
{
    public class ServiceType
    {
        private string name;
        private string scheme;

        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }

        public string Scheme
        {
            get { return scheme; }
            set { this.scheme = value; }
        }

        public ServiceType(string name, string scheme)
        {
            this.name = name;
            this.scheme = scheme;
        }

        public ServiceType(string name)
            : this(name, "")
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            ServiceType other = obj as ServiceType;
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
            return name + " = " + name + " (type: " + scheme + ")";
        }
    }
}
