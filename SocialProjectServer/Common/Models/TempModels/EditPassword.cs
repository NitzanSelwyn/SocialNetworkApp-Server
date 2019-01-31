using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.TempModels
{
    public class EditPassword
    {
        public string Username { get; set; }
        public string LastPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
        public EditPassword()
        {

        }
        public EditPassword(string LastPassword, string NewPassword, string NewPasswordConfirm, string Username)
        {
            this.LastPassword = LastPassword;
            this.NewPassword = NewPassword;
            this.NewPasswordConfirm = NewPasswordConfirm;
            this.Username = Username;
        }
    }
}
