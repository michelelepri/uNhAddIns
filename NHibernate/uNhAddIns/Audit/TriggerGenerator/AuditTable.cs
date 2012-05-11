using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;
using uNhAddIns.Audit.TriggerGenerator.Dialects;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public class AuditTable : AbstractAuxiliaryDatabaseObject
  {

    private bool isFinalized;
    protected readonly Table auditTable;
    protected IAuditColumnSource auditColumnSource;

    public AuditTable(Table dataTable,
      INamingStrategy namingStrategy,
      IAuditColumnSource auditColumnSource)
    {
      this.auditColumnSource = auditColumnSource;
      auditTable = BuildAuditTable(dataTable, namingStrategy);
    }

    public override string SqlCreateString(Dialect dialect,
      IMapping p, string defaultCatalog, string defaultSchema)
    {
      FinalizeAuditTable((IExtendedDialect) dialect);
      return auditTable.SqlCreateString(
        dialect, p, defaultCatalog, defaultSchema);
    }

    public override string SqlDropString(Dialect dialect,
      string defaultCatalog, string defaultSchema)
    {
      FinalizeAuditTable((IExtendedDialect)dialect);
      return auditTable.SqlDropString(dialect,
        defaultCatalog, defaultSchema);
    }

    protected virtual Table BuildAuditTable(Table dataTable,
      INamingStrategy namingStrategy)
    {
      var auditTableName = namingStrategy.GetAuditTableName(dataTable);
      var auditTable = new Table(auditTableName);
      CopyColumns(dataTable, auditTable);
      CopyPrimaryKey(dataTable, auditTable);
      return auditTable;
    }

    protected virtual void FinalizeAuditTable(
      IExtendedDialect dialect)
    {
      if (!isFinalized)
      {
        var auditColumns =
          auditColumnSource.GetAuditColumns(auditTable, dialect);
        AddAuditColumns(auditTable, auditColumns);
        auditColumnSource = null;
        isFinalized = true;
      }
    }

    protected virtual void CopyColumns(Table dataTable,
      Table auditTable)
    {
      foreach (var column in dataTable.ColumnIterator)
        auditTable.AddColumn((Column)column.Clone());
    }

    protected virtual void CopyPrimaryKey(Table dataTable,
      Table auditTable)
    {
      if (dataTable.PrimaryKey != null)
      {
        var pk = new PrimaryKey();

        pk.AddColumns(
          from column in dataTable.PrimaryKey.ColumnIterator
          select auditTable.GetColumn(column));

        auditTable.PrimaryKey = pk;
      }
    }

    protected virtual void AddAuditColumns(Table auditTable,
      IEnumerable<AuditColumn> auditColumns)
    {
      foreach (var column in auditColumns)
      {
        auditTable.AddColumn(column);
        if (column.IncludeInPrimaryKey)
          auditTable.PrimaryKey.AddColumn(column);
      }
    }

    internal Table GetTable(IExtendedDialect dialect)
    {
      FinalizeAuditTable(dialect);
      return auditTable;
    }

  }

}
