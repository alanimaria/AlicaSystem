using AlicaSystem.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages
{
    public class RecuperarModel : PageModel
    {
        private readonly UsuarioDatos usuarioDatos;

        public RecuperarModel(UsuarioDatos usuarioDatos)
        {
            this.usuarioDatos = usuarioDatos;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string NuevaPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmarPassword { get; set; } = string.Empty;

        public string? Mensaje { get; set; }

        public void OnPost()
        {
            if (NuevaPassword != ConfirmarPassword)
            {
                Mensaje = "Las contraseñas no coinciden.";
                return;
            }

            bool actualizado = usuarioDatos.ResetearPasswordUsuario(Email, NuevaPassword);

            Mensaje = actualizado
                ? "Contraseña actualizada. Ya puedes iniciar sesión."
                : "No existe un usuario registrado con ese correo.";
        }
    }
}