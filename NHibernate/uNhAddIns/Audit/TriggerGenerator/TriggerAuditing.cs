using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using uNhAddIns.Audit.TriggerGenerator.Dialects;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public class TriggerAuditing
  {

    private readonly Configuration _configuration;
    private readonly INamingStrategy _namingStrategy;
    private readonly IAuditColumnSource _columnSource;
    private readonly Func<Table, bool> _tableFilter;

    public TriggerAuditing(Configuration configuration)
      : this(configuration,
      new NamingStrategy(), new AuditColumnSource(),
      t => !t.Name.Contains("Audit"))
    { }

    public TriggerAuditing(Configuration configuration,
      Func<Table, bool> tableFilter)
      : this(configuration,
      new NamingStrategy(), new AuditColumnSource(),
      tableFilter)
    { }

    public TriggerAuditing(Configuration configuration,
      IAuditColumnSource auditColumnSource)
      : this(configuration, new NamingStrategy(),
      auditColumnSource, t => !t.Name.Contains("Audit"))
    { }

    public TriggerAuditing(Configuration configuration,
      IAuditColumnSource auditColumnSource,
      Func<Table, bool> tableFilter)
      : this(configuration, new NamingStrategy(),
      auditColumnSource, tableFilter)
    { }

    public TriggerAuditing(Configuration configuration,
      INamingStrategy namingStrategy)
      : this(configuration, namingStrategy,
      new AuditColumnSource(), t => !t.Name.Contains("Audit"))
    { }

    public TriggerAuditing(Configuration configuration,
      INamingStrategy namingStrategy,
      Func<Table, bool> tableFilter)
      : this(configuration, namingStrategy,
      new AuditColumnSource(), tableFilter)
    { }

    public TriggerAuditing(Configuration configuration,
      INamingStrategy namingStrategy,
      IAuditColumnSource auditColumnSource)
      : this(configuration, namingStrategy,
      auditColumnSource, t => !t.Name.Contains("Audit"))
    { }

    public TriggerAuditing(Configuration configuration,
      INamingStrategy namingStrategy,
      IAuditColumnSource columnSource,
      Func<Table, bool> tableFilter)
    {
      _configuration = configuration;
      _namingStrategy = namingStrategy;
      _columnSource = columnSource;
      _tableFilter = tableFilter;
    }

    public void Configure()
    {
      _configuration.BuildMappings();
      UpdateDialect();
      var dialect = Dialect.GetDialect(_configuration.Properties);
      var mappings = _configuration.CreateMappings(dialect);
      AddAuditing(mappings);
    }

    private void UpdateDialect()
    {
      var dialect = Dialect.GetDialect(_configuration.Properties);

      var eDialectType = DialectMap
        .GetExtendedDialectType(dialect.GetType());

      if (eDialectType == null)
        throw new ApplicationException(
          "Dialect must implement IExtendedDialect to "
          + "create audit triggers");

      if (eDialectType != dialect.GetType())
      {
        _configuration.SetProperty(
          NHibernate.Cfg.Environment.Dialect,
          eDialectType.AssemblyQualifiedName);
      }
    }

    private void AddAuditing(Mappings mappings)
    {
      var auditObjects = new List<IAuxiliaryDatabaseObject>();
      foreach (var table in mappings.IterateTables.Where(_tableFilter))
      {
        var auditTable = new AuditTable(
          table, _namingStrategy, _columnSource);
        mappings.AddAuxiliaryDatabaseObject(auditTable);

        var insertTrigger = new AuditTrigger(table,
          auditTable, _namingStrategy, TriggerActions.INSERT);
        mappings.AddAuxiliaryDatabaseObject(insertTrigger);

        var updateTrigger = new AuditTrigger(table,
          auditTable, _namingStrategy, TriggerActions.UPDATE);
        mappings.AddAuxiliaryDatabaseObject(updateTrigger);

        var deleteTrigger = new AuditTrigger(table,
          auditTable, _namingStrategy, TriggerActions.DELETE);
        mappings.AddAuxiliaryDatabaseObject(deleteTrigger);
      }

    }

  }

}
