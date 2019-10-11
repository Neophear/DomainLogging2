using DomainLoggingService.Model;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace DomainLoggingService.Controller
{
    public class UserController
    {
        public User GetUser(string username)
        {
            if (String.IsNullOrEmpty(username))
                return null;

            PrincipalContext ouContex = new PrincipalContext(ContextType.Domain, "TRR-INET.local", "DC = TRR-INET,DC = LOCAL");
            UserPrincipal up = UserPrincipal.FindByIdentity(ouContex, username);

            return up == null ? null : new User { Username = up.SamAccountName, Name = $"{up.GivenName} {up.Surname}", Locked = up.IsAccountLockedOut() };
        }
    }
}