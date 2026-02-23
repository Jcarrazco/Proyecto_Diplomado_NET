namespace IndigoAssits.Repositorio.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el patrón Unit of Work - manejo de transacciones
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repositorios específicos
        ITicketRepository Tickets { get; }
        ICategoriaRepository Categorias { get; }
        IUsuarioRepository Usuarios { get; }
        IDepartamentoRepository Departamentos { get; }
        IEmpleadoRepository Empleados { get; }
        IActivoRepository Activos { get; }

        // Repositorio genérico
        IGenericRepository<T> Repository<T>() where T : class;

        // Operaciones de transacción
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        
        // Operaciones de transacción con resultado
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
