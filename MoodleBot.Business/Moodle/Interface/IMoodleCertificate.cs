using MoodleBot.Business.Entity;
using MoodleBot.Persistent.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business.Moodle
{
    public interface IMoodleCertificate
    {
        Task<List<MoodleCertificateDetail>> GetCertificateDetail(long userId, long courseId);
        Task<CertificateDetailMessage> PrepareCourseCertificate(long userId, long courseId);
        Task<CertificateDetailMessage> PrepareCourseCertificate(List<MoodleCertificateDetail> certificateDetails, long userId, long courseId);
    }
}
