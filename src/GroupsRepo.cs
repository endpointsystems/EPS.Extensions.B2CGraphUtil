using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil.Config;
using Microsoft.Graph;

namespace EPS.Extensions.B2CGraphUtil
{
    public class GroupsRepo: BaseRepo
    {
        public GroupsRepo(GraphUtilConfig config) : base(config)
        {
        }

        /// <summary>
        /// Get all of the groups in the directory.
        /// </summary>
        /// <returns>The list of group objects.</returns>
        public async Task<List<Group>> GetAllGroups()
        {
            int i = 0;
            var resp = await client.Groups.Request().GetAsync();
            var list = resp.CurrentPage.ToList();

            var pi = PageIterator<Group>.CreatePageIterator(client, resp, g =>
            {
                i++;
                list.Add(g);
                return i < resp.Count;
            });

            await pi.IterateAsync();
            return list;

        }

        public async Task DeleteGroup(string groupId)
        {
            await client.Groups[groupId].Request().DeleteAsync();
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
            var resp = await client.Groups.Request().AddAsync(group);
            return resp;
        }

    }
}
