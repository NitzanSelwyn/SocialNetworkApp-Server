using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.TempModels
{
    public class UserRepresentation
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public UserRepresentation()
        {

        }
        public UserRepresentation(string Id,string FullName)
        {
            this.Id = Id;
            this.FullName = FullName;
        }
    }
}
