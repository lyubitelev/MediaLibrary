namespace MediaStream.Impl.DbContext.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
