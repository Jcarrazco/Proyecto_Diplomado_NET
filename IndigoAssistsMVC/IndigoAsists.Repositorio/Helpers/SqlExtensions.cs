using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IndigoAsists.Repositorio.Helpers
{
    /// <summary>
    /// Extensiones útiles para consultas SQL y Entity Framework
    /// </summary>
    public static class SqlExtensions
    {
        /// <summary>
        /// Aplica paginación a una consulta IQueryable
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="query">Consulta a paginar</param>
        /// <param name="page">Número de página (base 1)</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Consulta paginada</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Aplica ordenamiento dinámico a una consulta IQueryable
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="query">Consulta a ordenar</param>
        /// <param name="orderBy">Expresión de ordenamiento</param>
        /// <param name="ascending">Si es ascendente o descendente</param>
        /// <returns>Consulta ordenada</returns>
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, Expression<Func<T, object>> orderBy, bool ascending = true)
        {
            return ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        /// <summary>
        /// Aplica filtro de texto a una consulta usando LIKE
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="query">Consulta a filtrar</param>
        /// <param name="property">Propiedad a buscar</param>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Consulta filtrada</returns>
        public static IQueryable<T> WhereContains<T>(this IQueryable<T> query, Expression<Func<T, string>> property, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Invoke(property, parameter);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var containsCall = Expression.Call(propertyAccess, containsMethod, Expression.Constant(searchTerm));
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            return query.Where(lambda);
        }

        /// <summary>
        /// Aplica filtro de rango de fechas
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="query">Consulta a filtrar</param>
        /// <param name="property">Propiedad de fecha</param>
        /// <param name="startDate">Fecha inicio</param>
        /// <param name="endDate">Fecha fin</param>
        /// <returns>Consulta filtrada</returns>
        public static IQueryable<T> WhereDateRange<T>(this IQueryable<T> query, Expression<Func<T, DateTime>> property, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                var startParameter = Expression.Parameter(typeof(T), "x");
                var startPropertyAccess = Expression.Invoke(property, startParameter);
                var startComparison = Expression.GreaterThanOrEqual(startPropertyAccess, Expression.Constant(startDate.Value));
                var startLambda = Expression.Lambda<Func<T, bool>>(startComparison, startParameter);
                query = query.Where(startLambda);
            }

            if (endDate.HasValue)
            {
                var endParameter = Expression.Parameter(typeof(T), "x");
                var endPropertyAccess = Expression.Invoke(property, endParameter);
                var endComparison = Expression.LessThanOrEqual(endPropertyAccess, Expression.Constant(endDate.Value));
                var endLambda = Expression.Lambda<Func<T, bool>>(endComparison, endParameter);
                query = query.Where(endLambda);
            }

            return query;
        }

        /// <summary>
        /// Aplica filtro de rango de fechas nullable
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="query">Consulta a filtrar</param>
        /// <param name="property">Propiedad de fecha nullable</param>
        /// <param name="startDate">Fecha inicio</param>
        /// <param name="endDate">Fecha fin</param>
        /// <returns>Consulta filtrada</returns>
        public static IQueryable<T> WhereDateRange<T>(this IQueryable<T> query, Expression<Func<T, DateTime?>> property, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                var startParameter = Expression.Parameter(typeof(T), "x");
                var startPropertyAccess = Expression.Invoke(property, startParameter);
                var startComparison = Expression.GreaterThanOrEqual(startPropertyAccess, Expression.Constant(startDate.Value));
                var startLambda = Expression.Lambda<Func<T, bool>>(startComparison, startParameter);
                query = query.Where(startLambda);
            }

            if (endDate.HasValue)
            {
                var endParameter = Expression.Parameter(typeof(T), "x");
                var endPropertyAccess = Expression.Invoke(property, endParameter);
                var endComparison = Expression.LessThanOrEqual(endPropertyAccess, Expression.Constant(endDate.Value));
                var endLambda = Expression.Lambda<Func<T, bool>>(endComparison, endParameter);
                query = query.Where(endLambda);
            }

            return query;
        }

        /// <summary>
        /// Ejecuta una consulta SQL raw de forma segura
        /// </summary>
        /// <typeparam name="T">Tipo de resultado</typeparam>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="sql">Consulta SQL</param>
        /// <param name="parameters">Parámetros de la consulta</param>
        /// <returns>Resultado de la consulta</returns>
        public static async Task<IEnumerable<T>> ExecuteRawQueryAsync<T>(this DbContext context, string sql, params object[] parameters)
        {
            return await context.Database.SqlQueryRaw<T>(sql, parameters).ToListAsync();
        }

        /// <summary>
        /// Ejecuta un comando SQL raw de forma segura
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="sql">Comando SQL</param>
        /// <param name="parameters">Parámetros del comando</param>
        /// <returns>Número de filas afectadas</returns>
        public static async Task<int> ExecuteRawCommandAsync(this DbContext context, string sql, params object[] parameters)
        {
            return await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        /// <summary>
        /// Obtiene el siguiente ID disponible para una tabla
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="tableName">Nombre de la tabla</param>
        /// <param name="idColumnName">Nombre de la columna ID</param>
        /// <returns>Siguiente ID disponible</returns>
        public static async Task<int> GetNextIdAsync(this DbContext context, string tableName, string idColumnName = "Id")
        {
            var sql = $"SELECT ISNULL(MAX({idColumnName}), 0) + 1 FROM {tableName}";
            var result = await context.Database.ExecuteSqlRawAsync(sql);
            return result;
        }

        /// <summary>
        /// Aplica filtro de texto en múltiples propiedades
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="query">Consulta a filtrar</param>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <param name="properties">Propiedades donde buscar</param>
        /// <returns>Consulta filtrada</returns>
        public static IQueryable<T> WhereAnyContains<T>(this IQueryable<T> query, string searchTerm, params Expression<Func<T, string>>[] properties)
        {
            if (string.IsNullOrEmpty(searchTerm) || properties == null || properties.Length == 0)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var property in properties)
            {
                var propertyAccess = Expression.Invoke(property, parameter);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsCall = Expression.Call(propertyAccess, containsMethod, Expression.Constant(searchTerm));
                
                if (combinedExpression == null)
                {
                    combinedExpression = containsCall;
                }
                else
                {
                    combinedExpression = Expression.OrElse(combinedExpression, containsCall);
                }
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                return query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Convierte una consulta a SQL string para debugging
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <param name="query">Consulta a convertir</param>
        /// <returns>SQL string</returns>
        public static string ToSqlString<T>(this IQueryable<T> query)
        {
            return query.ToString();
        }
    }
}
