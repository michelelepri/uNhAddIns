using System;
using NHibernate.Mapping;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public class AuditColumn : Column
  {

    public bool IncludeInPrimaryKey { get; set; }

    public Func<TriggerActions, string> ValueFunction { get; set; }

  }

}
