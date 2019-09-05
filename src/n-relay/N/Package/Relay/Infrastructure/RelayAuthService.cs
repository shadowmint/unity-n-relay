using System;
using System.Text;
using N.Package.Relay.Infrastructure.Model;
using UnityEngine;

namespace N.Package.Relay.Infrastructure
{
    public class RelayAuthService
    {
        /// <summary>
        /// Generate an auth token from the given options
        /// </summary>
        public string GenerateAuthToken(RelayAuthOptions authOptions)
        {
            return GenerateAuthToken(authOptions.sessionLength, authOptions.authKey, authOptions.authSecret);
        }
        
        /// <summary>
        /// Return the base64 encoded json representation of the auth event
        /// </summary>
        public string GenerateAuthToken(long expiresInSeconds, string key, string secret)
        {
            var request = GenerateAuthRequest(expiresInSeconds, key, secret);
            var asJson = JsonUtility.ToJson(request);
            var buffer = (new UTF8Encoding()).GetBytes(asJson);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// The hash is:
        /// sha256(expires:key:secret)
        /// </summary>
        private AuthRequest GenerateAuthRequest(long expiresInSeconds, string key, string secret)
        {
            var request = new AuthRequest()
            {
                key = key,
                expires = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expiresInSeconds,
            };
            SignRequest(request, secret);
            return request;
        }

        private void SignRequest(AuthRequest request, string secret)
        {
            var secretPhrase = $"{request.expires}:{request.key}:{secret}";
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