using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomskGO.Models.VK;

namespace TomskGO.Functions.API.Public.VK
{
    public interface IVK
    {
        [Get("/method/wall.get")]
        Task<ResponseModel<VKFeedModel>> GetPostsAsync(VKNewsFeedRequestModel request);
        
        [Get("/method/users.get")]
        Task<ResponseModel<List<VKUserModel>>> GetUsersInformationAsync(VKUsersInformationRequestModel request);
        
        [Get("/method/groups.getById")]
        Task<ResponseModel<List<VKGroupModel>>> GetGroupsInformationAsync(VKGroupsInformationRequestModel request);
    }
}