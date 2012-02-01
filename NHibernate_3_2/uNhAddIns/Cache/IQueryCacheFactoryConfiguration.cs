namespace uNhAddIns.Cache
{
	public interface IQueryCacheFactoryConfiguration
	{
		IQueryCacheRegionResolver ResolveRegion(string regionName);
	}
}