using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssistMVC.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IndigoDBContext context, UserManager<Usuario> userManager, RoleManager<Rol> roleManager)
        {
            // Crear roles si no existen
            string[] roles = { "Administrador", "Supervisor", "Tecnico" };
            
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Crear rol solo con las propiedades básicas de Identity
                    // Las propiedades Descripcion y Activo no existen en la tabla AspNetRoles
                    await roleManager.CreateAsync(new Rol
                    {
                        Name = roleName
                    });
                }
            }

            // Obtener el primer departamento disponible (Sistemas)
            // Si mDepartamentos está ignorado temporalmente, usar null o un valor por defecto
            byte? idDepartamento = null;
            try
            {
                var departamentoSistemas = await context.mDepartamentos.FirstOrDefaultAsync(d => d.Departamento == "Sistemas");
                idDepartamento = departamentoSistemas?.IdDepto ?? 18; // Usar ID 18 como fallback
            }
            catch
            {
                // Si mDepartamentos no está disponible (ignorado), usar null
                idDepartamento = null;
            }

            // Crear usuarios de prueba si no existen
            await CreateUserIfNotExists(userManager, "admin@indigo.com", "Password123!", "Administrador", "Administrador", "Sistema", idDepartamento);
            await CreateUserIfNotExists(userManager, "supervisor@indigo.com", "Password123!", "Supervisor", "Juan", "Pérez", idDepartamento);
            await CreateUserIfNotExists(userManager, "tecnico@indigo.com", "Password123!", "Tecnico", "María", "García", idDepartamento);
            await CreateUserIfNotExists(userManager, "usuario@indigo.com", "Password123!", "Tecnico", "Carlos", "López", idDepartamento);
        }

        private static async Task CreateUserIfNotExists(UserManager<Usuario> userManager, string email, string password, string role, string nombre, string apellido, byte? idDepartamento)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Usuario
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    NombreCompleto = $"{nombre} {apellido}",
                    IdDepartamento = idDepartamento,
                    Activo = true,
                    FechaRegistro = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    Console.WriteLine($"Usuario creado: {email} con rol {role}");
                }
                else
                {
                    Console.WriteLine($"Error creando usuario {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
