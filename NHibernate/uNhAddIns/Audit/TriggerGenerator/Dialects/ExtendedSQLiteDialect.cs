using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect;

namespace uNhAddIns.Audit.TriggerGenerator.Dialects
{

  public class ExtendedSQLiteDialect
    : SQLiteDialect, IExtendedDialect 
  {

    public string GetCreateTriggerHeaderString(
      string triggerName,
      string dataTableName, 
      TriggerActions action)
    {
      var quotedTriggerName = IsQuoted(triggerName) ?
        triggerName : Quote(triggerName);
      var quotedTableName = QuoteForTableName(dataTableName);
      string actionName;
      switch (action)
      {
        case TriggerActions.INSERT:
          actionName = "INSERT";
          break;
        case TriggerActions.UPDATE:
          actionName = "UPDATE";
          break;
        case TriggerActions.DELETE:
          actionName = "DELETE";
          break;
        default:
          throw new NotSupportedException(
            string.Format("Trigger action {0} isn't supported by this dialect.", action));
      }

      return string.Format("create trigger {0} after {1} on {2}\nbegin",
        quotedTriggerName, actionName, quotedTableName);
    }

    public string GetCreateTriggerFooterString(string triggerName,
      string dataTableName, TriggerActions action)
    {
      return "end;";
    }

    public string GetDropTriggerString(string triggerName,
      string dataTableName, TriggerActions action)
    {
      return string.Format("drop trigger if exists {0};",
        triggerName);
    }

    public string GetInsertIntoString(string destTable,
      IEnumerable<string> columnNames, string sourceTable,
      IEnumerable<string> values)
    {
      var columnString = string.Join(", ", columnNames.ToArray());
      var valueString = string.Join(", ", values.ToArray());
      return string.Format(
        "insert into {0} ({1}) values ({2});",
        destTable, columnNames, values);
    }

    public string QualifyColumn(string tableName, string columnName)
    {
      return string.Format("{0}.{1}",
        QuoteForTableName(tableName),
        QuoteForColumnName(columnName));
    }

    public string GetTriggerNewDataAlias()
    {
      return "new";
    }

    public string GetTriggerOldDataAlias()
    {
      return "old";
    }

    public string QuoteForTriggerName(string triggerName)
    {
      return QuoteForTableName(triggerName);
    }

  }

}
