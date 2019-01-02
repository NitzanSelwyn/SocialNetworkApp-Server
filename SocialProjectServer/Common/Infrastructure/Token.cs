using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public class Token
    {
        public Token(string Id)
        {
            this.Id = Id;
            GenerateToken();
        }

        private void GenerateToken()
        {
            //generates the new token
            tokenKey = Guid.NewGuid().ToString();
            LastUsed = DateTime.Now;
        }

        public string tokenKey { get; set; }
        public string Id { get; set; }
        public DateTime LastUsed { get; set; }


    }
}
