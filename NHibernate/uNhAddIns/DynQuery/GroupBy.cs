using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Util;

namespace uNhAddIns.DynQuery
{
	public class GroupBy : IDynClause
	{
		private From owner;
		public GroupBy() {}
		public GroupBy(From owner)
		{
			this.owner = owner;
		}

		internal void SetOwner(From fromClause)
		{
			owner = fromClause;
		}

		private readonly HashedSet<string> groups = new HashedSet<string>();

		public GroupBy Add(string propertyPath)
		{
			if (string.IsNullOrEmpty(propertyPath))
				throw new ArgumentNullException("propertyPath");
			string pp = propertyPath.Trim();
			if (pp.Length == 0)
				throw new ArgumentNullException("propertyPath");
			groups.Add(pp);
			return this;
		}

		public OrderBy OrderBy(string propertyPath)
		{
			return OrderBy(propertyPath, false);
		}

		public OrderBy OrderBy(string propertyPath, bool isDescending)
		{
			if (owner == null)
				throw new InvalidOperationException("Can't set OrderBy without associate the GroupBy to a From clause.");
			return owner.OrderBy().Add(propertyPath, isDescending);
		}

		public string Clause
		{
			get { return (!HasMembers) ? string.Empty : string.Format("group by {0}", Expression); }
		}

		public string Expression
		{
			get { return GetExpression(); }
		}

		private string GetExpression()
		{
			if (!HasMembers) return string.Empty;
			var clause = new StringBuilder(groups.Count * 32 + 9);

			IEnumerator<string> iter = groups.GetEnumerator();
			iter.MoveNext();
			clause.Append(iter.Current);
			while (iter.MoveNext())
				clause.Append(StringHelper.CommaSpace).Append(iter.Current);

			return clause.ToString();
		}

		public bool HasMembers
		{
			get { return !groups.IsEmpty; }
		}

	}
}