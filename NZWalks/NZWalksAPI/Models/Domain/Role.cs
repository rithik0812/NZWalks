namespace NZWalksAPI.Models.Domain
{
    public class Role
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        // Navigation property
        public List<User_roles> UserRoles { get; set; }

    }
}
