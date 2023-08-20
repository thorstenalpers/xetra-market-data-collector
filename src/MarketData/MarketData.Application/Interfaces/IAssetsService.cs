namespace MarketData.Application.Interfaces;
using System.Threading.Tasks;

public interface IAssetsService
{
    Task CreateAssets();
    Task<int> DeleteAssets(int daysWithNoCourses);
}