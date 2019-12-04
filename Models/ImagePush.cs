using System;

namespace AcrUnleashed.Webhooks.Models
{
    public class ImagePush
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public string LoginServer { get; set; }
        public string Image { get; set; }
        public string Tag { get; set; }
    }
}
