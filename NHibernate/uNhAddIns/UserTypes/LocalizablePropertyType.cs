using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using uNhAddIns.Extensions;

namespace uNhAddIns.UserTypes
{
	[Serializable]
	public class LocalizablePropertyType : IUserType, IParameterizedType
	{
		public const char DefaultKeyValueEncloser = '~';
		private char keyValueEncloser = DefaultKeyValueEncloser;
		protected string KeyValueEncloserParameterName = "keyValueEncloser";

		private int length = 560;
		protected string LengthParameterName = "length";

		#region IParameterizedType Members

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			if (parameters == null)
			{
				return;
			}

			string userSeparator;
			if (parameters.TryGetValue(KeyValueEncloserParameterName, out userSeparator))
			{
				if (!string.IsNullOrEmpty(userSeparator))
				{
					keyValueEncloser = userSeparator[0];
				}
			}
			string userLength;
			if (parameters.TryGetValue(LengthParameterName, out userLength))
			{
				length = int.Parse(userLength);
			}
		}

		#endregion

		#region IUserType Members

		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(null, x) && ReferenceEquals(null, y))
			{
				return true;
			}
			if (ReferenceEquals(null, x) || ReferenceEquals(null, y))
			{
				return false;
			}
			var convertedX = ((IDictionary<CultureInfo, string>)x).ToString(DefaultKeyValueEncloser);
			var convertedY = ((IDictionary<CultureInfo, string>)y).ToString(DefaultKeyValueEncloser);

			return convertedX.Equals(convertedY);
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
			int ordinal = rs.GetOrdinal(names[0]);
			if (rs.IsDBNull(ordinal))
			{
				return null;
			}
			string savedString = rs.GetString(ordinal);
			if (!string.IsNullOrEmpty(savedString))
			{
				return savedString.SplitByEncloser(keyValueEncloser).ToPairs().ToDictionary(kv => new CultureInfo(kv.Key),
																																										kv => kv.Value);
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
				((IDbDataParameter)cmd.Parameters[index]).Value = ((IDictionary<CultureInfo, string>)value).ToString(DefaultKeyValueEncloser);
			}
		}

		public object DeepCopy(object value)
		{
			return value == null ? null : new Dictionary<CultureInfo, string>((IDictionary<CultureInfo, string>)value);
		}

		public object Replace(object original, object target, object owner)
		{
			return DeepCopy(original);
		}

		public object Assemble(object cached, object owner)
		{
			return DeepCopy(cached);
		}

		public object Disassemble(object value)
		{
			return DeepCopy(value);
		}

		public SqlType[] SqlTypes
		{
			get { return new SqlType[] { SqlTypeFactory.GetString(length) }; }
		}

		public Type ReturnedType
		{
			get { return typeof(IDictionary<CultureInfo, string>); }
		}

		public bool IsMutable
		{
			get { return true; }
		}

		#endregion
	}
}