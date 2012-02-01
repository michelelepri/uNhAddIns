using System;
using NHibernate.Mapping;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public class NamingStrategy
    : INamingStrategy 
  {

    public string GetAuditTableName(Table dataTable)
    {
      var unquotedName = dataTable.Name;
      var format = dataTable.IsQuoted ? "`{0}Audit`" : "{0}Audit";
      return string.Format(format, unquotedName);
    }

    public string GetTriggerName(Table dataTable, TriggerActions action)
    {
      var unquotedName = dataTable.Name;
      string actionString;
      switch (action)
      {
        case TriggerActions.INSERT:
          actionString = "Insert";
          break;
        case TriggerActions.UPDATE:
          actionString = "Update";
          break;
        case TriggerActions.DELETE:
          actionString = "Delete";
          break;
        default:
          throw new ArgumentOutOfRangeException("action");
      }
      return string.Format("{0}_on{1}", unquotedName, actionString);
    }


  }

}
