namespace Ads.Automation.Domain.Shared.Common
{
    /// <summary>
    /// Meta Token 加密服务
    /// 沿用 AesEncryptionService 的密钥和 IV，透明代理
    /// </summary>
    public class MetaTokenEncryptionService
    {
        private readonly AesEncryptionService _aes = new();

        public string Encrypt(string plainText) => _aes.Encrypt(plainText);
        public string Decrypt(string cipherText) => _aes.Decrypt(cipherText);
    }
}
