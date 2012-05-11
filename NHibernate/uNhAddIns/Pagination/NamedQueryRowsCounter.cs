using System;
using NHibernate;
using NHibernate.Impl;

namespace uNhAddIns.Pagination
{
	/// <summary>
	/// 
	/// </summary>
	public class NamedQueryRowsCounter : AbstractQueryRowsCounter
	{
		private readonly DetachedNamedQuery origin;
		private readonly DetachedNamedQuery detachedQuery;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryRowsCount"></param>
		public NamedQueryRowsCounter(string queryRowsCount)
		{
			if (string.IsNullOrEmpty(queryRowsCount))
			{
				throw new ArgumentNullException("queryRowsCount");
			}
			detachedQuery = new DetachedNamedQuery(queryRowsCount);
		}

		public NamedQueryRowsCounter(DetachedNamedQuery queryRowsCount)
		{
			if (queryRowsCount == null)
			{
				throw new ArgumentNullException("queryRowsCount");
			}

			detachedQuery = queryRowsCount;
		}

		public NamedQueryRowsCounter(string queryRowsCount, DetachedNamedQuery origin)
			: this(queryRowsCount)
		{
			this.origin = origin;
		}


		#region Overrides of AbstractQueryRowsCounter

		protected override IDetachedQuery GetDetachedQuery()
		{
			if (origin != null)
			{
				detachedQuery.CopyParametersFrom(origin);
			} 
			return detachedQuery;
		}

		#endregion
	}
}