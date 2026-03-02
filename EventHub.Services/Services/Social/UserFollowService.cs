
using AutoMapper;
using EventHub.Core.DTOs.Social;
using EventHub.Core.Exceptions.Social;
using EventHub.Core.Models.Social;
using EventHub.Repositories.Interfaces;
using EventHub.Repositories.Interfaces.Social;
using EventHub.Services.Interfaces.Social;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Services.Services.Social
{
    public class UserFollowService : IUserFollowService
    {
        private readonly IUserFollowRepository _userFollowRepository;
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfileRepository;



        public UserFollowService(IUserFollowRepository userFollowRepository,
                                 IMapper mapper,
                                 IUserProfileRepository userProfileRepository)
        {
            this._userFollowRepository = userFollowRepository;
            this._mapper = mapper;
            this._userProfileRepository = userProfileRepository;
        }
        public async Task Follow(string followerId, string followingId,
            CancellationToken cancellation)
        {
            if (followerId == followingId)
                throw new CannotFollowYourselfException();

            var followerHasProfile = await _userProfileRepository.ExistsAsync(followerId, cancellation);
            var followingHasProfile = await _userProfileRepository.ExistsAsync(followingId, cancellation);

            if (!followerHasProfile || !followingHasProfile)
                throw new FollowingProfileNotCompleted(); 


            if (await _userFollowRepository.ExistAsync(followerId, followingId, cancellation))
                return; //Ignore (idempotent)


            try
            {
                var userFollow = new UserFollow()
                {
                    FollowerId = followerId,
                    FollowingId = followingId
                };

                await _userFollowRepository.AddUserFollowAsync(userFollow, cancellation);
                await _userFollowRepository.SaveChangesAsync(cancellation);
            }
            catch (DbUpdateException ex)
            when (IsUniqueViolation(ex))
            {
                // If two requests hit at the same time -> one will go in, the other will hit PK.
                // For idempotent Follow this is SUCCESS.
                return;
            }
         
        }

        public Task Unfollow(string followerId, string followingId,
           CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<SocialUserPreviewDto> GetFollingsAsync(string userId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<SocialUserPreviewDto> GetFollowersAsync(string userId, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number is 2627 or 2601;

            return false;
        }
    }
}
