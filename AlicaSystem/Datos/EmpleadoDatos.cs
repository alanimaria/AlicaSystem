using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    // Se encarga de hablar con la base de datos para todo lo relacionado
    // con empleados (bibliotecarios y administradores).
    // Sigue el mismo patrón que UsuarioDatos, pero para la tabla "empleado".
    public class EmpleadoDatos
    {
        private readonly ConexionBD conexionBD;

        // Recibimos ConexionBD ya armado (con la cadena de conexión de appsettings.json).
        // Program.cs se encarga de "inyectarlo" automáticamente aquí.
        public EmpleadoDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        // Valida el email y password contra la tabla "empleado".
        // Si las credenciales son correctas, devuelve el Empleado con su rol.
        // Si no, devuelve null (así el Login sabe que debe mostrar el mensaje de error).
        public Empleado? ValidarEmpleado(string email, string password)
        {
            using var conexion = conexionBD.ObtenerConexion();

            // Llamamos al stored procedure sp_LoginEmpleado, que hace JOIN
            // entre "empleado" y "rol" y nos devuelve el nombre del rol ya listo.
            using var comando = new SqlCommand("sp_LoginEmpleado", conexion);
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Email", email);
            comando.Parameters.AddWithValue("@Password", password);

            conexion.Open();
            using var reader = comando.ExecuteReader();

            // Si el SP devolvió una fila, las credenciales son correctas.
            if (reader.Read())
            {
                return new Empleado
                {
                    IdEmpleado = reader.GetInt32(reader.GetOrdinal("id_empleado")),
                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                    Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Area = reader.GetString(reader.GetOrdinal("area")),
                    NombreRol = reader.GetString(reader.GetOrdinal("nombre_rol"))
                };
            }

            // No hubo coincidencia: email/password incorrectos, o el empleado está inactivo.
            return null;
        }
    }
}