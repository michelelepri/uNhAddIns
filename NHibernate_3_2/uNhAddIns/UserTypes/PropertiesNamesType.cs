using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using uNhAddIns.Extensions;

namespace uNhAddIns.UserTypes
{
	public class PropertiesNamesType : IUserType, IParameterizedType
	{
		protected string SeparatorParameterName = "separator";
		protected string LengthParameterName = "length";
		private char separator = ';';
		private int length = 500;

		#region Implementation of IUserType

		public new bool Equals(object x, object y)
		{
			if(ReferenceEquals(x,y))
			{
				return true;
			}
			if(ReferenceEquals(null,x) || ReferenceEquals(null,y))
			{
				return false;
			}
			return x.Equals(y);
		}

		public int GetHashCode(object x)
		{
			if (x == null)
			{
				throw new ArgumentNullException("x");
			}
			return x.GetHashCode();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			var ordinal = rs.GetOrdinal(names[0]);
			if(rs.IsDBNull(ordinal))
			{
				return null;
			}
			var savedString = rs.GetString(ordinal);
			if (!string.IsNullOrEmpty(savedString))
			{
				return new HashSet<string>(savedString.Split(separator));
			}
			return null;
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			if (value == null)
			{
				((IDbDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
			}
			else
			{
				((IDbDataParameter) cmd.Parameters[index]).Value = ((IEnumerable<string>) value).ConcatWithSeparator(separator);
			}
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}

		public SqlType[] SqlTypes
		{
			get { return new SqlType[] { SqlTypeFactory.GetString(length) }; }
		}

		public Type ReturnedType
		{
			get { return typeof(HashSet<string>); }
		}

		public bool IsMutable
		{
			get { return false; }
		}

		#endregion

		#region Implementation of IParameterizedType

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			if (parameters == null)
			{
				return;
			}

			string userSeparator;
			if(parameters.TryGetValue(SeparatorParameterName,out userSeparator))
			{
				if (!string.IsNullOrEmpty(userSeparator))
				{
					separator = userSeparator[0];
				}
			}
			string userLength;
			if (parameters.TryGetValue(LengthParameterName, out userLength))
			{
				length = int.Parse(userLength);
			}
		}

		#endregion
	}
}