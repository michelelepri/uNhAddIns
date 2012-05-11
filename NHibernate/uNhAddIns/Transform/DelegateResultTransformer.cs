using System;
using System.Collections;
using NHibernate;
using NHibernate.Transform;

namespace uNhAddIns.Transform
{
	/// <summary>
	/// Result transformer that allows to transform a result
	/// to an specific suplied <see cref="TEntity"/> with a 
	/// custom transform function based on a tuple that returns a <see cref="TEntity"/>.
	/// </summary>
	/// <example>
	/// <code>
	/// IList result = s.CreateQuery(select f.Name, f.Description from Foo f)
	/// 			.SetResultTransformer(new DelegateTransformer[Foo](t => new Foo(t[0], t[1]))
	/// 			.List[Foo]();
	/// 
	/// NoFoo dto = result[0];
	/// </code>
	/// </example>
	/// <remarks>
	/// If you have a <see cref="ICriteria"/> or a <see cref="IQuery"/> with aliases you can use
	/// <see cref="NHibernate.Transform.AliasToBeanResultTransformer"/> class.
	/// </remarks>
	[Serializable]
	public class DelegateTransformer<TEntity> : IResultTransformer
	{
		private readonly Func<object[], TEntity> _transformFunction;

		public DelegateTransformer(Func<object[], TEntity> transformFunction)
		{
			if (transformFunction == null) throw new ArgumentNullException("transformFunction");

			_transformFunction = transformFunction;
		}

		#region IResultTransformer Members

		public IList TransformList(IList collection)
		{
			return collection;
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return _transformFunction(tuple);
		}

		#endregion
	}
	
}