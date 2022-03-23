using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Data.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(
                    destination => destination.PhotoUrl, 
                    options => options.MapFrom(source => source.Photos != null 
                        ? source.Photos.First(photo => photo.IsMain).Url
                        : null
                    )
                )
                .ForMember(
                    destination => destination.Age,
                    options => options.MapFrom(source => source.DateOfBirth.CalculateAge())
                );
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
        }
    }
}