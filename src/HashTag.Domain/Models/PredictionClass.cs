namespace HashTag.Domain.Models
{
    public class PredictionClass : EntityBase<long>
    {
        protected PredictionClass() { }

        public PredictionClass(int index, string @class)
        {
            Index = index;
            Class = @class;
        }

        public int Index { get; protected set; }

        public string Class { get; protected set; }
    }
}