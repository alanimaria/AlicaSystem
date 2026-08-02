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

        // ==========================================================================
        // A partir de aqui: metodos nuevos para la pantalla "Gestionar Empleados"
        // del Administrador (rama camila/administrador-empleados).
        // ==========================================================================

        // Lista todos los empleados (activos e inactivos) con el nombre de su rol,
        // para la tabla principal de la pantalla.
        public List<Empleado> ListarEmpleadosAdmin()
        {
            var lista = new List<Empleado>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarEmpleados", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Empleado
                {
                    IdEmpleado = Convert.ToInt32(dr["id_empleado"]),
                    Nombre = dr["nombre"].ToString()!,
                    Apellido = dr["apellido"].ToString()!,
                    Email = dr["email"].ToString()!,
                    Telefono = dr["telefono"] == DBNull.Value ? null : dr["telefono"].ToString(),
                    IdRol = Convert.ToInt32(dr["id_rol"]),
                    NombreRol = dr["rol"].ToString()!,
                    FechaIngreso = Convert.ToDateTime(dr["fecha_ingreso"]),
                    Area = dr["area"].ToString()!,
                    Estado = Convert.ToBoolean(dr["estado"])
                });
            }
            return lista;
        }

        // CREATE: registra un nuevo empleado. Devuelve false + mensaje de error si
        // el correo ya esta en uso (validado tambien en el stored procedure).
        public bool RegistrarEmpleado(string nombre, string apellido, string email, string password,
            string? telefono, int idRol, string area, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_RegistrarEmpleado", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Apellido", apellido);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.Parameters.AddWithValue("@Area", area);
                cmd.ExecuteScalar(); // el SP devuelve el IdEmpleado nuevo, no lo necesitamos aqui
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message.Contains("correo")
                    ? "Ya existe un empleado registrado con ese correo."
                    : ex.Message;
                return false;
            }
        }

        // UPDATE: actualiza los datos del empleado (sin tocar la contraseña).
        public bool ActualizarEmpleado(int idEmpleado, string nombre, string apellido, string email,
            string? telefono, int idRol, string area, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_ActualizarEmpleado", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Apellido", apellido);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.Parameters.AddWithValue("@Area", area);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message.Contains("correo")
                    ? "Ya existe otro empleado registrado con ese correo."
                    : ex.Message;
                return false;
            }
        }

        // Activa / desactiva un empleado (soft delete, no se borra el registro).
        public void CambiarEstadoEmpleado(int idEmpleado, bool estado)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_CambiarEstadoEmpleado", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.ExecuteNonQuery();
        }
    }
}