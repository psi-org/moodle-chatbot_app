using MoodleBot.Persistent.Entity;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public interface ITwilioPhoneNumberAPI
    {
        Task<WhatsAppNumberDetails> GetWhatsAppNumberDetails(string whatsAppNumber);
    }
}
