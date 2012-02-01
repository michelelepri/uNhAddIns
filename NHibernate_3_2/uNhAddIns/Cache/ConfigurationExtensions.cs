using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using uNhAddIns.Cache.ConfigurationImpl;
using Environment=NHibernate.Cfg.Environment;

namespace uNhAddIns.Cache
{
	public static class ConfigurationExtensions
	{
		private const string QueryCacheResolverKeyPrefix = "uNhAddIns.Cache_";
		private const string QueryCacheToleranceKeyPrefix = "uNhAddIns.CacheTolerance_";
		private const string AlwaysTolerant = "AlwaysTolerant";

		public static IQueryCacheFactoryConfiguration QueryCache(this Configuration cfg)
		{
			cfg.SetProperty(Environment.QueryCacheFactory, typeof (RegionQueryCacheFactory).AssemblyQualifiedName);
			cfg.SetProperty(Environment.UseQueryCache, "true");
			return new QueryCacheFactoryConfiguration(cfg);
		}

		public static Type GetQueryCacheRegionResolver(this Configuration cfg, string regionName)
		{
			return Type.GetType(cfg.GetProperty(GetResolverConfigurationKey(regionName)));
		}

		public static IEnumerable<string> GetQueryCacheRegionTolerance(this Configuration cfg, string regionName)
		{
			return GetDisassembledSpacesString(cfg.GetProperty(GetToleranceConfigurationKey(regionName)));
		}

		internal static void SetQueryCacheRegionResolver(this Configuration cfg, string regionName, Type type)
		{
			cfg.SetProperty(GetResolverConfigurationKey(regionName), type.AssemblyQualifiedName);
		}

		internal static void SetQueryCacheRegionTolerance(this Configuration cfg, string regionName,
		                                                  IEnumerable<string> spaces)
		{
			cfg.SetProperty(GetToleranceConfigurationKey(regionName), GetAssembledSpacesString(spaces));
		}

		internal static void SetQueryCacheRegionAlwaysTolerant(this Configuration cfg, string regionName)
		{
			cfg.SetProperty(GetToleranceConfigurationKey(regionName), AlwaysTolerant);
		}

		private static string GetResolverConfigurationKey(string regionName)
		{
			return string.Concat(QueryCacheResolverKeyPrefix, regionName);
		}

		private static string GetToleranceConfigurationKey(string regionName)
		{
			return string.Concat(QueryCacheToleranceKeyPrefix, regionName);
		}

		private static string GetAssembledSpacesString(IEnumerable<string> spaces)
		{
			return string.Join(";", spaces.ToArray());
		}

		private static IEnumerable<string> GetDisassembledSpacesString(string assembledSpacesString)
		{
			return assembledSpacesString.Split(';');
		}

		public static Type GetQueryCacheRegionResolver(this IDictionary<string, string> properties, string regionName)
		{
			string key = GetResolverConfigurationKey(regionName);
			string value;
			return properties.TryGetValue(key, out value) ? Type.GetType(value) : null;
		}

		public static IEnumerable<string> GetQueryCacheRegionTolerance(this IDictionary<string, string> properties,
		                                                               string regionName)
		{
			string key = GetToleranceConfigurationKey(regionName);
			string value;
			return properties.TryGetValue(key, out value) ? GetDisassembledSpacesString(value) : new string[0];
		}

		public static bool IsQueryCacheRegionAlwaysTolerant(this IDictionary<string, string> properties,
																															 string regionName)
		{
			string key = GetToleranceConfigurationKey(regionName);
			string value;
			return properties.TryGetValue(key, out value) ? AlwaysTolerant.Equals(value) : false;
		}
	}
}