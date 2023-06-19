using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Models.DTO
{
    public class Walk
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public double Length { get; set; }

        public Guid RegionID { get; set; }
        public Guid WalkDifficultyID { get; set; }

        // Navigation Properties
        public Region Region { get; set; }
        public WalkDifficulty WalkDifficulty { get; set; }
    }
}