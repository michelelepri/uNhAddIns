using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace uNhAddIns.UserTypes
{
	public class uNHAddinsEncryptor : IEncryptor
	{
		private readonly SymmetricAlgorithm cryptoProvider;
		private byte[] myBytes;

		public uNHAddinsEncryptor()
		{
			cryptoProvider = new DESCryptoServiceProvider();
			EncryptionKey = "uNHAddin";
		}

		public string EncryptionKey { get; set; }

		public string Encrypt(string password)
		{
			var bytes = GetBytes();
			using (var memoryStream = new MemoryStream())
			{
				ICryptoTransform encryptor = cryptoProvider.CreateEncryptor(bytes, bytes);
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					using (var writer = new StreamWriter(cryptoStream))
					{
						writer.Write(password);
						writer.Flush();
						cryptoStream.FlushFinalBlock();
						return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
					}
				}
			}
		}

		private byte[] GetBytes()
		{
			if (myBytes == null)
				myBytes = Encoding.ASCII.GetBytes(EncryptionKey);

			return myBytes;
		}

		public string Decrypt(string encryptedPassword)
		{
			var bytes = GetBytes();
			using (var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedPassword)))
			{
				ICryptoTransform decryptor = cryptoProvider.CreateDecryptor(bytes, bytes);
				using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
				{
					using (var reader = new StreamReader(cryptoStream))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}

	}
}
