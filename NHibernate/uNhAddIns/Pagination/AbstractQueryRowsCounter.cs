using System;
using NHibernate;
using NHibernate.Impl;

namespace uNhAddIns.Pagination
{
	/// <summary>
	/// The <see cref="AbstractQueryRowsCounter"/> object is a base class used for HQL based Row counters. 
	/// </summary>
	/// <seealso cref="IRowsCounter"/>
	public abstract class AbstractQueryRowsCounter: IRowsCounter
	{
		/// <summary>
		/// Get the <see cref="IDetachedQuery"/> representing the query for rowcount.
		/// </summary>
		/// <returns>The the query for rowcount.</returns>
		protected abstract IDetachedQuery GetDetachedQuery();

		#region IRowsCounter Members

		/// <summary>
		/// Get the row count.
		/// </summary>
		/// <param name="session">The <see cref="ISession"/>.</param>
		/// <returns>The row count.</returns>
		public long GetRowsCount(ISession session)
		{
			IQuery q = GetDetachedQuery().GetExecutableQuery(session);
			try
			{
				return q.UniqueResult<long>();
			}
			catch (Exception e)
			{
				throw new HibernateException(string.Format("Invalid RowsCounter query:{0}", q.QueryString), e);
			}
		}

		#endregion
	}
}