using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarEmpleadosModel : PageModel
    {
        private readonly EmpleadoDatos empleadoDatos;

        public GestionarEmpleadosModel(EmpleadoDatos empleadoDatos)
        {
            this.empleadoDatos = empleadoDatos;
        }

        public List<Empleado> Empleados { get; set; } = new();

        [BindProperty]
        public int IdEmpleado { get; set; }

        [BindProperty]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty]
        public string Apellido { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        // Solo se usa al registrar un empleado nuevo (IdEmpleado == 0).
        // Al editar, este campo llega vacío y no se toca la contraseña existente.
        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        public string? Telefono { get; set; }

        [BindProperty]
        public int IdRol { get; set; }

        [BindProperty]
        public string Area { get; set; } = string.Empty;

        // Solo para mostrar en modo "editar" (readonly). El SP no la recibe,
        // así que nunca se manda de vuelta al servidor para guardar.
        public DateTime? FechaIngreso { get; set; }

        public void OnGet(int? id)
        {
            ViewData["Activo"] = "GestionarEmpleados";
            Empleados = empleadoDatos.ListarEmpleadosAdmin();

            if (id != null)
            {
                var e = Empleados.FirstOrDefault(x => x.IdEmpleado == id);
                if (e != null)
                {
                    IdEmpleado = e.IdEmpleado;
                    Nombre = e.Nombre;
                    Apellido = e.Apellido;
                    Email = e.Email;
                    Telefono = e.Telefono;
                    IdRol = e.IdRol;
                    Area = e.Area;
                    FechaIngreso = e.FechaIngreso;
                }
            }
        }

        public IActionResult OnPost()
        {
            bool exito;
            string? error;

            if (IdEmpleado == 0)
            {
                if (string.IsNullOrWhiteSpace(Password))
                {
                    TempData["Mensaje"] = "La contraseña temporal es obligatoria para registrar un empleado nuevo.";
                    return RedirectToPage();
                }

                exito = empleadoDatos.RegistrarEmpleado(Nombre, Apellido, Email, Password, Telefono, IdRol, Area, out error);
            }
            else
            {
                exito = empleadoDatos.ActualizarEmpleado(IdEmpleado, Nombre, Apellido, Email, Telefono, IdRol, Area, out error);
            }

            TempData["Mensaje"] = exito ? "Empleado guardado correctamente." : error;
            return RedirectToPage();
        }

        public IActionResult OnPostCambiarEstado(int id, bool estado)
        {
            empleadoDatos.CambiarEstadoEmpleado(id, estado);
            TempData["Mensaje"] = estado ? "Empleado reactivado." : "Empleado desactivado.";
            return RedirectToPage();
        }
    }
}