namespace AbstractionBlocks.CommonDomain
{
    public class Entity
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public Guid CreateById { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public Guid? UpdatedById { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;
    }
}
