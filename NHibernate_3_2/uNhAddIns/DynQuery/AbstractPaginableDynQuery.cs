using System;
using NHibernate;
using uNhAddIns.Pagination;

namespace uNhAddIns.DynQuery
{
	public abstract class AbstractPaginableDynQuery<T> : AbstractPaginableRowsCounterQuery<T>
	{
		private readonly DetachedDynQuery query;

		protected AbstractPaginableDynQuery(DetachedDynQuery query)
		{
			if (query == null)
				throw new ArgumentNullException("query");

			this.query = query;
		}

		protected override IDetachedQuery GetRowCountQuery()
		{
			return query.TransformToRowCount();
		}

		protected override IDetachedQuery DetachedQuery
		{
			get { return query; }
		}
	}
}
