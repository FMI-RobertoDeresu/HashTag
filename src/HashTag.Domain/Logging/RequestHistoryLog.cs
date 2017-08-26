using System;

namespace HashTag.Domain.Logging
{
    public class RequestHistoryLog : EntityBase<long>
    {
        public DateTime Created { get; set; }

        public string Ip { get; set; }

        public string Username { get; set; }

        public string HttpMethod { get; set; }

        public string Url { get; set; }

        public string UrlReferrer { get; set; }

        public string Logger { get; set; }
    }
}