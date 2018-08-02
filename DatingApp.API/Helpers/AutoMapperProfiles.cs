using System;
using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForList>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
            ;

            CreateMap<User, UserForDetail>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
            ;

            CreateMap<UserForUpdate, User>();
            CreateMap<UserForRegistration, User>();

            CreateMap<Photo, PhotoForUser>();
            CreateMap<Photo, PhotoForReturn>();

            CreateMap<PhotoForCreation, Photo>();

            CreateMap<Message, MessageForList>();
            CreateMap<MessageForCreation, Message>();
        }
    }
}