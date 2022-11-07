namespace MediaStream.Interfaces
{
    public interface IMediaFileRepository
    {
        Task<string> GetFullPathByNameAsync(string fileName, CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetAllVideoFilesNameAsync(CancellationToken cancellationToken);
    }
}
