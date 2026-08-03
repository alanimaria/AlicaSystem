using AlicaSystem.Datos;
using Microsoft.AspNetCore.Mvc;

namespace AlicaSystem.Pages.Bibliotecario
{
    // Cerebro de la pantalla "Registrar prestamo".
    public class RegistrarPrestamoModel : PaginaBibliotecarioBase
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly UsuarioDatos usuarioDatos;

        public RegistrarPrestamoModel(PrestamoDatos prestamoDatos, UsuarioDatos usuarioDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.usuarioDatos = usuarioDatos;
        }

        public string? CodigoPrecargado { get; set; }

        public void OnGet(string? codigo)
        {
            CodigoPrecargado = codigo;
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
        public IActionResult OnPostRegistrar(int idUsuario, int idLibro, int diasPlazo, string tituloLibro)
        {
            if (diasPlazo < 1 || diasPlazo > 7)
            {
                return new JsonResult(new { exito = false, mensaje = "Los días de plazo deben estar entre 1 y 7." });
            }

            var (idPrestamo, mensaje) = prestamoDatos.RegistrarPrestamo(idUsuario, idLibro, IdEmpleadoSesion, diasPlazo);
            var fechaVence = DateTime.Now.AddDays(diasPlazo).ToString("dd/MM/yyyy");
            return new JsonResult(new { exito = idPrestamo > 0, idPrestamo, mensaje, tituloLibro, fechaVence });
        }
    }
}