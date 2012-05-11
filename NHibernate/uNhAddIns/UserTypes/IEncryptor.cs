namespace uNhAddIns.UserTypes
{
	public interface IEncryptor
	{
		string Encrypt(string password);
		string Decrypt(string encryptedPassword);
		string EncryptionKey { get; set; }
	}
}