using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioDatos usuarioDatos;

        public LoginModel(UsuarioDatos usuarioDatos)
        {
            this.usuarioDatos = usuarioDatos;
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
            Usuario? usuario = usuarioDatos.ValidarUsuario(Email, Clave);

            if (usuario == null)
            {
                Mensaje = "Usuario no encontrado o credenciales incorrectas.";
                return Page();
            }

            // Guardamos en Session los datos que las demás páginas van a necesitar
            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("NombreUsuario", usuario.Nombre + " " + usuario.Apellido);
            HttpContext.Session.SetString("Rol", "Lector");

            // Redirige al menú principal del lector
            return RedirectToPage("/Lector/Dashboard");
        }
    }
}