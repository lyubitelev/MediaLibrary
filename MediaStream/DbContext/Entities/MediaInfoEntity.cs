namespace MediaStream.DbContext.Entities
{
    public class MediaInfoEntity : BaseEntity
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Extension { get; set; }
        public DateTime CreationTime { get; set; }
        public byte[] PreviewImage { get; set; }
        public int LastViewedMin { get; set; }
        public bool IsDeleted { get; set; }
    }
}
