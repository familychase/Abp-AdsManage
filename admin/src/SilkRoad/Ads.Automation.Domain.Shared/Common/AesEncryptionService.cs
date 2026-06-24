using System.Security.Cryptography;
using System.Text;

namespace Ads.Automation.Domain.Shared.Common
{
    /// <summary>
    /// AES 加密服务，提供 AES-256-CBC + PKCS7 加密和解密功能
    /// </summary>
    public class AesEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        /// <summary>
        /// 使用默认硬编码密钥和 IV（旧版兼容）
        /// </summary>
        public AesEncryptionService()
        {
            _key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes
            _iv = Encoding.UTF8.GetBytes("1234567890123456");                 // 16 bytes
        }

        /// <summary>
        /// 使用指定的密钥和 IV
        /// </summary>
        /// <param name="key">AES 密钥（32 bytes for AES-256）</param>
        /// <param name="iv">初始向量（16 bytes）</param>
        public AesEncryptionService(byte[] key, byte[] iv)
        {
            _key = key;
            _iv = iv;
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor();
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var decryptor = aes.CreateDecryptor();
            var bytes = Convert.FromBase64String(cipherText);
            var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
