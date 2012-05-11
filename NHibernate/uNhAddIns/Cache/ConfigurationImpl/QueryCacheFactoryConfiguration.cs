using System;
using NHibernate.Cache;
using NHibernate.Cfg;

namespace uNhAddIns.Cache.ConfigurationImpl
{
	public class QueryCacheFactoryConfiguration : IQueryCacheFactoryConfiguration, IQueryCacheRegionResolver,
	                                              ITolerantQueryCacheConfiguration
	{
		private readonly Configuration cfg;
		private TolerantQueryCacheConfExpressionBuilder tqcEb;

		public QueryCacheFactoryConfiguration(Configuration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			cfg = configuration;
		}

		#region IQueryCacheFactoryConfiguration Members

		public IQueryCacheRegionResolver ResolveRegion(string regionName)
		{
			tqcEb = new TolerantQueryCacheConfExpressionBuilder(regionName);
			return this;
		}

		#endregion

		#region IQueryCacheRegionResolver Members

		public ITolerantQueryCacheConfiguration Using<T>() where T : IQueryCache
		{
			tqcEb.SetRegionResolver<T>();
			cfg.SetQueryCacheRegionResolver(tqcEb.RegionName, tqcEb.QueryCache);
			return this;
		}

		#endregion

		#region ITolerantQueryCacheConfiguration Members

		public void TolerantWith(string querySpace)
		{
			tqcEb.AddSpace(querySpace);
			cfg.SetQueryCacheRegionTolerance(tqcEb.RegionName, tqcEb.SpacesTolerance);
		}

		public void TolerantWith(params string[] querySpaces)
		{
			tqcEb.AddSpaces(querySpaces);
			cfg.SetQueryCacheRegionTolerance(tqcEb.RegionName, tqcEb.SpacesTolerance);
		}

		public void AlwaysTolerant()
		{
			cfg.SetQueryCacheRegionAlwaysTolerant(tqcEb.RegionName);
		}

		#endregion
	}
}