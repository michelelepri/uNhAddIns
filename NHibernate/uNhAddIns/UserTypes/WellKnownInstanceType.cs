using System;
using System.Collections.Generic;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace uNhAddIns.UserTypes
{
    /// <summary>
    /// A <see cref="IUserType"/> to manage relationships with a well know entities.
    /// </summary>
    /// <typeparam name="T">The type of the wellknow entity</typeparam>
    /// <remarks>
    /// <typeparamref name="T"/> is the type tp use in the entity owning the relation, the type in the persistence is <see cref="int"/>.
    /// </remarks>
    [Serializable]
    public abstract class WellKnownInstanceType<T> : GenericWellKnownInstanceType<T, int> where T : class
    {
        private static readonly SqlType[] sqlTypes = new[] { SqlTypeFactory.Int32 };

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="repository">The collection that represent a in-memory repository.</param>
        /// <param name="findPredicate">The predicate an instance by the persisted value.</param>
        /// <param name="idGetter">The getter of the persisted value.</param>
        protected WellKnownInstanceType(IEnumerable<T> repository, Func<T, int, bool> findPredicate, Func<T, int> idGetter) : base(repository, findPredicate, idGetter)
        {
        }

        public override SqlType[] SqlTypes
        {
            get { return sqlTypes; }
        }
    }
}