using System.Collections.Generic;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace EPS.Extensions.B2CGraphUtil
{
    /// <summary>
    /// Repository for manipulating <see cref="Group"/> objects in the directory.
    /// </summary>
    public class GroupsRepo: BaseRepo
    {
        /// <summary>
        /// Create a new instance of the repo.
        /// </summary>
        /// <param name="config">The <see cref="GraphUtilConfig"/> instance.</param>
        public GroupsRepo(GraphUtilConfig config) : base(config)
        {
        }
        
        /// <summary>
        /// Create a new repo instance with logging.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public GroupsRepo(GraphUtilConfig config, ILogger<GroupsRepo> logger): base(config,logger){}

        /// <summary>
        /// Get all of the groups in the directory.
        /// </summary>
        /// <returns>The list of group objects.</returns>
        public async Task<List<Group>> GetAllGroups()
        {
            var resp = await client.Groups.GetAsync();
            var list = new List<Group>();

            foreach (var group in resp.Value)
            {
                list.Add(group);
            }
            return list;

        }

        /// <summary>
        /// Get a <see cref="Group"/> from the directory.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>A <see cref="Task"/> containing the <see cref="Group"/>.</returns>
        public Task<Group> GetGroup(string groupId)
        {
            return client.Groups[groupId].GetAsync();
        }

        /// <summary>
        /// Delete a group from the directory.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        public async Task DeleteGroup(string groupId)
        {
            await client.Groups[groupId].DeleteAsync();
        }

        /// <summary>
        /// Create a group within the directory with MailEnabled at false and SecurityEnabled at true.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <returns>The group object.</returns>
        public async Task<Group> CreateGroup(string groupName)
        {
            var group = new Group()
            {
                DisplayName = groupName,
                MailEnabled = false,
                MailNickname = groupName,
                SecurityEnabled = true
            };
            var resp = await client.Groups.PostAsync(group);
            return resp;
        }
        
        /// <summary>
        /// Create a group within the directory with MailEnabled at false and SecurityEnabled at true.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="desc">A description for the group.</param>
        /// <returns>The group object.</returns>
        public async Task<Group> CreateGroup(string groupName, string desc)
        {
            var group = new Group()
            {
                Description = desc,
                DisplayName = groupName,
                MailEnabled = false,
                MailNickname = groupName,
                SecurityEnabled = true
            };
            var resp = await client.Groups.PostAsync(group);
            return resp;
        }

    }
}
