
namespace MoodleBot.Models
{
    public class Log : BaseEntity
    {
        protected Log()
        {

        }

        public string EventLevel { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string EventMessage { get; set; }
        public string Exception { get; set; }
        public int? UserId { get; set; }
    }
}
