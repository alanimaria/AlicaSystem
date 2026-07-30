using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class DashboardModel : PaginaBibliotecarioBase
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly ReservaDatos reservaDatos;
        private readonly MultaDatos multaDatos;

        public DashboardModel(PrestamoDatos prestamoDatos, ReservaDatos reservaDatos, MultaDatos multaDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.reservaDatos = reservaDatos;
            this.multaDatos = multaDatos;
        }

        public int PrestamosActivos { get; set; }
        public int ReservasPendientes { get; set; }
        public int MultasPendientes { get; set; }
        public List<ActividadReciente> ActividadReciente { get; set; } = new();
        public List<PrestamoActivoResumen> PrestamosActivosDetalle { get; set; } = new();

        public void OnGet()
        {
            PrestamosActivos = prestamoDatos.ContarPrestamosActivos();
            ReservasPendientes = reservaDatos.ContarReservasPendientes();
            MultasPendientes = multaDatos.ContarMultasPendientes();
            ActividadReciente = prestamoDatos.ListarActividadReciente(10);
            PrestamosActivosDetalle = prestamoDatos.ListarPrestamosActivosGlobal();
        }
    }
}