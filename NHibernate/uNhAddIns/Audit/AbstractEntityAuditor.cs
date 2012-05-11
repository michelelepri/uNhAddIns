using System;
using NHibernate.Cfg;
using NHibernate.Event;

namespace uNhAddIns.Audit
{
	public abstract class AbstractEntityAuditor : IAuditor
	{
		protected AbstractEntityAuditor(string entityName, IAuditableMetaData meta)
		{
			if (entityName == null)
			{
				throw new ArgumentNullException("entityName");
			}
			if (meta == null)
			{
				throw new ArgumentNullException("meta");
			}
			EntityName = entityName;
			Meta = meta;
		}

		public string EntityName { get; private set; }
		public IAuditableMetaData Meta { get; private set; }

		#region IAuditor Members

		public abstract void Initialize(Configuration cfg);
		public abstract void Inserted(PostInsertEvent eventArgs);
		public abstract void Updated(PostUpdateEvent eventArgs);
		public abstract void Deleted(PostDeleteEvent eventArgs);

		#endregion
	}
}