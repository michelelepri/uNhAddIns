using System;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Util;

namespace uNhAddIns.Listeners.AutoDirtyCheck
{
	[Serializable]
	public class ResetReadOnlyEntityDeleteListener : IDeleteEventListener
	{
		public void OnDelete(DeleteEvent @event)
		{
			OnDelete(@event, new IdentitySet());
		}

		public void OnDelete(DeleteEvent @event, ISet transientEntities)
		{
			var session = @event.Session;
			EntityEntry entry = session.PersistenceContext.GetEntry(@event.Entity);
			if (entry != null && entry.Persister.IsMutable && entry.Status == Status.ReadOnly)
			{
				entry.BackSetStatus(Status.Loaded);
			}
		}
	}
}