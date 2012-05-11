namespace uNhAddIns.Cache
{
	public interface ITolerantQueryCacheConfiguration
	{
		void TolerantWith(string querySpace);
		void TolerantWith(params string[] querySpace);
		void AlwaysTolerant();
	}
}