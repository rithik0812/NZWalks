using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private NZWalksDBContext nZWalksDBContext;

        public UserRepository(NZWalksDBContext nZWalksDBContext)
        {
            this.nZWalksDBContext = nZWalksDBContext;
        }
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            // if the user credentials match in the Db then return that user from the user DB
            var user = await nZWalksDBContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower() && x.Password == password);

            // if no users match the given credentials in the DB then return null
            if (user == null) 
            {
                return null;
            }

            // if the user credentials matches a user in the Db, then find all the role IDs for that specific user from the User_roles DB
            var AllRoleIDsForThisUser = await nZWalksDBContext.Users_Roles.Where(x => x.UserID == user.ID).ToListAsync();

            // if the list of role IDs for that user is not empty then append then get back the individual roles for each role ID
            // then append the (one/many) role(s) into the user.Roles (object) created in User.cs
            // user = {..., new List(){"reader", "writer"}} thats what the data would look like after this loop

            if (AllRoleIDsForThisUser.Any()) 
            {
                user.Roles = new List<string>();
                foreach (var userRoleID in AllRoleIDsForThisUser)  
                {
                    var role = await nZWalksDBContext.Roles.FirstOrDefaultAsync(x => x.ID == userRoleID.RoleID);

                    if (role != null) 
                    {
                        user.Roles.Add(role.Name);
                    }
                }
            }

            // get rid of the password since we dont want that to be visible in other areas of the application
            user.Password = null; 
            return user;
        }
    }
}
