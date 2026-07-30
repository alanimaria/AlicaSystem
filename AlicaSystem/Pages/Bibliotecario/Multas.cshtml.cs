using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class GestionarMultasModel : PaginaBibliotecarioBase
    {
        private readonly MultaDatos multaDatos;

        public GestionarMultasModel(MultaDatos multaDatos)
        {
            this.multaDatos = multaDatos;
        }

        public List<Multa> Multas { get; set; } = new();

        public void OnGet()
        {
            Multas = multaDatos.ListarMultas();
        }

        // Marca una multa como Pagada o Perdonada.
        // RN-MUL-04: solo Bibliotecario o Administrador pueden perdonar.
        // La validación de sesión/rol ya la hace PaginaBibliotecarioBase
        // antes de que este handler se ejecute.
        public IActionResult OnPostActualizarEstado(int idMulta, string estadoDestino)
        {
            if (estadoDestino != "Pagada" && estadoDestino != "Perdonada")
            {
                return new JsonResult(new { exito = false, mensaje = "Estado destino invalido." });
            }

            bool exito = multaDatos.ActualizarEstadoMulta(idMulta, estadoDestino);
            return new JsonResult(new { exito });
        }
    }
}