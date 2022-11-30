namespace MediaStream.Interfaces.DbContext
{
    public interface IDbContextFactory
    {
        IDbContext CreateContext();
    }
}
