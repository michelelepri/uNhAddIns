using System.Collections.Generic;

namespace uNhAddIns.Audit
{
	public interface IAuditableMetaData
	{
		string EntityName { get; }
		IEnumerable<string> Propeties { get; }
	}
}