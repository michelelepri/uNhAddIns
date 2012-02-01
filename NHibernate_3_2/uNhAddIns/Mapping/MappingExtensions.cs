using NHibernate.Mapping;
using uNhAddIns.Serialization;

namespace uNhAddIns.Mapping
{
	public static class MappingExtensions
	{
		public static PersistentClass Clone(this PersistentClass persistentClass)
		{
			return (PersistentClass) Cloner.Clone(persistentClass);
		}
	}
}