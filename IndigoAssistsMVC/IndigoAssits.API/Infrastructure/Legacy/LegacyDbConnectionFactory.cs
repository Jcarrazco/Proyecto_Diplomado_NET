using Microsoft.Data.SqlClient;

namespace IndigoAssits.API.Infrastructure.Legacy
{
    public interface ILegacyDbConnectionFactory
    {
        SqlConnection CreateConnection();
        string CurrentSucursal { get; }
    }

    public sealed class LegacyDbConnectionFactory : ILegacyDbConnectionFactory
    {
        private readonly ILegacyConnectionResolver _resolver;

        public LegacyDbConnectionFactory(ILegacyConnectionResolver resolver)
        {
            _resolver = resolver;
        }

        public string CurrentSucursal => _resolver.CurrentSucursal;

        public SqlConnection CreateConnection()
        {
            var connectionString = _resolver.Resolve();
            return new SqlConnection(connectionString);
        }
    }
}
