using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Exceptions.User
{
    public class InvalidUserPermissionsException : Exception
    {
        public InvalidUserPermissionsException()
            :base("User doesn't have that permissions!")
        {
            
        }
    }
}
