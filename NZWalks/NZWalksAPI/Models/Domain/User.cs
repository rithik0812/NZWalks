namespace NZWalksAPI.Models.Domain
{
    public class User
    {
        public Guid ID { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
