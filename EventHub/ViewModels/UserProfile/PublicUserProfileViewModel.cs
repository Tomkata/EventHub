namespace EventHub.Web.ViewModels.UserProfile
{
    public class PublicUserProfileViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImagePath { get; set; }
        public string Description { get; set; } = null!;
        public int JoinedEventsCount { get; set; }

        public bool IsFollowing { get; set; } 
        public bool IsOwnProfile { get; set; } 

        //public int MutualFriends { get; set; }    
        //public bool IsFriend { get; set; } 

        public IEnumerable<string> Interests { get; set; } =
                  new List<string>();
    }
}
