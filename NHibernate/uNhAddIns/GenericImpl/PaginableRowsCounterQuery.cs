using System;
using NHibernate;
using NHibernate.Impl;
using uNhAddIns.Pagination;

namespace uNhAddIns.GenericImpl
{
	/// <summary>
	/// Generic implementation of <see cref="IPaginable{T}"/> and <see cref="IRowsCounter"/> 
	/// based on <see cref="NHibernate.Impl.DetachedQuery"/>.
	/// </summary>
	/// <typeparam name="T">The type of DAO.</typeparam>
	/// <seealso cref="NHibernate.Impl.DetachedQuery"/>
	/// <remarks>
	/// Use this class only if you are secure that the DetachedQuery is based on a HQL that can be trasformed
	/// to it's row count.
	/// An HQL is supported if contain only 'from' clause and/or 'where' clause.
	/// Any other clause throw an exception.
	/// </remarks>
	public class PaginableRowsCounterQuery<T> : AbstractPaginableRowsCounterQuery<T>
	{
		private readonly ISession session;
		private readonly DetachedQuery detachedQuery;
		public PaginableRowsCounterQuery(ISession session, DetachedQuery detachedQuery)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (detachedQuery == null)
			{
				throw new ArgumentNullException("detachedQuery");
			}
			this.session = session;
			this.detachedQuery = detachedQuery;
		}

		protected override IDetachedQuery DetachedQuery
		{
			get { return detachedQuery; }
		}

		public override ISession GetSession()
		{
			return session;
		}

		protected override IDetachedQuery GetRowCountQuery()
		{
			if (!detachedQuery.Hql.StartsWith("from", StringComparison.InvariantCultureIgnoreCase))
				throw new HibernateException(string.Format("Can't trasform the HQL to it's counter, the query must start with 'from' clause:{0}", detachedQuery.Hql));
			var result = new DetachedQuery("select count(*) " + detachedQuery.Hql);
			result.CopyParametersFrom(detachedQuery);
			return result;
		}
	}
}