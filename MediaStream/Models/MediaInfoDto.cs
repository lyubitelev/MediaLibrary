namespace MediaStream.Models
{
    public class MediaInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string FullName { get; set; }
        public string CreationTime { get; set; }
        //ToDo maybe just a link
        public byte[] PreviewImage { get; set; }
    }
}
