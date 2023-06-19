using AutoMapper;

namespace NZWalksAPI.Profiles
{
    public class WalksDifficultyProfile : Profile
    {
        public WalksDifficultyProfile()
        {
            CreateMap<Models.Domain.WalkDifficulty, Models.DTO.WalkDifficulty>()
                .ReverseMap();
            // the reverse method revereses the mapping from the DTO to the Domain
        }
    }
}
