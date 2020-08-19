using System;
using System.DirectoryServices;
using PTCwebApi.Models;

namespace PTCwebApi.Security {
    public class AuthenticateUsers {
        internal static Boolean AuthenticateUser (UserLogin model) {
            DirectoryEntry entry = new DirectoryEntry ("LDAP://192.168.1.2", model.Username, model.Password);
            entry.AuthenticationType = AuthenticationTypes.Secure;
            try {
                DirectorySearcher search = new DirectorySearcher (entry);
                search.SearchRoot = entry;
                search.Filter = "(&(ObjectClass=user)SAMAccountName=" + model.Username + ")";
                SearchResultCollection result = search.FindAll ();
                if (null == result) {
                    return false;
                }
            } catch (Exception ex) {
                return false;
                throw new Exception ("Error authenticating user." + ex.Message);
            }
            return true;
        }
    }
}