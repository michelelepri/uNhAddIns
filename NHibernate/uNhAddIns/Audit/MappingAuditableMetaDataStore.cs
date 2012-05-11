using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace uNhAddIns.Audit
{
	public class MappingAuditableMetaDataStore : IAuditableMetaDataStore
	{
		private readonly Dictionary<string, IAuditableMetaData> store = new Dictionary<string, IAuditableMetaData>();

		public MappingAuditableMetaDataStore(Configuration cfg)
		{
			if (cfg == null)
			{
				throw new ArgumentNullException("cfg");
			}
			Cfg = cfg;
		}

		protected Configuration Cfg { get; private set; }

		public virtual bool RegisterAuditableEntityIfNeeded(string entityName)
		{
			PersistentClass pc = Cfg.GetClassMapping(entityName);
			if (pc == null)
			{
				return false;
			}
			return RegisterAuditableEntityIfNeeded(pc);
		}

		public bool RegisterAuditableEntityIfNeeded(PersistentClass pc)
		{
			string entityName = pc.EntityName;
			if (!pc.MetaAttributes.ContainsKey(GetAuditableClassMarker()))
			{
				return false;
			}
			var meta = new AuditableMetaData(entityName);
			string marker = GetAuditablePropertyMarker();
			meta.AddProperties(pc.PropertyIterator
			                   	.Where(p => p.MetaAttributes
			                   	            	.Where(ma => ma.Value.Name == marker && (!ma.Value.Values.Contains("false"))).Count() > 0)
			                   	.Select(p => p.Name));
			store.Add(entityName, meta);
			return true;
		}

		protected virtual string GetAuditableClassMarker()
		{
			return "Auditable";
		}

		protected virtual string GetAuditablePropertyMarker()
		{
			return "Auditable";
		}

		public bool Contains(string entityName)
		{
			if (string.IsNullOrEmpty(entityName))
			{
				return false;
			}
			return store.ContainsKey(entityName);
		}

		public virtual IAuditableMetaData GetAuditableMetaData(string entityName)
		{
			if (string.IsNullOrEmpty(entityName))
			{
				return null;
			}
			IAuditableMetaData result;
			store.TryGetValue(entityName, out result);
			return result;
		}
	}
}