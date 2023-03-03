using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil.Config;
using EPS.Extensions.B2CGraphUtil.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Polly;
using User = Microsoft.Graph.Models.User;
// ReSharper disable PartialTypeWithSinglePart

namespace EPS.Extensions.B2CGraphUtil
{
    /// <summary>
    /// Repository of <see cref="User"/> objects found in the B2C Graph.
    /// </summary>
    public partial class UserRepo: BaseRepo
    {

        /// <summary>
        /// Create a new instance of the <see cref="User"/> graph repository.
        /// </summary>
        /// <param name="config">The configuration object instance.</param>
        public UserRepo(GraphUtilConfig config): base(config){ }

        /// <summary>
        /// Create a new instance of the <see cref="User"/> graph repository, with logging.
        /// </summary>
        /// <param name="config">The graph configuration.</param>
        /// <param name="logger">The logger.</param>
        public UserRepo(GraphUtilConfig config, ILogger<UserRepo> logger): base(config, logger){ }

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
            try
            {
                User ret = null;
                await Policy.Handle<Exception>().RetryAsync(graphUtilConfig.RetryCount, (ex, i) =>
                {
                    warn($"{ex.GetType()} on attempt {i} of {graphUtilConfig.RetryCount} to add user: {ex.Message}. Retrying...");
                }).ExecuteAsync(async () => ret = await client.Users.PostAsync(user));

                return ret;
            }
            catch (ServiceException se)
            {
                throw new UserException(
                    $"An exception occured adding user {user.UserPrincipalName} to the directory: {se.Message} Check the inner exception for details.",
                    user, se);
            }
        }

        /// <summary>
        /// Update the user in the directory.
        /// </summary>
        /// <param name="user"></param>
        public async Task UpdateUser(User user)
        {
            await Policy.Handle<Exception>().RetryAsync(graphUtilConfig.RetryCount, (ex, i) =>
            {
                warn($"{ex.GetType()} on attempt {i} of {graphUtilConfig.RetryCount} to update user: {ex.Message}. Retrying...");
            }).ExecuteAsync(async () => await client.Users[user.Id].PatchAsync(user));
        }

        /// <summary>
        /// Checks for the existence of a <see cref="User"/> based on their User Principal Name.
        /// </summary>
        /// <param name="upn">their User Principal Name</param>
        /// <returns><c>true</c> if they exist in the directory.</returns>
        public async Task<bool> Exists(string upn)
        {
            try
            {
                var u = await client.Users.GetAsync(config =>
                {
                    config.QueryParameters.Select = new[] { "userPrincipalName", upn };
                });
                return u.Value.Count > 0;
            }
            catch (ServiceException se)
            {
                err($"An exception occured checking the existence of user user {upn} to the directory: {se.Message} Check the inner exception for details.",se);
                throw;
            }
        }

        /// <summary>
        /// Find user by Other Mails (otherMails).
        /// </summary>
        /// <param name="email">The other email</param>
        /// <returns>The user</returns>
        /// <remarks>
        /// In the Azure AD B2C directory, if someone registers using an external identity provider, you can
        /// get their email address from the otherMails property of the User object.
        /// </remarks>
        public async Task<User?> FindUserByOtherMails(string email)
        {
            //queries that didn't work
            // $"identities/any(id:id eq '{email}')"
            var users = await client.Users.GetAsync(config =>
            {
                config.QueryParameters.Filter = $"otherMails/any(id:id eq '{email}')";
            });
            return users.Value.Count == 0 ? null : users.Value.First();
        }

