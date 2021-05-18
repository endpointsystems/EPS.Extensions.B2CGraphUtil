using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Graph;
using User = Microsoft.Graph.User;

namespace EPS.Extensions.B2CGraphUtil
{
    /// <summary>
    /// Repository of <see cref="Microsoft.Graph.User"/> objects found in the B2C Graph.
    /// </summary>
    public partial class UserRepo: BaseRepo
    {

        public UserRepo(GraphUtilConfig config): base(config)
        {
        }

        /// <summary>
        /// Add the provided User to the graph.
        /// </summary>
        /// <param name="user">The User.</param>
        /// <returns>The updated User object.</returns>
        /// <remarks>Required fields are: DisplayName, PasswordProfile,AccountEnabled (true or false),
        /// MailNickname, UserPrincipalName.
        /// </remarks>
        public async Task<User> AddUser(User user)
        {
            var ret = await client.Users.Request().AddAsync(user);
            return ret;
        }


        /// <summary>
        /// Create a new user with the parameters provided.
        /// </summary>
        /// <param name="firstName">The user's first name.</param>
        /// <param name="lastName">The user's last name.</param>
        /// <param name="displayName">The user's display name.</param>
        /// <param name="pwd">The user's plain-text password.</param>
        /// <returns>A new local User object from the graph.</returns>
        /// <remarks>
        /// The required MailNickname will be the first name, a dot, and a last name. The required
        /// userPrincipalName will be the same with the first domain (the .onmicrosoft.com) added
        /// to the end of the name.
        /// </remarks>
        public async Task<User> AddUser(string firstName, string lastName, string displayName, string pwd)
        {
            var user = new User
            {
                GivenName = firstName, Surname = lastName, DisplayName = displayName,
                PasswordProfile = new PasswordProfile {Password = pwd},
                AccountEnabled = false,
                MailNickname = firstName + "." + lastName,
                UserPrincipalName = firstName + "." + lastName + "@" + domains[0].Id
            };

            var ret = await client.Users.Request().AddAsync(user);
            return ret;
        }

        public async Task DeleteUser(string id)
        {
            await client.Users[id].Request().DeleteAsync();
        }

        public async Task<User> GetUser(string userId)
        {
            return await client.Users[userId].Request().GetAsync();
        }

        public async Task<bool> MemberOf(string userId, string groupId)
        {
            var resp = await client.Users[userId].MemberOf[groupId].Request().GetAsync();
            return resp != null;
        }

        public async Task AddToGroup(string userId, string groupId)
        {
            var d = new DirectoryObject() {Id = userId};
            await client.Groups[groupId].Members.References.Request().AddAsync(d);
        }

        public async Task<List<DirectoryObject>> MemberOf(string userId)
        {
            int i = 0;
            var resp = await client.Users[userId].MemberOf.Request().GetAsync();
            var list = resp.CurrentPage.ToList();
            var pi = PageIterator<DirectoryObject>.CreatePageIterator(client, resp, d =>
            {
                i++;
                list.Add(d);
                return i < resp.Count;
            });
            await pi.IterateAsync();
            return list;
        }

        /// <summary>
        /// Returns the entire directory of users.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            int i = 0;

            var result = await client.Users.Request().GetAsync();
            var list = result.CurrentPage.ToList();
            var pi = PageIterator<User>.CreatePageIterator(client, result, user =>
            {
                i++;
                list.Add(user);
                return i < result.Count;
            });

            await pi.IterateAsync();

            return list;
        }
    }
}
