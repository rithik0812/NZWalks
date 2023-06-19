using AutoMapper;

namespace NZWalksAPI.Profiles
{
    public class WalksProfile : Profile
    {
        public WalksProfile()
        {
            CreateMap<Models.Domain.Walk, Models.DTO.Walk>()
                .ReverseMap();
            // the reverse method revereses the mapping from the DTO to the Domain
        }
    }
}
