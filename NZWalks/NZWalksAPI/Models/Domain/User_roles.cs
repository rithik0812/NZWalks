namespace NZWalksAPI.Models.Domain
{
    public class User_roles
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public User User { get; set; }
        public Guid RoleID { get; set; }
        public Role Role { get; set; }

    }
}
