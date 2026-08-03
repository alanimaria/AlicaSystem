using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarRolesModel : PageModel
    {
        private readonly RolDatos rolDatos;

        public GestionarRolesModel(RolDatos rolDatos)
        {
            this.rolDatos = rolDatos;
        }

        public List<Rol> Roles { get; set; } = new();

        [BindProperty]
        public int IdRol { get; set; }

        [BindProperty]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty]
        public string? Descripcion { get; set; }

        public void OnGet(int? id)
        {
            ViewData["Activo"] = "GestionarRoles";
            Roles = rolDatos.ListarRoles();

            if (id != null)
            {
                var r = Roles.FirstOrDefault(x => x.IdRol == id);
                if (r != null)
                {
                    IdRol = r.IdRol;
                    Nombre = r.Nombre;
                    Descripcion = r.Descripcion;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                TempData["Mensaje"] = "El nombre del rol es obligatorio.";
                return RedirectToPage();
            }

            bool exito;
            string? error;

            if (IdRol == 0)
                exito = rolDatos.InsertarRol(Nombre, Descripcion, out error);
            else
                exito = rolDatos.ActualizarRol(IdRol, Nombre, Descripcion, out error);

            TempData["Mensaje"] = exito ? "Rol guardado correctamente." : error;
            return RedirectToPage();
        }
    }
}