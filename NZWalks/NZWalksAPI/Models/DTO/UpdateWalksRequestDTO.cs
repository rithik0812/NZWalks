namespace NZWalksAPI.Models.DTO
{
    public class UpdateWalksRequestDTO
    {
        public string Name { get; set; }
        public double Length { get; set; }

        public Guid RegionID { get; set; }
        public Guid WalkDifficultyID { get; set; }
    }
}
