using System;

namespace rbxMessageExport
{
    public class Sender
    {
        public long id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class Recipient
    {
        public long id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class Message
    {
        public long id { get; set; }
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public bool isRead { get; set; }
        public bool isSystemMessage { get; set; }
        public bool isReportAbuseDisplayed { get; set; }
    }
}