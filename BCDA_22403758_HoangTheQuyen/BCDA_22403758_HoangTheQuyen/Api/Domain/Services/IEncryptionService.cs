namespace K8sManager.Api.Domain.Services;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
}

