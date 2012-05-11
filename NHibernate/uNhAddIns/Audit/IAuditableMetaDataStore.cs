namespace uNhAddIns.Audit
{
	/// <summary>
	/// Metadata store for auditable entities.
	/// </summary>
	public interface IAuditableMetaDataStore
	{
		/// <summary>
		/// Register metadata in the store where the entity is auditable.
		/// </summary>
		/// <param name="entityName">The entity name.</param>
		/// <returns>true if the entity was register as auditable; false otherwise.</returns>
		bool RegisterAuditableEntityIfNeeded(string entityName);

		/// <summary>
		/// Returns a Boolean value that indicates whether the store contains the metadata for the specific entity.
		/// </summary>
		/// <param name="entityName">The entity name.</param>
		/// <returns>true if the the store contains metadata for the given entity; false otherwise.</returns>
		bool Contains(string entityName);

		/// <summary>
		/// Returns a <see cref="IAuditableMetaData"/> for the specific entity.
		/// </summary>
		/// <param name="entityName">The entity name.</param>
		/// <returns><see cref="IAuditableMetaData"/> if the the store contains metadata for the given entity; null otherwise.</returns>
		IAuditableMetaData GetAuditableMetaData(string entityName);
	}
}