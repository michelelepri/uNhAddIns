using NHibernate.Cache;
namespace uNhAddIns.Cache
{
	public interface IQueryCacheRegionResolver
	{
		ITolerantQueryCacheConfiguration Using<T>() where T: IQueryCache;
	}
}