using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioDatos usuarioDatos;
        private readonly EmpleadoDatos empleadoDatos; // nuevo

        public LoginModel(UsuarioDatos usuarioDatos, EmpleadoDatos empleadoDatos) // nuevo parámetro
        {
            this.usuarioDatos = usuarioDatos;
            this.empleadoDatos = empleadoDatos;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Clave { get; set; } = string.Empty;

        public string? Mensaje { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // 1) Primero probamos como lector (igual que antes, intacto)
            Usuario? usuario = usuarioDatos.ValidarUsuario(Email, Clave);

            if (usuario != null)
            {
                HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
                HttpContext.Session.SetString("NombreUsuario", usuario.Nombre + " " + usuario.Apellido);
                HttpContext.Session.SetString("Rol", "Lector");

                return RedirectToPage("/Lector/Dashboard");
            }

            // 2) Si no es lector, probamos como empleado (bibliotecario o administrador)
            Empleado? empleado = empleadoDatos.ValidarEmpleado(Email, Clave);

            if (empleado != null)
            {
                HttpContext.Session.SetInt32("IdEmpleado", empleado.IdEmpleado);
                HttpContext.Session.SetString("NombreUsuario", empleado.Nombre + " " + empleado.Apellido);
                HttpContext.Session.SetString("Rol", empleado.NombreRol); // "Bibliotecario" o "Administrador"

                if (empleado.NombreRol == "Administrador")
                    return RedirectToPage("/Administrador/Dashboard");

                return RedirectToPage("/Bibliotecario/Dashboard");
            }

            // 3) Ninguno de los dos coincidió
            Mensaje = "Usuario no encontrado o credenciales incorrectas.";
            return Page();
        }
    }
}