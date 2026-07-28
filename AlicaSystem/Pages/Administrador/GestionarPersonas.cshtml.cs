using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarPersonasModel : PageModel
    {
        private readonly UsuarioDatos usuarioDatos;

        public GestionarPersonasModel(UsuarioDatos usuarioDatos)
        {
            this.usuarioDatos = usuarioDatos;
        }

        public List<Usuario> Usuarios { get; set; } = new();
        public string? Buscar { get; set; }

        [BindProperty]
        public int IdUsuario { get; set; }
        [BindProperty]
        public string Matricula { get; set; } = string.Empty;
        [BindProperty]
        public string Nombre { get; set; } = string.Empty;
        [BindProperty]
        public string Apellido { get; set; } = string.Empty;
        [BindProperty]
        public string? Telefono { get; set; }

        public void OnGet(int? id, string? buscar)
        {
            ViewData["Activo"] = "GestionarPersonas";
            Buscar = buscar;

            Usuarios = usuarioDatos.ListarUsuariosAdmin();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                Usuarios = Usuarios.Where(u =>
                    u.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    u.Apellido.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    u.Matricula.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (id != null)
            {
                var u = usuarioDatos.ListarUsuariosAdmin().FirstOrDefault(x => x.IdUsuario == id);
                if (u != null)
                {
                    IdUsuario = u.IdUsuario;
                    Matricula = u.Matricula;
                    Nombre = u.Nombre;
                    Apellido = u.Apellido;
                    Telefono = u.Telefono;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
            {
                TempData["Mensaje"] = "Nombre y apellido son obligatorios.";
                return RedirectToPage();
            }

            if (!Regex.IsMatch(Matricula, @"^\d{4}-\d{4}$"))
            {
                TempData["Mensaje"] = "La matrícula debe tener el formato 0000-0000.";
                return RedirectToPage();
            }

            if (!string.IsNullOrWhiteSpace(Telefono) && !Regex.IsMatch(Telefono, @"^\(\d{3}\) \d{3}-\d{4}$"))
            {
                TempData["Mensaje"] = "El teléfono debe tener el formato (123) 456-7890.";
                return RedirectToPage();
            }

            string email = Matricula + "@alica.edu.do";

            bool ok;
            string? error = null;

            if (IdUsuario == 0)
                ok = usuarioDatos.RegistrarUsuarioAdmin(Matricula, Nombre, Apellido, email, Telefono, out error);
            else
            {
                usuarioDatos.ActualizarUsuarioAdmin(IdUsuario, Matricula, Nombre, Apellido, email, Telefono);
                ok = true;
            }

            TempData["Mensaje"] = ok ? "Usuario guardado correctamente." : error;
            return RedirectToPage();
        }

        public IActionResult OnPostCambiarEstado(int id, bool nuevoEstado)
        {
            usuarioDatos.CambiarEstadoUsuario(id, nuevoEstado);
            TempData["Mensaje"] = nuevoEstado ? "Usuario activado." : "Usuario desactivado.";
            return RedirectToPage();
        }
    }
}