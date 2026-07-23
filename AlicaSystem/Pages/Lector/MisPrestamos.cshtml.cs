using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class MisPrestamosModel : PageModel
    {
        private readonly PrestamoDatos prestamoDatos;

        public MisPrestamosModel(PrestamoDatos prestamoDatos)
        {
            this.prestamoDatos = prestamoDatos;
        }

        public List<Prestamo> Prestamos { get; set; } = new();

        public void OnGet()
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            Prestamos = prestamoDatos.ListarPrestamosActivosPorUsuario(idUsuario);
        }

        public IActionResult OnPostRenovar(int idPrestamo)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            var (exito, mensaje) = prestamoDatos.RenovarPrestamo(idPrestamo, idUsuario);
            TempData["Mensaje"] = mensaje;
            return RedirectToPage();
        }
    }
}