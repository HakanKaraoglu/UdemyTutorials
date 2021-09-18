using AutoMapper;
using BandApi.DataTransferObjects;
using BandApi.Entities;
using BandApi.Helpers;

namespace BandApi.Profiles
{
    public class BandProfile : Profile
    {
        public BandProfile()
        {
            CreateMap<Band, BandDto>()
                .ForMember(m => m.FoundedYearsAgo, opt =>
                {
                    opt.MapFrom(src => $"Founded in {src.Founded:yyyy} ({src.Founded.FoundedYearsAgo()} years ago)");
                });

            CreateMap<BandCreationDto, Band>();
        }
    }
}
