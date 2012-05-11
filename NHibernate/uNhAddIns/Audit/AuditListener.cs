using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Event;

namespace uNhAddIns.Audit
{
	public class AuditListener : IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener, IInitializable
	{
		private readonly IAuditorsFactory auditorsFactory;
		private readonly Dictionary<string, IAuditor> auditors = new Dictionary<string, IAuditor>();
		private IAuditableMetaDataStore store;

		public AuditListener(IAuditorsFactory auditorsFactory)
		{
			this.auditorsFactory = auditorsFactory;
		}

		protected IAuditorsFactory AuditorsFactory
		{
			get { return auditorsFactory; }
		}

		protected IAuditableMetaDataStore Store
		{
			get { return store; }
			set { store = value; }
		}

		public Dictionary<string, IAuditor> Auditors
		{
			get { return auditors; }
		}

		public virtual void Initialize(Configuration cfg)
		{
			var classMappings = cfg.ClassMappings.ToList();
			var mstore = new MappingAuditableMetaDataStore(cfg);
			foreach (var classMapping in classMappings)
			{
				if(mstore.RegisterAuditableEntityIfNeeded(classMapping))
				{
					var entityName = classMapping.EntityName;
					var auditor = auditorsFactory.CreateAuditor(entityName, store.GetAuditableMetaData(entityName));
					auditor.Initialize(cfg);
					auditors[entityName] = auditor;
				}
			}
			Store = mstore;
		}

		public void OnPostInsert(PostInsertEvent @event)
		{
			IAuditor auditor;
			auditors.TryGetValue(@event.Persister.EntityName, out auditor);
			if (auditor == null)
			{
				return;
			}
			auditor.Inserted(@event);
		}

		public void OnPostUpdate(PostUpdateEvent @event)
		{
			IAuditor auditor;
			auditors.TryGetValue(@event.Persister.EntityName, out auditor);
			if (auditor == null)
			{
				return;
			}
			auditor.Updated(@event);
		}

		public void OnPostDelete(PostDeleteEvent @event)
		{
			IAuditor auditor;
			auditors.TryGetValue(@event.Persister.EntityName, out auditor);
			if (auditor == null)
			{
				return;
			}
			auditor.Deleted(@event);
		}
	}
}