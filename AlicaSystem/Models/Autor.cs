// Models/Autor.cs
namespace AlicaSystem.Models
{
    public class Autor
    {
        public int IdAutor { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string PaisOrigen { get; set; } = string.Empty;
    }
}