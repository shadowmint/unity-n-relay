using System;
using System.Text;
using N.Package.Relay.Infrastructure.Model;

namespace N.Package.Relay.Infrastructure
{
    public class RelayAuthService
    {
        /// <summary>
        /// The hash is:
        /// sha256(transaction_id:expires:key:secret)
        /// </summary>
        public AuthRequest GenerateAuthRequest(string transactionId, long expiresInSeconds, string key, string secret)
        {
            var request = new AuthRequest()
            {
                key = key,
                expires = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expiresInSeconds,
            };
            SignRequest(transactionId, request, secret);
            return request;
        }

        private void SignRequest(string transactionId, AuthRequest request, string secret)
        {
            var secretPhrase = $"{transactionId}:{request.expires}:{request.key}:{secret}";
            request.hash = Sha256(secretPhrase);
        }

        private static string Sha256(string phrase)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(phrase));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}