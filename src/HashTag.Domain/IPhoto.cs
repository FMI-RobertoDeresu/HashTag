namespace HashTag.Domain
{
    public interface IPhoto
    {
        string Description { get; set; }

        double[] Prediction { get; set; }
    }
}