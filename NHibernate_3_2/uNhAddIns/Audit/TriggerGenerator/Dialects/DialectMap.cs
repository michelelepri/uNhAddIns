using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NHibernate.Dialect;

namespace uNhAddIns.Audit.TriggerGenerator.Dialects
{

  public static class DialectMap
  {
    private static readonly Type dialectType = typeof(Dialect);
    private static readonly Type eDialectInterface = typeof(IExtendedDialect);

    private static readonly IDictionary<Type, Type> eDialects;

    static DialectMap()
    {
      
      var asm = Assembly.GetExecutingAssembly();
      
      var query = from type in asm.GetTypes()
                  where type.IsClass &&
                  !type.IsAbstract &&
                  eDialectInterface.IsAssignableFrom(type) &&
                  dialectType.IsAssignableFrom(type)
                  select type;

      eDialects = new Dictionary<Type, Type>();
      foreach (var type in query)
        eDialects.Add(type.BaseType, type);
    }

    public static Type GetExtendedDialectType(Type dialect)
    {
      if (dialect.GetInterfaces().Contains(eDialectInterface))
        return dialect;
      if (eDialects.ContainsKey(dialect))
        return eDialects[dialect];
      throw new NotSupportedException(string.Format(
        "Extended dialect for {0} not found.", dialect.Name));
    }

  }

}
