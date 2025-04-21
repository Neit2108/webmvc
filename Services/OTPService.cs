using OtpNet;
using System;

namespace BR_Architects.Services
{
    public class OtpService
    {
        private readonly int _step = 30; // số giây mỗi lần tạo mã OTP

        public string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateTotp(string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentException("Secret key cannot be null or empty.", nameof(secretKey));
            }

            var keyBytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(keyBytes, step: _step);
            return totp.ComputeTotp();
        }

        public bool ValidateTotp(string secretKey, string userInput)
        {
            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(userInput))
            {
                return false;
            }

            var keyBytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(keyBytes, step: _step);
            return totp.VerifyTotp(userInput, out long timeStepMatched, new VerificationWindow(previous: 1, future: 1));
        }
    }
}