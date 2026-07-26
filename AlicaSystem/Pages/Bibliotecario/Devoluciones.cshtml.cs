using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class DevolucionesModel : PageModel
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly UsuarioDatos usuarioDatos;

        public DevolucionesModel(PrestamoDatos prestamoDatos, UsuarioDatos usuarioDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.usuarioDatos = usuarioDatos;
        }

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

        // Busca al usuario por matrícula y devuelve sus préstamos activos
        public IActionResult OnGetBuscarLector(string matricula)
        {
            var usuario = usuarioDatos.BuscarPorMatricula(matricula);

            if (usuario == null)
                return new JsonResult(new { encontrado = false });

            var prestamos = prestamoDatos.ListarPrestamosActivosPorUsuario(usuario.Value.IdUsuario);

            return new JsonResult(new
            {
                encontrado = true,
                nombreCompleto = usuario.Value.Nombre + " " + usuario.Value.Apellido,
                prestamos = prestamos.Select(p => new
                {
                    idPrestamo = p.IdPrestamo,
                    titulo = p.Titulo,
                    codigoInterno = p.CodigoInterno,
                    fechaDevEsperada = p.FechaDevEsperada.ToString("dd/MM/yyyy"),
                    diasAtraso = (DateTime.Now.Date - p.FechaDevEsperada.Date).Days
                })
            });
        }

        // Registra la devolución del préstamo seleccionado
        public IActionResult OnPostRegistrar(int idPrestamo)
        {
            int? idEmpleado = HttpContext.Session.GetInt32("IdEmpleado");

            if (idEmpleado == null)
                return new JsonResult(new { exito = false, mensaje = "Sesión inválida. Vuelve a iniciar sesión." });

            bool exito = prestamoDatos.RegistrarDevolucion(idPrestamo, idEmpleado.Value);

            return new JsonResult(new
            {
                exito,
                mensaje = exito ? "Devolución registrada correctamente." : "No se pudo registrar la devolución (préstamo no encontrado o ya devuelto)."
            });
        }
    }
}