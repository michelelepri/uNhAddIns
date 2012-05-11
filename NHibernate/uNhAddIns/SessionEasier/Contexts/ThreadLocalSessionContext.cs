using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;

namespace uNhAddIns.SessionEasier.Contexts
{
	public class ThreadLocalSessionContext: CurrentSessionContext
	{
		[ThreadStatic]
		protected static IDictionary<ISessionFactory, ISession> context;

		public ThreadLocalSessionContext(ISessionFactoryImplementor factory) : base(factory) {}

		#region Overrides of AbstractCurrentSessionContext

		protected override IDictionary<ISessionFactory, ISession> GetContextDictionary()
		{
			return context;
		}

		protected override void SetContextDictionary(IDictionary<ISessionFactory, ISession> value)
		{
			context = value;
		}

		#endregion
	}
}