using MoodleBot.Persistent.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public interface ICertificate
    {
        Task<List<MoodleCertificateDetail>> GetCourseCertificateDetail(long userId, long courseId);
    }
}
