using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaar.Authorizations
{
    public class TokenResponse
    {
        public string TokenType { get; set; }
        public string ExpiresIn { get; set; }
        public string ExtExpiresIn { get; set; }
        public string ExpiresOn { get; set; }
        public string NotBefore { get; set; }
        public string Resource { get; set; }
        public string AccessToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
    }
}
