using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;

namespace uNhAddIns.Audit.TriggerGenerator
{

  public static class ConfigurationExtensions
  {

    public static Configuration AddTriggerAuditing(
      this Configuration cfg)
    {
      new TriggerAuditing(cfg).Configure();
      return cfg;
    }

    public static Configuration AddTriggerAuditing(
      this Configuration cfg,
      INamingStrategy namingStrategy)
    {
      new TriggerAuditing(cfg, namingStrategy).Configure();
      return cfg;
    }

    public static Configuration AddTriggerAuditing(
      this Configuration cfg,
      IAuditColumnSource auditColumnSource)
    {
      new TriggerAuditing(cfg, auditColumnSource).Configure();
      return cfg;
    }

    public static Configuration AddTriggerAuditing(
      this Configuration cfg,
      INamingStrategy namingStrategy,
      IAuditColumnSource auditColumnSource)
    {
      new TriggerAuditing(cfg, namingStrategy, auditColumnSource)
        .Configure();
      return cfg;
    }


  }

}
