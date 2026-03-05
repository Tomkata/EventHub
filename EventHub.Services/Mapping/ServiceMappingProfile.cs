using AutoMapper;
using EventHub.Core.Common;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Category;
using EventHub.Core.DTOs.Event;
using EventHub.Core.DTOs.Interest;
using EventHub.Core.DTOs.Location;
using EventHub.Core.DTOs.Messaging;
using EventHub.Core.DTOs.Organizer;
using EventHub.Core.DTOs.Social;
using EventHub.Core.DTOs.UserProfile;
using EventHub.Core.Models.Common;
using EventHub.Core.Models.Events;
using EventHub.Core.Models.Messaging;
using EventHub.Core.Models.Organizer;
using EventHub.Core.Models.Users;

namespace EventHub.Services.Mapping
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<Event, EventDto>()
             .ForMember(dest => dest.City,
                 opt => opt.MapFrom(src => src.Location.City))
             .ForMember(dest => dest.Category,
                 opt => opt.MapFrom(src => src.Category.Name))
             .ForMember(dest => dest.ParticipantsCount,
                 opt => opt.MapFrom(src => src.EventParticipants.Count));


            CreateMap<Event, DetailedEventDto>()
               .ForMember(dest => dest.CategoryName,
                   opt => opt.MapFrom(src => src.Category.Name))
               .ForMember(dest => dest.City,
                   opt => opt.MapFrom(src => src.Location.City))
               .ForMember(dest => dest.ParticipantList, opt => opt.Ignore())
               .ForMember(dest => dest.ParticipantsCount, opt => opt.Ignore())
               .ForMember(dest => dest.OrganizerName, opt => opt.Ignore());



            CreateMap<Event, EditEventDto>();

            CreateMap<DetailedEventDto, EditEventDto>();

            CreateMap<UserBasicInfo, ParticipantDto>();

            CreateMap<CreateEventDto, Event>();


            CreateMap<Category, CategoryDto>();

            CreateMap<OrganizerRequest, PendingRequestForOrganizerDto>();

            CreateMap<CreateUserProfileDto, UserProfile>()
                .ForMember(dest => dest.ProfileImagePath, 
                opt => opt.MapFrom(x => x.ImagePath))
                 .ForMember(dest => dest.Phone, 
                 opt => opt.MapFrom(x => x.PhoneNumber));

            CreateMap<Location, DropdownOptionModel>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.City));

            CreateMap<Interest, DropdownOptionModel>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.InterestName));


            CreateMap<OrganizerRequest, OrganizerRequestDto>();

            CreateMap<UserProfile, SocialUserPreviewDto>()
             .ForMember(dest => dest.UserId,
                 opt => opt.MapFrom(src => src.UserId))
             .ForMember(dest => dest.DisplayName,
                 opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
             .ForMember(dest => dest.Location,
                 opt => opt.MapFrom(src => src.Location.City))
             .ForMember(dest => dest.ProfileImagePath,
                 opt => opt.MapFrom(src => src.ProfileImagePath));


        }
    }
}
