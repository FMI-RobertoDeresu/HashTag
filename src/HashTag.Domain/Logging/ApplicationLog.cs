namespace HashTag.Domain.Logging
{
    public class ApplicationLog : EntityBase<long>
    {
        public string Created { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string Trace { get; set; }

        public string Logger { get; set; }
    }
}