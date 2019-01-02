using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure
{
    public class Token
    {
        public Token(string id)
        {
            this.id = id;
            GenerateToken();
        }

        private void GenerateToken()
        {
            //generates the new token
            token = Guid.NewGuid().ToString();
            lastUsed = DateTime.Now;
        }

        public string token { get; set; }
        public string id { get; set; }
        public DateTime lastUsed { get; set; }


    }
}