        /// <summary>
        /// Get the user (object) id of a user in the directory by their email address.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <returns></returns>
        public async Task<string?> FindUserIdByOtherMails(string email)
        {
            var user = await FindUserByOtherMails(email);
            return user?.Id;
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
                PasswordProfile = new PasswordProfile
                {
                    Password = pwd,
                    ForceChangePasswordNextSignIn = false
                },
                PasswordPolicies = "DisablePasswordExpiration",
                AccountEnabled = false,
                MailNickname = firstName + "." + lastName,
                UserPrincipalName = firstName + "." + lastName + "@" + domain.Id
            };
            try
            {
                var ret = await Policy.Handle<Exception>()
                    .RetryAsync(graphUtilConfig.RetryCount, (ex, i) =>
                    {
                        warn($"{ex.GetType()} on attempt {i} of {graphUtilConfig.RetryCount} to add new user: {ex.Message}. Retrying...");
                    })
                    .ExecuteAsync(async () => await client.Users.PostAsync(user));
                return ret;
            }
            catch (ServiceException se)
            {
                throw new UserException(
                    $"An exception occured building and adding user {user.UserPrincipalName} to the directory: {se.Message} Check the inner exception for details.",
                    user, se);
            }
        }

        /// <summary>
        /// Delete a <see cref="User"/> from the directory.
        /// </summary>
        /// <param name="id">The user's identifier.</param>
        public async Task DeleteUser(string id)
        {
            await client.Users[id].DeleteAsync();
        }

        /// <summary>
        /// Get the <see cref="User"/> by their UserPrincipalName.
        /// </summary>
        /// <param name="upn">the userPrincipalName.</param>
        /// <returns>The <see cref="User"/> or null if they do not exist.</returns>
        public async Task<User> GetUserByUPN(string upn)
        {
            var u = await client.Users.GetAsync(config =>
            {
                config.QueryParameters.Filter = $"userPrincipalName eq '{upn}'";
            });

            return u.Value.Count > 0 ? u.Value.FirstOrDefault() : null;
        }

        /// <summary>
        /// Get a <see cref="User"/> from the directory.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>The <see cref="User"/>.</returns>
        public async Task<User> GetUser(string userId)
        {
            return await client.Users[userId].GetAsync();
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
                resp = await client.Users[userId].MemberOf[groupId].GetAsync();
            }
            catch (ServiceException)
            {
                return false;
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
            await Policy.Handle<Exception>().RetryAsync(graphUtilConfig.RetryCount, (ex, i) =>
            {
                warn(
                    $"{ex.GetType()} on attempt {i} of {graphUtilConfig.RetryCount} to add new user: {ex.Message}. Retrying...");
            }).ExecuteAsync(async () => await client.Groups[groupId].Members.GetAsync());
        }

        /// <summary>
        /// Discovery of what <see cref="Group"/> objects a <see cref="User"/> is a member of.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> identifier.</param>
        /// <returns>A list of <see cref="DirectoryObject"/> values containing the <see cref="User"/></returns>
        public async Task<List<DirectoryObject>> MemberOf(string userId)
        {
            var resp = await client.Users[userId].MemberOf.GetAsync();
            return resp.Value;
        }

        /// <summary>
        /// Get the list of groups that the user is a member of.
        /// </summary>
        /// <param name="userId">The user to test against.</param>
        /// <returns>A list of groups.</returns>
        /// <remarks>
        /// This code will catch an exception if the group isn't part of the Azure Active Directory
        /// Groups collection (i.e. Global administrator or anything listed in Azure AD B2C | Roles and administrators)
        /// </remarks>
        public async Task<List<Group>> GetMemberGroupListAsync(string userId)
        {
            try
            {
                var objs = await client.Users[userId].MemberOf.GraphGroup.GetAsync();
                return objs.Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Get the list of directory objects that the user is a member of.
        /// </summary>
        /// <param name="userId">The user to test against.</param>
        /// <returns>A list of <see cref="DirectoryObject"/>.</returns>
        /// <remarks>
        /// This code will catch an exception if the group isn't part of the Azure Active Directory
        /// Groups collection (i.e. Global administrator or anything listed in Azure AD B2C | Roles and administrators)
        /// </remarks>
        public async Task<List<DirectoryObject>> GetMemberDirectoryObjectsAsync(string userId)
        {
            var dirObjs = await client.Users[userId].MemberOf.GetAsync();
            return dirObjs.Value;
        }

        /// <summary>
        /// Returns the entire directory of users.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            var result = await client.Users.GetAsync();
            return result.Value;
        }
    }
}
