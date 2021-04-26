using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Exceptions
{
    public class LoginException: Exception
    {
        public LoginException(string message) : base(message) { }
    }
}
