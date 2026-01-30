using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.Common.Validation.Messages
{
    public static class EventMessages
    {
        public const string TitleRequired = "Event title is required.";
        public const string TitleLength =
            "Event title must be between {2} and {1} characters.";

        public const string DescriptionLength =
            "Description must be between {2} and {1} characters.";

        public const string InvalidDates =
            "End date must be after start date.";

        public const string InvalidParticipants =
            "Participants must be between {1} and {2}.";
    }
}
