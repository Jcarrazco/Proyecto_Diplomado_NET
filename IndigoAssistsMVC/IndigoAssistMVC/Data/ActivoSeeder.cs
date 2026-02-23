using Microsoft.EntityFrameworkCore;
using IndigoAssistMVC.Models;

namespace IndigoAssistMVC.Data
{
    public static class ActivoSeeder
    {
        public static async Task SeedAsync(IndigoDBContext context)
        {
            // Verificar si ya existen datos
            if (await context.TiposActivo.AnyAsync())
            {
                return; // Ya hay datos, no hacer nada
            }

            // Crear tipos de activo
            var tiposActivo = new[]
            {
                new TipoActivo { TipoActivoNombre = "Laptop" },
                new TipoActivo { TipoActivoNombre = "PC de Escritorio" },
                new TipoActivo { TipoActivoNombre = "Servidor" },
                new TipoActivo { TipoActivoNombre = "Impresora" },
                new TipoActivo { TipoActivoNombre = "Router" },
                new TipoActivo { TipoActivoNombre = "Monitor" },
                new TipoActivo { TipoActivoNombre = "Tablet" },
                new TipoActivo { TipoActivoNombre = "Smartphone" },
                new TipoActivo { TipoActivoNombre = "Switch de Red" },
                new TipoActivo { TipoActivoNombre = "Firewall" },
                new TipoActivo { TipoActivoNombre = "UPS" },
                new TipoActivo { TipoActivoNombre = "Escaner" }
            };

            context.TiposActivo.AddRange(tiposActivo);

            // Crear status
            var status = new[]
            {
                new Status { StatusNombre = "Activo" },
                new Status { StatusNombre = "Mantenimiento" },
                new Status { StatusNombre = "Baja" },
                new Status { StatusNombre = "Garantia" }
            };

            context.Status.AddRange(status);

            // Crear proveedores
            var proveedores = new[]
            {
                new Proveedor { ProveedorNombre = "Dell Technologies" },
                new Proveedor { ProveedorNombre = "HP Inc." },
                new Proveedor { ProveedorNombre = "Lenovo Group" },
                new Proveedor { ProveedorNombre = "Apple Inc." },
                new Proveedor { ProveedorNombre = "Microsoft Corporation" },
                new Proveedor { ProveedorNombre = "ASUS Computer" },
                new Proveedor { ProveedorNombre = "Acer Inc." },
                new Proveedor { ProveedorNombre = "Samsung Electronics" },
                new Proveedor { ProveedorNombre = "Canon Inc." },
                new Proveedor { ProveedorNombre = "Epson Corporation" },
                new Proveedor { ProveedorNombre = "Cisco Systems" },
                new Proveedor { ProveedorNombre = "Ubiquiti Networks" },
                new Proveedor { ProveedorNombre = "APC by Schneider Electric" },
                new Proveedor { ProveedorNombre = "Brother Industries" },
                new Proveedor { ProveedorNombre = "Xerox Corporation" }
            };

            context.Proveedores.AddRange(proveedores);

            // Crear componentes
            var componentes = new[]
            {
                new Componente { ComponenteNombre = "Procesador", ValorBit = 1 },
                new Componente { ComponenteNombre = "Memoria RAM", ValorBit = 2 },
                new Componente { ComponenteNombre = "Disco Duro SSD", ValorBit = 4 },
                new Componente { ComponenteNombre = "Tarjeta Grafica", ValorBit = 8 },
                new Componente { ComponenteNombre = "Fuente de Poder", ValorBit = 16 },
                new Componente { ComponenteNombre = "Tarjeta de Red", ValorBit = 32 },
                new Componente { ComponenteNombre = "Ventiladores", ValorBit = 64 },
                new Componente { ComponenteNombre = "Pantalla", ValorBit = 128 },
                new Componente { ComponenteNombre = "Teclado", ValorBit = 256 },
                new Componente { ComponenteNombre = "Mouse", ValorBit = 512 },
                new Componente { ComponenteNombre = "Bocinas", ValorBit = 1024 },
                new Componente { ComponenteNombre = "Camara Web", ValorBit = 2048 },
                new Componente { ComponenteNombre = "Microfono", ValorBit = 4096 },
                new Componente { ComponenteNombre = "Disco Duro HDD", ValorBit = 8192 }
            };

            context.Componentes.AddRange(componentes);

            // Crear software
            var software = new[]
            {
                new Software { Nombre = "Windows 11 Pro" },
                new Software { Nombre = "Windows 10 Pro" },
                new Software { Nombre = "Windows Server 2022" },
                new Software { Nombre = "macOS Sonoma" },
                new Software { Nombre = "macOS Ventura" },
                new Software { Nombre = "Ubuntu 22.04 LTS" },
                new Software { Nombre = "Ubuntu 20.04 LTS" },
                new Software { Nombre = "Red Hat Enterprise Linux" },
                new Software { Nombre = "CentOS Stream" },
                new Software { Nombre = "Microsoft Office 365" },
                new Software { Nombre = "Microsoft Office 2021" },
                new Software { Nombre = "Adobe Creative Cloud" },
                new Software { Nombre = "Adobe Acrobat Pro" },
                new Software { Nombre = "Google Workspace" },
                new Software { Nombre = "Zoom Professional" },
                new Software { Nombre = "Microsoft Teams" },
                new Software { Nombre = "Slack" },
                new Software { Nombre = "Visual Studio Professional" },
                new Software { Nombre = "IntelliJ IDEA" },
                new Software { Nombre = "Photoshop CC" }
            };

            context.Software.AddRange(software);

            // Crear departamentos (necesarios para usuarios de Identity)
            var departamentos = new[]
            {
                new mDepartamentos { Departamento = "Ventas", Tickets = true },
                new mDepartamentos { Departamento = "Soporte Tecnico", Tickets = true },
                new mDepartamentos { Departamento = "Recursos Humanos", Tickets = true },
                new mDepartamentos { Departamento = "Facturacion", Tickets = true },
                new mDepartamentos { Departamento = "Marketing", Tickets = true },
                new mDepartamentos { Departamento = "Direccion General", Tickets = true },
                new mDepartamentos { Departamento = "Contabilidad", Tickets = true },
                new mDepartamentos { Departamento = "Sistemas", Tickets = true },
                new mDepartamentos { Departamento = "Almacen", Tickets = true }
            };

            context.mDepartamentos.AddRange(departamentos);

            await context.SaveChangesAsync();
            Console.WriteLine("Datos de catálogos de activos y departamentos creados exitosamente.");
        }
    }
}
