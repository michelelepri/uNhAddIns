using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace uNhAddIns.Audit.TriggerGenerator.Dialects
{

  public class ExtendedMsSql2008Dialect : 
    MsSql2008Dialect, IExtendedDialect 
  {

    public ExtendedMsSql2008Dialect()
    {
      RegisterFunction("current_user",
        new NoArgSQLFunction(
          "system_user",
          NHibernateUtil.AnsiString, false));
    }

    public string GetCreateTriggerHeaderString(
      string triggerName,
      string dataTableName, 
      TriggerActions action)
    {
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

      return string.Format(
        "CREATE TRIGGER {0} " +
        "ON {1} " +
        "AFTER {2} AS\nBEGIN\nSET NOCOUNT ON;",
        QuoteForTriggerName(triggerName),
        QuoteForTableName(dataTableName), actionName);
    }

    public string GetCreateTriggerFooterString(string triggerName,
      string dataTableName, TriggerActions action)
    {
      return "SET NOCOUNT OFF;\nEND";
    }

    public string GetDropTriggerString(string triggerName,
      string dataTableName, TriggerActions action)
    {
      return string.Format("IF OBJECT_ID ('{0}', 'TR') IS NOT NULL " +
                           "DROP TRIGGER {1};", triggerName,
                           QuoteForTriggerName(triggerName));
    }

    public string GetInsertIntoString(string destTable,
      IEnumerable<string> columnNames, string sourceTable,
      IEnumerable<string> values)
    {
      var columnString = string.Join(", ", columnNames.ToArray());
      var valueString = string.Join(", ", values.ToArray());

      return string.Format(
        "insert into {0} ({1}) " +
        "select {2} " +
        "from {3}",
        QuoteForTableName(destTable),
        columnString, valueString, sourceTable);
    }

    public string QualifyColumn(string tableName,
      string columnName)
    {
      return string.Format("{0}.{1}",
        tableName, columnName);
    }

    public string GetTriggerNewDataAlias()
    {
      return "inserted";
    }

    public string GetTriggerOldDataAlias()
    {
      return "deleted";
    }

    public string QuoteForTriggerName(string triggerName)
    {
      return QuoteForTableName(triggerName);
    }

  }

}
