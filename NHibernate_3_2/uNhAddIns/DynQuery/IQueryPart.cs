namespace uNhAddIns.DynQuery
{
	public interface IQueryPart
	{
		/// <summary>
		/// The query complete clause.
		/// </summary>
		string Clause { get; }

		/// <summary>
		/// The query part.
		/// </summary>
		string Expression { get;}
	}
}