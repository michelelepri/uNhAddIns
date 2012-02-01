using System;
using System.Collections;
using System.Collections.Generic;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Util;

namespace uNhAddIns.SessionEasier
{
	[Serializable]
	public class SessionFactoryProvider : ISessionFactoryProvider
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (SessionFactoryProvider));

		private IEnumerable<ISessionFactory> esf;
		private IConfigurationProvider fcp;
		private ISessionFactory sf;

		public SessionFactoryProvider() : this(new DefaultSessionFactoryConfigurationProvider()) {}

		public SessionFactoryProvider(IConfigurationProvider configurationProvider)
		{
			if (configurationProvider == null)
			{
				throw new ArgumentNullException("configurationProvider");
			}
			fcp = configurationProvider;
		}

		#region ISessionFactoryProvider Members

		public ISessionFactory GetFactory(string factoryId)
		{
			Initialize();
			return sf;
		}

		public event EventHandler<EventArgs> BeforeCloseSessionFactory;

		#endregion

		public void Initialize()
		{
			if (sf == null)
			{
				log.Debug("Initialize a new session factory reading the configuration.");
				IEnumerator<Configuration> conf = fcp.Configure().GetEnumerator();
				if (conf.MoveNext())
				{
					sf = conf.Current.BuildSessionFactory();
					esf = new SingletonEnumerable<ISessionFactory>(sf);
				}
				fcp = null; // after built the SessionFactory the configuration is not needed
				if (conf.MoveNext())
				{
					log.Warn(
						@"More than one configurations are available but only the first was used.
Check your configuration or use a Multi-RDBS session-factory provider.");
				}
			}
		}

		private void DoBeforeCloseSessionFactory()
		{
			if (BeforeCloseSessionFactory != null)
			{
				BeforeCloseSessionFactory(this, new EventArgs());
			}
		}

		#region Implementation of IEnumerable

		public IEnumerator<ISessionFactory> GetEnumerator()
		{
			Initialize();
			return esf.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IDisposable

		private bool disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SessionFactoryProvider()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (sf != null)
					{
						DoBeforeCloseSessionFactory();
						sf.Close();
						sf = null;
					}
				}
				disposed = true;
			}
		}

		#endregion
	}
}