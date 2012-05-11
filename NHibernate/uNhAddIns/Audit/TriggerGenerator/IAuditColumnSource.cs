using System.Collections.Generic;
using NHibernate.Mapping;
using uNhAddIns.Audit.TriggerGenerator.Dialects;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public interface IAuditColumnSource
  {

    IEnumerable<AuditColumn> GetAuditColumns(
      Table auditTable, IExtendedDialect dialect);

  }

}
