using System.Threading.Tasks;

namespace OCRBot.Services.AzureStorage
{
    public interface IAzureStorageService
    {
        Task<string> Execute(byte[] data, string type);
    }
}