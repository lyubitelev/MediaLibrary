namespace MediaStream.Interfaces
{
    public interface IDbContextFactory
    {
        IDbContext CreateContext();
    }
}
