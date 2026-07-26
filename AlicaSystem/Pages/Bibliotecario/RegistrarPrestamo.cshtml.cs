using AlicaSystem.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Bibliotecario
{
    // Cerebro de la pantalla "Registrar prestamo".
    public class RegistrarPrestamoModel : PageModel
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly UsuarioDatos usuarioDatos;

        public RegistrarPrestamoModel(PrestamoDatos prestamoDatos, UsuarioDatos usuarioDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.usuarioDatos = usuarioDatos;
        }

        // Se muestra en la tarjeta "Bibliotecario responsable"
        public string NombreUsuario { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            string? rol = HttpContext.Session.GetString("Rol");

            if (rol != "Bibliotecario" && rol != "Administrador")
            {
                return RedirectToPage("/Login");
            }

            NombreUsuario = HttpContext.Session.GetString("NombreUsuario") ?? "";

            return Page();
        }

        // Se llama cuando el bibliotecario aprieta "Buscar" junto al libro.
        public IActionResult OnGetBuscarLibro(string codigo)
        {
            var libro = prestamoDatos.BuscarLibroPorCodigo(codigo);

            if (libro == null)
                return new JsonResult(new { encontrado = false });

            return new JsonResult(new
            {
                encontrado = true,
                idLibro = libro.Value.IdLibro,
                titulo = libro.Value.Titulo,
                cantidadDisponible = libro.Value.CantidadDisponible
            });
        }

        // Se llama cuando el bibliotecario aprieta "Buscar" junto al lector.
        // Ahora tambien devuelve cuantos prestamos activos tiene el usuario
        // y si tiene alguna multa pendiente.
        public IActionResult OnGetBuscarLector(string matricula)
        {
            var usuario = usuarioDatos.BuscarPorMatricula(matricula);

            if (usuario == null)
                return new JsonResult(new { encontrado = false });

            return new JsonResult(new
            {
                encontrado = true,
                idUsuario = usuario.Value.IdUsuario,
                nombreCompleto = usuario.Value.Nombre + " " + usuario.Value.Apellido,
                prestamosActivos = usuario.Value.PrestamosActivos,
                tieneMultaPendiente = usuario.Value.TieneMultaPendiente
            });
        }

        // Se llama cuando el bibliotecario aprieta "Registrar prestamo".
        // El SP ya valida multas pendientes, maximo de prestamos activos
        // y copias disponibles, y devuelve el mensaje exacto del motivo
        // si algo no se pudo hacer.
        public IActionResult OnPostRegistrar(int idUsuario, int idLibro)
        {
            int? idEmpleado = HttpContext.Session.GetInt32("IdEmpleado");

            if (idEmpleado == null)
                return new JsonResult(new { exito = false, mensaje = "Sesion invalida. Vuelve a iniciar sesion." });

            var (idPrestamo, mensaje) = prestamoDatos.RegistrarPrestamo(idUsuario, idLibro, idEmpleado.Value);

            return new JsonResult(new { exito = idPrestamo > 0, idPrestamo, mensaje });
        }
    }
}