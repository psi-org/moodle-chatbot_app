using System;
using System.Collections.Generic;
using System.Text;

namespace MoodleBot.Business.Entity
{
    public class CertificateDetailMessage
    {
        public string Message { get; set; }
        public List<string> CertificateUrl { get; set; }
        public bool IsCertificateAvailable { get; set; }
    }
}
