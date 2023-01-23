namespace MediaStream.Impl.DbContext.Entities
{
    public class MediaInfoEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Theme { get; set; }
        public string FullName { get; set; }
        public string Extension { get; set; }
        public int LastViewedMin { get; set; }
        public byte[] PreviewImage { get; set; }
        public string FullSearchText { get; set; }
        public bool IsLiked { get; set; }
    }
}
