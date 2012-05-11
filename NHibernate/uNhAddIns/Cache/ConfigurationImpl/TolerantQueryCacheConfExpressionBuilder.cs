using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;

namespace uNhAddIns.Cache.ConfigurationImpl
{
	public class TolerantQueryCacheConfExpressionBuilder
	{
		private readonly HashSet<string> spaces = new HashSet<string>();

		public TolerantQueryCacheConfExpressionBuilder(string regionName)
		{
			RegionName = regionName;
			if (string.IsNullOrEmpty(regionName))
			{
				throw new ArgumentNullException("regionName");
			}
			QueryCache = typeof (StandardQueryCache);
		}

		public string RegionName { get; private set; }
		public Type QueryCache { get; private set; }

		public IEnumerable<string> SpacesTolerance
		{
			get { return spaces; }
		}

		public void SetRegionResolver<T>() where T : IQueryCache
		{
			QueryCache = typeof (T);
		}

		public void AddSpace(string spaceName)
		{
			if (!string.IsNullOrEmpty(spaceName))
			{
				spaces.Add(spaceName);
			}
		}

		public void AddSpaces(IEnumerable<string> spacesNames)
		{
			spaces.UnionWith(spacesNames.Where(x => !string.IsNullOrEmpty(x)));
		}
	}
}