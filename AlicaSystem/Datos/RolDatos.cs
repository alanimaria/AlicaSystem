using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    // Se encarga de hablar con la base de datos para todo lo relacionado
    // con roles del personal (Bibliotecario, Administrador, etc).
    public class RolDatos
    {
        private readonly ConexionBD conexionBD;

        public RolDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        // Lista todos los roles con la cantidad de empleados que tiene cada uno.
        public List<Rol> ListarRoles()
        {
            var lista = new List<Rol>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarRoles", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Rol
                {
                    IdRol = Convert.ToInt32(dr["id_rol"]),
                    Nombre = dr["nombre"].ToString()!,
                    Descripcion = dr["descripcion"] == DBNull.Value ? null : dr["descripcion"].ToString(),
                    CantidadEmpleados = Convert.ToInt32(dr["cantidad_empleados"])
                });
            }
            return lista;
        }

        // CREATE: registra un rol nuevo.
        public bool InsertarRol(string nombre, string? descripcion, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_InsertarRol", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)descripcion ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        // UPDATE: actualiza nombre y descripción de un rol existente.
        public bool ActualizarRol(int idRol, string nombre, string? descripcion, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_ActualizarRol", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)descripcion ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        // DELETE físico real. El SP bloquea el borrado (RAISERROR) si hay
        // empleados asignados a ese rol, así que ese mensaje se captura aquí.
        public bool EliminarRol(int idRol, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_EliminarRol", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRol", idRol);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message.Contains("empleados asignados")
                    ? "No se puede eliminar el rol: hay empleados asignados a él."
                    : ex.Message;
                return false;
            }
        }
    }
}