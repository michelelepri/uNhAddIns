using System.Text;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;
using uNhAddIns.Audit.TriggerGenerator.Dialects;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public abstract class Trigger :
    AbstractAuxiliaryDatabaseObject
  {

    private readonly string _triggerName;
    private readonly TriggerActions _action;
    private readonly string _tableName;

    public string TriggerName { get { return _triggerName; } }
    public TriggerActions Action { get { return _action; } }
    public string TableName { get { return _tableName; } }

    public Trigger(string triggerName, string tableName, TriggerActions action)
    {
      _action = action;
      _triggerName = triggerName;
      _tableName = tableName;
    }

    public abstract string SqlTriggerBody(Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema);

    public override string SqlCreateString(Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
    {
      IExtendedDialect eDialect = (IExtendedDialect)dialect;

      var buf = new StringBuilder();

      buf.AppendLine(eDialect.GetCreateTriggerHeaderString(
        _triggerName, _tableName, _action));

      buf.AppendLine(SqlTriggerBody(dialect, p, defaultCatalog, defaultSchema));

      buf.AppendLine(eDialect.GetCreateTriggerFooterString(
        _triggerName, _tableName, _action));

      return buf.ToString();
    }

    public override string SqlDropString(Dialect dialect, string defaultCatalog, string defaultSchema)
    {
      var eDialect = (IExtendedDialect)dialect;
      return eDialect.GetDropTriggerString(
        _triggerName, _tableName, _action);
    }
  }

}
