using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;

namespace uNhAddIns.Pagination
{
	public abstract class AbstractPaginableQuery<T> : IPaginable<T>
	{
		protected abstract IDetachedQuery DetachedQuery { get;}

		private IList<T> InternalExecute(IDetachedQuery query)
		{
			ISession session = GetSession();
			if (session == null)
			{
				throw new ArgumentException("The NHibernate session is null; not available during pagination.");
			}
			return query.GetExecutableQuery(session).List<T>();
		}

		private static void ResetPagination(IDetachedQuery query)
		{
			if (query == null)
				throw new ArgumentNullException("query");

			query.SetFirstResult(default(int)).SetMaxResults(RowSelection.NoValue);
		}

		private static void SetPagination(IDetachedQuery query, int pageSize, int pageNumber)
		{
			if (query == null)
				throw new ArgumentNullException("query");
			query.SetFirstResult(pageSize * (pageNumber < 1 ? 0 : pageNumber - 1)).SetMaxResults(pageSize);
		}

		#region IPaginable<T> Members

		public abstract ISession GetSession();

		public IList<T> ListAll()
		{
			ResetPagination(DetachedQuery);
			return InternalExecute(DetachedQuery);
		}

		public virtual IList<T> GetPage(int pageSize, int pageNumber)
		{
			SetPagination(DetachedQuery, pageSize, pageNumber);
			return InternalExecute(DetachedQuery);
		}

		#endregion
	}
}