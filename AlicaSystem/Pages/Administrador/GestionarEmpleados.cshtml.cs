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

        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        public string? Telefono { get; set; }

        [BindProperty]
        public int IdRol { get; set; }

        [BindProperty]
        public string Area { get; set; } = string.Empty;

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
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) || string.IsNullOrWhiteSpace(Email) || IdRol == 0 || string.IsNullOrWhiteSpace(Area))
            {
                TempData["Mensaje"] = "Nombre, apellido, correo, rol y área son obligatorios.";
                return RedirectToPage();
            }

            bool exito;
            string? error;

            if (IdEmpleado == 0)
            {
                if (string.IsNullOrWhiteSpace(Password))
                {
                    TempData["Mensaje"] = "La contraseña temporal es obligatoria para registrar un empleado nuevo.";
                    TempData["MensajeTipo"] = "error";
                    return RedirectToPage();
                }

                exito = empleadoDatos.RegistrarEmpleado(Nombre, Apellido, Email, Password, Telefono, IdRol, Area, out error);
            }
            else
            {
                exito = empleadoDatos.ActualizarEmpleado(IdEmpleado, Nombre, Apellido, Email, Telefono, IdRol, Area, out error);
            }

            TempData["Mensaje"] = exito ? "Empleado guardado correctamente." : error;
            TempData["MensajeTipo"] = exito ? "ok" : "error";
            return RedirectToPage();
        }

        public IActionResult OnPostCambiarEstado(int id, bool estado)
        {
            empleadoDatos.CambiarEstadoEmpleado(id, estado);
            TempData["Mensaje"] = estado ? "Empleado reactivado." : "Empleado desactivado.";
            TempData["MensajeTipo"] = "ok";
            return RedirectToPage();
        }
    }
}