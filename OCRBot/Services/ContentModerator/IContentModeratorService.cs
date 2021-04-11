using System.Threading.Tasks;

namespace OCRBot.Services.ContentModerator
{
    public interface IContentModeratorService
    {
        Task<string> processImage(string imageUrl);
    }
}