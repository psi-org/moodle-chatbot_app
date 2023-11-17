using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBContext
{
    public interface IStoreWrapper
    {
        void Save();

        Task SaveAsync();
    }
}
