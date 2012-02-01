using NHibernate.Mapping;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public interface INamingStrategy
  {

    string GetAuditTableName(Table dataTable);
    string GetTriggerName(Table dataTable, TriggerActions action);

  }

}
