using System;

namespace uNhAddIns.SessionEasier.Conversations
{
	public class DefaultConversationFactory : IConversationFactory
	{
		private readonly ISessionFactoryProvider factoriesProvider;
		private readonly ISessionWrapper wrapper;

		public DefaultConversationFactory(ISessionFactoryProvider factoriesProvider, ISessionWrapper wrapper)
		{
			if (factoriesProvider == null)
			{
				throw new ArgumentNullException("factoriesProvider");
			}
			if (wrapper == null)
			{
				throw new ArgumentNullException("wrapper");
			}
			this.factoriesProvider = factoriesProvider;
			this.wrapper = wrapper;
		}

		#region Implementation of IConversationFactory

		public IConversation CreateConversation()
		{
			return new NhConversation(factoriesProvider, wrapper);
		}

		public IConversation CreateConversation(string conversationId)
		{
			return new NhConversation(factoriesProvider, wrapper, conversationId);
		}

		#endregion
	}
}