using Microsoft.Data.SqlClient;

namespace AlicaSystem.Datos
{
    public class ConexionBD
    {
        // En vez de tener la cadena escrita a mano aquí,
        // la recibimos desde afuera (appsettings.json)
        private readonly string cadena;

        // El constructor recibe la configuración de la app
        // y saca de ahí la cadena de conexión llamada "AlicaSystem"
        public ConexionBD(IConfiguration configuracion)
        {
            cadena = configuracion.GetConnectionString("AlicaSystem")!;
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}