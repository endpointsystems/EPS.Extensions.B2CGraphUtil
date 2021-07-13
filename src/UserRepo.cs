using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Graph;
using User = Microsoft.Graph.User;
// ReSharper disable PartialTypeWithSinglePart

namespace EPS.Extensions.B2CGraphUtil
{
    /// <summary>
    /// Repository of <see cref="Microsoft.Graph.User"/> objects found in the B2C Graph.
    /// </summary>
    public partial class UserRepo: BaseRepo
    {

        /// <summary>
        /// Create a new instance of the <see cref="User"/> graph repository.
        /// </summary>
        /// <param name="config">The configuration object instance.</param>
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
        /// Checks for the existence of a <see cref="User"/> based on their User Principal Name.
        /// </summary>
        /// <param name="upn">their User Principal Name</param>
        /// <returns><c>true</c> if they exist in the directory.</returns>
        public async Task<bool> Exists(string upn)
        {
            var u = await client.Users.Request()
                .Filter($"userPrincipalName eq '{upn}'").GetAsync();
            return u.Count > 0;
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

        /// <summary>
        /// Delete a <see cref="User"/> from the directory.
        /// </summary>
        /// <param name="id">The user's identifier.</param>
        public async Task DeleteUser(string id)
        {
            await client.Users[id].Request().DeleteAsync();
        }

        /// <summary>
        /// Get the <see cref="User"/> by their UserPrincipalName.
        /// </summary>
        /// <param name="upn">the userPrincipalName.</param>
        /// <returns>The <see cref="User"/> or null if they do not exist.</returns>
        public async Task<User> GetUserByUPN(string upn)
        {
            var u = await client.Users.Request()
                .Filter($"userPrincipalName eq '{upn}'").GetAsync();

            return u.Count > 0 ? u.First() : null;
        }

        /// <summary>
        /// Get a <see cref="User"/> from the directory.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>The <see cref="User"/>.</returns>
        public async Task<User> GetUser(string userId)
        {
            return await client.Users[userId].Request().GetAsync();
        }

        /// <summary>
        /// Used to confirm the <see cref="User"/> is a member of said <see cref="Group"/>.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> identifier.</param>
        /// <param name="groupId">The <see cref="Group"/> identifier.</param>
        /// <returns><c>true</c> if the <see cref="User"/> is a member of the <see cref="Group"/>.</returns>
        public async Task<bool> MemberOf(string userId, string groupId)
        {
            DirectoryObject resp;
            try
            {
                resp = await client.Users[userId].MemberOf[groupId].Request().GetAsync();
            }
            catch (ServiceException se)
            {
                if (se.StatusCode == HttpStatusCode.NotFound) return false;
                throw;
            }
            return resp != null;
        }

        /// <summary>
        /// Add a <see cref="User"/> to the specified <see cref="Group"/>.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> identifier</param>
        /// <param name="groupId">The <see cref="Group"/> identifier</param>
        public async Task AddToGroup(string userId, string groupId)
        {
            var d = new DirectoryObject() {Id = userId};
            await client.Groups[groupId].Members.References.Request().AddAsync(d);
        }

        /// <summary>
        /// Discovery of what <see cref="Group"/> objects a <see cref="User"/> is a member of.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> identifier.</param>
        /// <returns>A list of <see cref="DirectoryObject"/> values containing the <see cref="User"/></returns>
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
