using Common.Configs;
using Common.Contracts;
using Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthService
{
    public class AuthService:IAuthService
    {
        Dictionary<string, Token> TokensCollection;
        public AuthService()
        {
            TokensCollection = new Dictionary<string, Token>();

        }
        public string GetNewToken(string userId)
        {
            //returns a new token after adding it to the collection
            Token newToken = new Token(userId);
            TokensCollection[newToken.token] = newToken;
            return newToken.token;
        }
        public bool IsTokenValid(string token)
        {
            //checks if the token is valid
            if (TokenExists(token))
            {
                if (TokenIsActive(TokensCollection[token].lastUsed))
                {
                    UpdateTokenOnUse(token);
                    return true;
                }
            }
            return false;
        }

        private void UpdateTokenOnUse(string token)
        {
            //updates the token's last used
            TokensCollection[token].lastUsed = DateTime.Now;
        }

        private bool TokenExists(string token)
        {
            //checks if the token exists
            return TokensCollection.ContainsKey(token);
        }

        private bool TokenIsActive(DateTime lastUsed)
        {
            //checks if the token is still active
            return lastUsed.AddMinutes(MainConfigs.TokenTTL) >= DateTime.Now;
        }
    }
}