using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uNhAddIns.Inflector
{
	public class ItalianNamingStrategy : InflectorNamingStrategy
	{
		public ItalianNamingStrategy() : base(new ItalianInflector()) {}
	}
}
