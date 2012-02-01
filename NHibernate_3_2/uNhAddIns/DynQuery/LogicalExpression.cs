using System;
using System.Text;

namespace uNhAddIns.DynQuery
{
	public enum		LogicalOperator
	{
		Null,
		And,
		Or,
		Not
	}

	[Serializable]
	public class LogicalExpression: IQueryPart
	{
		private readonly LogicalOperator loperator;
		private readonly string expression;

		internal LogicalExpression(LogicalOperator loperator, string expression)
		{
			this.loperator = loperator;
			this.expression = expression.Trim();
		}

		public LogicalExpression(string expression) : this(LogicalOperator.Null, expression) {}


	  #region IQueryPart Members

		/// <summary>
		/// The query complete clause.
		/// </summary>
		public string Clause
		{
			get
			{
				StringBuilder result = new StringBuilder(200);
				switch (loperator)
				{
					case LogicalOperator.And:
						result.Append(" and ").Append(Expression);
						break;
					case LogicalOperator.Or:
						result.Append(" or ").Append(Expression);
						break;
					case LogicalOperator.Not:
						result.Append(" and ").Append(Expression);
						break;
					case LogicalOperator.Null:
					default:
						result.Append(Expression);
						break;
				}
				return result.ToString();
			}
		}

		/// <summary>
		/// The query part.
		/// </summary>
		public string Expression
		{
			get
			{
				if (loperator == LogicalOperator.Not)
					return string.Format("not ({0})", expression);
				return string.Format("({0})", expression);
			}
		}

		#endregion
	}
}
