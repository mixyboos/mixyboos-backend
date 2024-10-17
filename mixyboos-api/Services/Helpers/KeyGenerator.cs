using System;
using System.Security.Cryptography;

namespace MixyBoos.Api.Services.Helpers {
    public static class KeyGenerator {
        public static string GenerateRandomString(int length,
            string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890") =>
            new Random().GenerateRandomString(length, charset);

        public static string GenerateRandomString(this Random random, int length,
            string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890") =>
            RandomString(random.NextBytes, length, charset.ToCharArray());

        public static string GenerateRandomCryptoString(int length) {
            using var rng = RandomNumberGenerator.Create();
            var randomNumber = new byte[length];
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static string RandomString(Action<byte[]> fillRandomBuffer, int length, char[] charset) {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length),
                    $"{nameof(length)} must be greater or equal to 0");
            if (charset is null)
                throw new ArgumentNullException(nameof(charset));
            if (charset.Length == 0)
                throw new ArgumentException($"{nameof(charset)} must contain at least 1 character", nameof(charset));

            var maxIdx = charset.Length;
            var chars = new char[length];
            var randomBuffer = new byte[length * 4];
            fillRandomBuffer(randomBuffer);

            for (var i = 0; i < length; i++)
                chars[i] = charset[BitConverter.ToUInt32(randomBuffer, i * 4) % maxIdx];

            return new string(chars);
        }
    }
}
