using Microsoft.Data.SqlClient;

namespace RequestFlow.Infrastructure.Persistence;

public static class SqlServerConnectionStringFactory
{
    public static string Create(
        string server,
        string database,
        string user,
        string password)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = user,
            Password = password,
            TrustServerCertificate = true,
            MultipleActiveResultSets = true
        };

        return builder.ConnectionString;
    }
}
