using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Exceptions.Oranizer.ForReject
{
    public class InvalidUserToReject : Exception
    {
        public InvalidUserToReject()
            :base("Invalid user to reject!")
        {
            
        }
    }
}
