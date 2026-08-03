using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class ReservasPendientesModel : PaginaBibliotecarioBase
    {
        private readonly ReservaDatos reservaDatos;

        public ReservasPendientesModel(ReservaDatos reservaDatos)
        {
            this.reservaDatos = reservaDatos;
        }

        public List<Reserva> Reservas { get; set; } = new();

        public void OnGet()
        {
            Reservas = reservaDatos.ListarReservasPendientes();
        }

        // Se llama desde los botones "Marcar entregado" y "Cancelar".
        // nuevoEstado llega como "COMPLETADA" o "CANCELADA" desde el JS
        // (deben coincidir exacto con los nombres del catalogo ESTADO_RESERVA).
        public IActionResult OnPostActualizarEstado(int idReserva, string nuevoEstado)
        {
            var (exito, mensaje) = reservaDatos.ActualizarEstadoReserva(idReserva, nuevoEstado, IdEmpleadoSesion);
            return new JsonResult(new { exito, mensaje });
        }
    }
}