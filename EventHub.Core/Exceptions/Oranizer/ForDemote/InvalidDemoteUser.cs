using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Exceptions.Oranizer.ForDemote
{
    public class InvalidDemoteUser : Exception
    {
        public InvalidDemoteUser()
        :base("Invalid user to demote!")
        {
            
        }
    }
}
