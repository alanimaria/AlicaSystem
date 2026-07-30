using AlicaSystem.Datos;
using Microsoft.AspNetCore.Mvc;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class DevolucionesModel : PaginaBibliotecarioBase
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

        public void OnGet()
        {
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
            bool exito = prestamoDatos.RegistrarDevolucion(idPrestamo, IdEmpleadoSesion);

            return new JsonResult(new
            {
                exito,
                mensaje = exito ? "Devolución registrada correctamente." : "No se pudo registrar la devolución (préstamo no encontrado o ya devuelto)."
            });
        }

        // Registra la devolucion Y, si el libro llego danado, agrega la multa aparte
        public IActionResult OnPostRegistrarConEstado(int idPrestamo, bool libroDanado, decimal montoDano)
        {
            bool exito = prestamoDatos.RegistrarDevolucion(idPrestamo, IdEmpleadoSesion);
            if (!exito)
                return new JsonResult(new { exito = false, mensaje = "No se pudo registrar la devolución (préstamo no encontrado o ya devuelto)." });

            string mensaje = "Devolución registrada correctamente.";
            if (libroDanado && montoDano > 0)
            {
                var (exitoMulta, mensajeMulta) = multaDatos.RegistrarMultaPorEstadoLibro(idPrestamo, IdEmpleadoSesion, montoDano);
                mensaje += exitoMulta ? " Multa por mal estado registrada." : $" Devolución OK, pero la multa falló: {mensajeMulta}";
            }

            return new JsonResult(new { exito = true, mensaje });
        }
    }
}