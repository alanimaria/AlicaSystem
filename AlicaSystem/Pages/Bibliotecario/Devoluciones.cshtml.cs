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
        private readonly MultaDatos multaDatos;

        public DevolucionesModel(PrestamoDatos prestamoDatos, UsuarioDatos usuarioDatos, MultaDatos multaDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.usuarioDatos = usuarioDatos;
            this.multaDatos = multaDatos;
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
        // Registra la devolucion Y, si el libro llego danado, agrega la multa aparte
        public IActionResult OnPostRegistrarConEstado(int idPrestamo, bool libroDanado, decimal montoDano)
        {
            int? idEmpleado = HttpContext.Session.GetInt32("IdEmpleado");
            if (idEmpleado == null)
                return new JsonResult(new { exito = false, mensaje = "Sesión inválida. Vuelve a iniciar sesión." });

            bool exito = prestamoDatos.RegistrarDevolucion(idPrestamo, idEmpleado.Value);
            if (!exito)
                return new JsonResult(new { exito = false, mensaje = "No se pudo registrar la devolución (préstamo no encontrado o ya devuelto)." });

            string mensaje = "Devolución registrada correctamente.";
            if (libroDanado && montoDano > 0)
            {
                var (exitoMulta, mensajeMulta) = multaDatos.RegistrarMultaPorEstadoLibro(idPrestamo, idEmpleado.Value, montoDano);
                mensaje += exitoMulta ? " Multa por mal estado registrada." : $" Devolución OK, pero la multa falló: {mensajeMulta}";
            }

            return new JsonResult(new { exito = true, mensaje });
        }
    }
}