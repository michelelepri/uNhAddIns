using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Mapping;
using uNhAddIns.Audit.TriggerGenerator.Dialects;
using NHibernate.Dialect.Function;
using System.Collections;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public class AuditColumnSource
    : IAuditColumnSource
  {

    public const string CurrentTimestampFunctionName = "current_timestamp";
    public const string CurrentUserFunctionName = "current_user";

    public IEnumerable<AuditColumn> GetAuditColumns(
      Table auditTable,
      IExtendedDialect dialect)
    {

      var auditColumns = new List<AuditColumn>();
      var baseDialect = (Dialect)dialect;
      var hasUserFunction = baseDialect.Functions.ContainsKey(
        CurrentUserFunctionName);
      var hasTimestampFunction = baseDialect.Functions.ContainsKey(
        CurrentTimestampFunctionName);

      if (hasUserFunction)
        auditColumns.Add(new AuditColumn()
        {
          Name = "AuditUser",
          Value = new SimpleValue()
          {
            TypeName = NHibernateUtil.AnsiString.Name
          },
          Length = 256,
          IsNullable = false,
          IncludeInPrimaryKey = true,
          ValueFunction = delegate(TriggerActions action)
          {
            var funcName = CurrentUserFunctionName;
            return baseDialect.Functions[funcName]
              .Render(new ArrayList(), null).ToString();
          }
        });

      if (hasTimestampFunction)
        auditColumns.Add(new AuditColumn()
          {
            Name = "AuditTimestamp",
            Value = new SimpleValue()
            {
              TypeName = NHibernateUtil.DateTime.Name
            },
            IsNullable = false,
            IncludeInPrimaryKey = true,
            ValueFunction = delegate(TriggerActions action)
            {
              var funcName = CurrentTimestampFunctionName;
              return baseDialect.Functions[funcName]
                .Render(new ArrayList(), null).ToString();
            }
          });

      auditColumns.Add(new AuditColumn()
      {
        Name = "AuditOperation",
        Value = new SimpleValue()
        {
          TypeName = NHibernateUtil.AnsiString.Name
        },
        Length = 6,
        IsNullable = false,
        IncludeInPrimaryKey = false,
        ValueFunction = delegate(TriggerActions action)
        {
          switch (action)
          {
            case TriggerActions.INSERT:
              return "'INSERT'";
            case TriggerActions.UPDATE:
              return "'UPDATE'";
            case TriggerActions.DELETE:
              return "'DELETE'";
            default:
              throw new ArgumentOutOfRangeException("action");
          }
        }
      });

      return auditColumns;
    }

  }
}
