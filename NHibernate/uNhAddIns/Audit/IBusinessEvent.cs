using System;

namespace uNhAddIns.Audit
{
	public interface IBusinessEvent
	{
		Guid Id { get; }
		string UserName { get; }
		DateTime EventStartAt { get; }
	}
}