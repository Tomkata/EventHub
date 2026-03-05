

namespace EventHub.Services.Services.Social
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.DTOs.Social;
    using EventHub.Core.Exceptions.Social;
    using EventHub.Core.Models.Social;
    using EventHub.Repositories.Interfaces.Social;
    using EventHub.Repositories.Interfaces.User;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces.Social;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

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


        public async Task Unfollow(string followerId,
                            string followingId,
                            CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(followerId) ||
                string.IsNullOrWhiteSpace(followingId))
                throw new ArgumentException("Invalid user id.");

            await _userFollowRepository
                .RemoveAsync(followerId, followingId, cancellation);
        }

        public async Task<PagedResult<SocialUserPreviewDto>> GetFollingsAsync(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellation
            )
        => await _userFollowRepository.GetAll()
            .Where(x=>x.FollowerId ==userId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => x.Following)
            .ProjectTo<SocialUserPreviewDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(pageNumber,pageSize,cancellation);

        public async Task<PagedResult<SocialUserPreviewDto>> GetFollowersAsync(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellation
            )
      => await _userFollowRepository.GetAll()
            .Where(x => x.FollowingId == userId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => x.Follower)
            .ProjectTo<SocialUserPreviewDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(pageNumber, pageSize, cancellation);


        public async Task<bool> IsFollowingAsync(string followerId, string followingId, CancellationToken cancellation)
    => await _userFollowRepository.ExistAsync(followerId, followingId, cancellation);

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number is 2627 or 2601;

            return false;
        }
    }
}
