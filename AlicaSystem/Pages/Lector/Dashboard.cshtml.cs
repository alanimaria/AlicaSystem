using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlicaSystem.Pages.Lector
{
    public class DashboardModel : PaginaLectorBase
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly ReservaDatos reservaDatos;
        private readonly MultaDatos multaDatos;
        private readonly ListaDatos listaDatos;
        private readonly LibroDatos libroDatos;

        public DashboardModel(PrestamoDatos prestamoDatos, ReservaDatos reservaDatos, MultaDatos multaDatos, ListaDatos listaDatos, LibroDatos libroDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.reservaDatos = reservaDatos;
            this.multaDatos = multaDatos;
            this.listaDatos = listaDatos;
            this.libroDatos = libroDatos;
        }

        public int PrestamosActivos { get; set; }
        public int ReservasPendientes { get; set; }
        public int MultasPendientes { get; set; }
        public int ListasCreadas { get; set; }
        public List<Prestamo> PrestamosRecientes { get; set; } = new();
        public List<Libro> LibrosSugeridos { get; set; } = new();

        public void OnGet()
        {
            PrestamosActivos = prestamoDatos.ContarPrestamosActivosPorUsuario(IdUsuarioSesion);
            ReservasPendientes = reservaDatos.ContarReservasPendientesPorUsuario(IdUsuarioSesion);
            MultasPendientes = multaDatos.ContarMultasPendientesPorUsuario(IdUsuarioSesion);
            ListasCreadas = listaDatos.ContarListasActivas(IdUsuarioSesion);
            PrestamosRecientes = prestamoDatos.ListarPrestamosActivosPorUsuario(IdUsuarioSesion).Take(2).ToList();
            LibrosSugeridos = libroDatos.ListarCatalogo(null, null, true).Take(2).ToList();
        }
    }
}