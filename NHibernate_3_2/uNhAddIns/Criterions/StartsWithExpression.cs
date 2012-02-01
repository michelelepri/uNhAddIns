using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace uNhAddIns.Criterions
{
	public class StartsWithExpression : AbstractCriterion
	{
		private readonly ICriterion realCriterion;
		
		/// <summary>
		/// Initialize a new instance of the <see cref="EqOrNullExpression" /> class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public StartsWithExpression(string propertyName, object value)
		{
			realCriterion= new LikeExpression(propertyName, value.ToString(), MatchMode.Start);
		}

		public override string ToString()
		{
			return realCriterion.ToString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return realCriterion.GetTypedValues(criteria, criteriaQuery);
		}

		public override IProjection[] GetProjections()
		{
			return realCriterion.GetProjections();
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
		                                      IDictionary<string, IFilter> enabledFilters)
		{
			return realCriterion.ToSqlString(criteria, criteriaQuery, enabledFilters);
		}
	}
}
