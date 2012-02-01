namespace uNhAddIns.Audit
{
	public interface IAuditorsFactory
	{
		IAuditor CreateAuditor(string entityName, IAuditableMetaData metaData);
	}
}