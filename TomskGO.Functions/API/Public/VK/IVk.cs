using Refit;
using System.Threading.Tasks;
using TomskGO.Models.VK;

namespace TomskGO.Functions.API.Public.VK
{
    public interface IVk
    {
        [Get("/method/wall.get")]
        Task<ResponseModel<VKFeedModel>> GetPostsAsync(VKRequestModel request);
    }
}