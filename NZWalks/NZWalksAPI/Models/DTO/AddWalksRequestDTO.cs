using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Models.DTO
{
    public class AddWalksRequestDTO
    {
        public string Name { get; set; }
        public double Length { get; set; }

        public Guid RegionID { get; set; }
        public Guid WalkDifficultyID { get; set; }

    }
}
