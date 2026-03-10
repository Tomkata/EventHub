

namespace EventHub.Web.Mappings
{
    using AutoMapper;
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.DTOs.Messaging;
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.DTOs.Social;
    using EventHub.Core.DTOs.UserProfile;
    using EventHub.Web.ViewModels.Chat;
    using EventHub.Web.ViewModels.Events;
    using EventHub.Web.ViewModels.Organizers;
    using EventHub.Web.ViewModels.Social;
    using EventHub.Web.ViewModels.UserProfile;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            CreateMap<EditEventViewModel, EditEventDto>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate!.Value))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate!.Value));

            CreateMap<EditEventDto, EditEventViewModel>();


            CreateMap<CreateEventViewModel, CreateEventDto>();


            CreateMap<DetailedEventDto, DetailedEventViewModel>()
                .ForMember(dest=>dest.Category, opt=>opt.MapFrom(src=>src.CategoryName))
                .ForMember(dest=>dest.CityName, opt=>opt.MapFrom(src=>src.City))
                .ForMember(dest=>dest.MaxParticipants, opt=>opt.MapFrom(src=>src.MaxParticipants))
                .ForMember(dest=>dest.ParticipantsCount, opt=>opt.MapFrom(src=>src.ParticipantsCount))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.ParticipantList));



            CreateMap<EventDto, EventListViewModel>()
                 .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City))
                 .ForMember(dest => dest.CanEdit,opt => opt.Ignore())
                 .ForMember(dest => dest.CanDelete,  opt => opt.Ignore())
                 .ForMember(dest => dest.HideJoinLeaveButtons,opt => opt.Ignore())
                 .ForMember(dest => dest.IsParticipant,opt => opt.Ignore())
                 .AfterMap((src, dest, context) =>
                 {
                     if (context.Items.TryGetValue("JoinedIds", out var value)
                         && value is HashSet<Guid> joinedIds)
                     {
                         dest.IsParticipant = joinedIds.Contains(src.Id);
                     }

                     if (context.Items.TryGetValue("IsAdminView", out var adminFlag)
                         && adminFlag is bool isAdminView
                         && isAdminView)
                     {
                         dest.CanEdit = true;
                         dest.CanDelete = true;
                     }

                     if (context.Items.TryGetValue("IsOrganizerView", out var orgFlag)
                         && orgFlag is bool isOrganizerView
                         && isOrganizerView)
                     {
                         dest.CanEdit = true;
                         dest.CanDelete = true;
                         dest.HideJoinLeaveButtons = true;
                     }
                     if (context.Items.TryGetValue("IsParticipantView",out var participantFlag)
                     && participantFlag is bool isParticipantView
                         && isParticipantView)
                     {
                         dest.IsParticipant = true;
                         dest.HideJoinLeaveButtons = false;
                     }
                 });

            CreateMap<PendingRequestForOrganizerDto, OrganizerRequestViewModel>();

            CreateMap<ApplyOrganizerForm, OrganizerRequestFormDto>();

            CreateMap<OrganizerRequestFormDto, ApplyOrganizerForm>();

            CreateMap<CreateUserProfileViewModel, CreateUserProfileDto>()

             .ForMember(dest => dest.InterestIds,  opt => opt.MapFrom(src => src.SelectedInterestIds))
             .ForMember(dest => dest.LocationId,  opt => opt.MapFrom(src => src.LocationId))
             .ForMember(dest => dest.ImagePath,  opt => opt.Ignore());


            CreateMap<DetailUserProfileDto, DetailedUserProfileViewModel>();

            CreateMap<EditUserProfileViewModel, EditUserProfileDto>()
    .ForMember(dest => dest.SelectedInterestIds,    opt => opt.MapFrom(src => src.SelectedInterestIds))
    .ForMember(dest => dest.ProfileImagePath, opt => opt.Ignore());


            CreateMap<OrganizerRequestDto, AllRequestsViewModel>();

            CreateMap<PublicUserProfileDto, PublicUserProfileViewModel>();

            CreateMap<SocialUserPreviewDto, SocialUserPreviewViewModel>();

            CreateMap<ConversationPreviewDto, ConversationPreviewViewModel>();

        }
    }
}
