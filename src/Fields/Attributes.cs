using System;
using System.Text;

namespace Fields
{
	/// <summary>
	/// Definition of the Constraint attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class ConstraintAttribute : Attribute
	{
		public ConstraintAttribute(string consName)
		{
			Name = consName;
		}
		public string Description { get; set; }
		public string Name { get; set; }
	}

	/// <summary>
	/// Definition of the Operation attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class OperationAttribute : Attribute
	{
		public OperationAttribute(string opName)
		{
			Name = opName;
		}
		public string Description { get; set; }
		public string Name { get; set; }
	}


	/// <summary>
	/// Definition of the Predicate attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class PredicateAttribute : Attribute
	{
		public PredicateAttribute(string predName)
		{
			Name = predName;
		}
		public string Description { get; set; }
		public string Name { get; set; }
	}

	/// <summary>
	/// Definition of the Property attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class PropertyAttribute : Attribute
	{
		public PropertyAttribute(string consName)
		{
			Name = consName;
		}
		public string Description { get; set; }

		public string Attribute { get; set; }

		public string Default { get; set; }

		public string Name { get; set; }
	}

}
