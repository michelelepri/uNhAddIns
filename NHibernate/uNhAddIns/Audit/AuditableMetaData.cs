using System;
using System.Collections.Generic;

namespace uNhAddIns.Audit
{
	public class AuditableMetaData : IAuditableMetaData
	{
		private readonly HashSet<string> properties = new HashSet<string>();
		private readonly string entityName;
		private readonly int hashCode;

		public AuditableMetaData(string entityName)
		{
			if (string.IsNullOrEmpty(entityName))
			{
				throw new ArgumentNullException("entityName");
			}
			this.entityName = entityName;
			hashCode = entityName.GetHashCode();
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public IEnumerable<string> Propeties
		{
			get { return properties; }
		}

		public void AddProperties(IEnumerable<string> propertiesNames)
		{
			properties.UnionWith(propertiesNames);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as IAuditableMetaData);
		}

		public bool Equals(IAuditableMetaData other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.EntityName, entityName);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}