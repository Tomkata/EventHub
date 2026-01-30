using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Common
{
    public class DataValidations
    {
        public static class Event
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 100;


            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 2000;

            public const int AddressMinLength = 5;
            public const int AddressMaxLength = 200;

            public const int MaxParticipantsMin = 1;
            public const int MaxParticipantsMax = 10000;
        }
    }
}
