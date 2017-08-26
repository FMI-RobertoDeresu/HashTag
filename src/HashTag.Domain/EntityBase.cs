namespace HashTag.Domain
{
    public abstract class EntityBase<TKey>
    {
        public TKey Id { get; protected set; }
    }
}