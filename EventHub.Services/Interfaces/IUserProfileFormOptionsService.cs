
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.UserProfile;

namespace EventHub.Services.Interfaces
{
    public interface IUserProfileFormOptionsService
    {
        public Task<UserProfileOptionDto> GetFormOptionsAsync(CancellationToken cancellationToken);

    }
}
