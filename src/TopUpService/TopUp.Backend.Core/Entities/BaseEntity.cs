namespace Backend.TopUp.Core.Entities
{
    public abstract class BaseEntity<IdType>(DateTimeOffset createdAt, DateTimeOffset? updatedAt)
    {
        public IdType Id { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; } = createdAt;
        public DateTimeOffset? UpdatedAt { get; private set; } = updatedAt;
    }
}
