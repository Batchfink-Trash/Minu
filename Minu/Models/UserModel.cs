using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;

namespace Minu.Models
{
    public class UserModel : IUserIdentity
    {
        public Guid id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}