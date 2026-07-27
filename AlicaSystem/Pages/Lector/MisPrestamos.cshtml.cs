using Microsoft.AspNetCore.Mvc;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class MisPrestamosModel : PaginaLectorBase
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly MultaDatos multaDatos;

        public MisPrestamosModel(PrestamoDatos prestamoDatos, MultaDatos multaDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.multaDatos = multaDatos;
        }

        public List<Prestamo> Prestamos { get; set; } = new();
        public List<Multa> Multas { get; set; } = new();
        public string Tab { get; set; } = "activos";

        public void OnGet(string? tab)
        {
            ViewData["Activo"] = "MisPrestamos";
            Tab = tab ?? "activos";

            if (Tab == "historial")
            {
                Prestamos = prestamoDatos.ListarPrestamosPorUsuario(IdUsuarioSesion)
                    .Where(p => p.FechaDevReal != null)
                    .ToList();
            }
            else if (Tab == "multas")
            {
                Multas = multaDatos.ListarMultasPorUsuario(IdUsuarioSesion);
            }
            else
            {
                Prestamos = prestamoDatos.ListarPrestamosActivosPorUsuario(IdUsuarioSesion);
            }
        }

        public IActionResult OnPostRenovar(int idPrestamo)
        {
            var (exito, mensaje) = prestamoDatos.RenovarPrestamo(idPrestamo, IdUsuarioSesion);
            TempData["Mensaje"] = mensaje;
            return RedirectToPage();
        }
    }
}