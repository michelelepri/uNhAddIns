using System;
using System.Collections;
using NHibernate;
using NHibernate.Properties;
using NHibernate.Transform;
using uNhAddIns.Extensions;

namespace uNhAddIns.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result to 
	/// a user specified class which will be populated via setter  
	/// methods or fields matching the alias names. 
	/// </summary>
	/// <example>
	/// <code>
	/// IList resultWithAliasedBean = s.CreateQuery(select f.Name, f.Description from Foo f)
	/// 			.SetResultTransformer(new PositionalToBeanResultTransformer(typeof (NoFoo), new string[] {"_name", "_description"}))
	/// 			.List();
	/// 
	/// NoFoo dto = (NoFoo)resultWithAliasedBean[0];
	/// </code>
	/// </example>
	/// <remarks>
	/// If you have a <see cref="ICriteria"/> or a <see cref="IQuery"/> with aliases you can use
	/// <see cref="NHibernate.Transform.AliasToBeanResultTransformer"/> class.
	/// </remarks>
	[Serializable]
	public class PositionalToBeanResultTransformer : IResultTransformer
	{
		private readonly Type resultClass;
		private readonly string[] positionalAliases;
		private ISetter[] setters;
		private readonly IPropertyAccessor propertyAccessor;

		/// <summary>
		/// Initializes a new instance of the PositionalToBeanResultTransformer class.
		/// </summary>
		/// <param name="resultClass">The return <see cref="Type"/>.</param>
		/// <param name="positionalAliases">Alias for each position of the query.</param>
		public PositionalToBeanResultTransformer(Type resultClass, string[] positionalAliases)
		{
			if (resultClass == null)
			{
				throw new ArgumentNullException("resultClass");
			}
			this.resultClass = resultClass;
			if (positionalAliases == null || positionalAliases.Length == 0)
			{
				throw new ArgumentNullException("positionalAliases");
			}
			this.positionalAliases = positionalAliases;
			propertyAccessor =
				new ChainedPropertyAccessor(new[]
				                            	{
				                            		PropertyAccessorFactory.GetPropertyAccessor("field"),
				                            		PropertyAccessorFactory.GetPropertyAccessor(null)
				                            	});
			AssignSetters();
		}

		private void AssignSetters()
		{
			setters = new ISetter[positionalAliases.Length];
			for (int i = 0; i < positionalAliases.Length; i++)
			{
				string alias = positionalAliases[i];
				if (!string.IsNullOrEmpty(alias))
				{
					setters[i] = propertyAccessor.GetSetter(resultClass, alias);
				}
			}
		}

		#region IResultTransformer Members

		public IList TransformList(IList collection)
		{
			return collection;
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			var result = ReflectionExtensions.Instantiate<object>(resultClass);
			try
			{
				for (int i = 0; i < setters.Length; i++)
				{
					if (setters[i] != null)
					{
						setters[i].Set(result, tuple[i]);
					}
				}
			}
			catch (IndexOutOfRangeException)
			{
				throw new HibernateException("Tuple have less scalars then trasformer class: " + resultClass.FullName);
			}

			return result;
		}

		#endregion
	}
}