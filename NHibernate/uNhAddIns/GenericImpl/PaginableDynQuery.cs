using System;
using NHibernate;
using uNhAddIns.DynQuery;
using uNhAddIns.Pagination;

namespace uNhAddIns.GenericImpl
{
	/// <summary>
	/// Generic implementation of <see cref="IPaginable{T}"/> based on <see cref="DetachedDynQuery"/>.
	/// </summary>
	/// <typeparam name="T">The type of entity.</typeparam>
	/// <seealso cref="DetachedDynQuery"/>
	/// <remarks>
	/// A <see cref="DetachedDynQuery"/> can be used with <see cref="PaginableQuery{T}"/> too but
	/// with <see cref="PaginableDynQuery{T}"/> you can a query with "SELECT" and "ORDER BY" clauses
	/// because the <see cref="DetachedDynQuery"/> know each query part.
	/// </remarks>
	public class PaginableDynQuery<T> : AbstractPaginableDynQuery<T>
	{
		private readonly ISession session;

		public PaginableDynQuery(ISession session, DetachedDynQuery query) : base(query)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.session = session;
		}

		#region Overrides of AbstractPaginableQuery<T>

		public override ISession GetSession()
		{
			return session;
		}

		#endregion
	}
}