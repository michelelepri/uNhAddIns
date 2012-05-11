using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;
using uNhAddIns.Audit.TriggerGenerator.Dialects;
using System;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public class AuditTrigger :
    Trigger
  {

    private readonly string[] _dataColumnNames;
    private readonly AuditTable _auditTable;

    public AuditTrigger(Table dataTable, AuditTable auditTable,
      INamingStrategy namingStrategy, TriggerActions action)
      : base(
      namingStrategy.GetTriggerName(dataTable, action),
      dataTable.GetQuotedName(),
      action)
    {
      _auditTable = auditTable;
      _dataColumnNames = (
        from column in dataTable.ColumnIterator
        select column.GetQuotedName()
        ).ToArray();
    }

    public override string SqlTriggerBody(
      Dialect dialect, IMapping p,
      string defaultCatalog, string defaultSchema)
    {
      var auditTable = _auditTable
        .GetTable((IExtendedDialect)dialect);

      var auditColumns = (
        from column in auditTable.ColumnIterator
        let c = column as AuditColumn
        where c != null
        select c
        ).ToArray();

      var auditTableName = dialect.QuoteForTableName(auditTable.Name);
      var eDialect = (IExtendedDialect)dialect;

      string triggerSource = Action == TriggerActions.DELETE ?
        eDialect.GetTriggerOldDataAlias() :
        eDialect.GetTriggerNewDataAlias();

      var columns = new List<string>(_dataColumnNames);
      columns.AddRange(from ac in auditColumns
                       select ac.Name);

      var values = new List<string>();
      values.AddRange(
        from columnName in _dataColumnNames
        select eDialect.QualifyColumn(
          triggerSource, columnName));
      values.AddRange(
        from auditColumn in auditColumns
        select auditColumn.ValueFunction.Invoke(Action));

      return eDialect.GetInsertIntoString(auditTableName,
        columns, triggerSource, values);

    }

  }

}
