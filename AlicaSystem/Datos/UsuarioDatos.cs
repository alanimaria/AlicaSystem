using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class UsuarioDatos
    {
        private readonly ConexionBD conexionBD;

        public UsuarioDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public bool ResetearPasswordUsuario(string email, string nuevaPassword)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ResetearPasswordUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@NuevaPassword", nuevaPassword);
            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());
            return filasAfectadas > 0;
        }

        public Usuario? ValidarUsuario(string email, string clave)
        {
            Usuario? usuario = null;

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_LoginUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", clave);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                usuario = new Usuario();
                usuario.IdUsuario = Convert.ToInt32(dr["id_usuario"]);
                usuario.Matricula = dr["matricula"].ToString()!;
                usuario.Nombre = dr["nombre"].ToString()!;
                usuario.Apellido = dr["apellido"].ToString()!;
                usuario.Email = dr["email"].ToString()!;
                usuario.Telefono = dr["telefono"] == DBNull.Value ? null : dr["telefono"].ToString();
                usuario.FechaRegistro = Convert.ToDateTime(dr["fecha_registro"]);
                usuario.Estado = Convert.ToBoolean(dr["estado"]);
            }

            return usuario;
        }
        public List<Usuario> ListarUsuariosAdmin()
        {
            var lista = new List<Usuario>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarUsuariosAdmin", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Usuario
                {
                    IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                    Matricula = dr["matricula"].ToString()!,
                    Nombre = dr["nombre"].ToString()!,
                    Apellido = dr["apellido"].ToString()!,
                    Email = dr["email"].ToString()!,
                    Telefono = dr["telefono"] == DBNull.Value ? null : dr["telefono"].ToString(),
                    FechaRegistro = Convert.ToDateTime(dr["fecha_registro"]),
                    Password = dr["password"].ToString()!,
                    Estado = Convert.ToBoolean(dr["estado"])
                });
            }
            return lista;
        }

        public void ActualizarUsuarioAdmin(int idUsuario, string matricula, string nombre, string apellido, string email, string? telefono)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ActualizarUsuarioAdmin", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@Matricula", matricula);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Apellido", apellido);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void CambiarEstadoUsuario(int idUsuario, bool estado)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_CambiarEstadoUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.ExecuteNonQuery();
        }
        public bool RegistrarUsuarioAdmin(string matricula, string nombre, string apellido, string email, string? telefono, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_RegistrarUsuarioAdmin", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Matricula", matricula);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Apellido", apellido);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Telefono", (object?)telefono ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message.Contains("UNIQUE")
                    ? "Ya existe un usuario con ese email o matrícula."
                    : ex.Message;
                return false;
            }
        }

        public (int IdUsuario, string Nombre, string Apellido, string Matricula, int PrestamosActivos, bool TieneMultaPendiente)? BuscarPorMatricula(string matricula)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_BuscarUsuarioPorMatricula", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Matricula", matricula);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return (
                    Convert.ToInt32(dr["id_usuario"]),
                    dr["nombre"].ToString()!,
                    dr["apellido"].ToString()!,
                    dr["matricula"].ToString()!,
                    Convert.ToInt32(dr["PrestamosActivos"]),
                    Convert.ToInt32(dr["TieneMultaPendiente"]) == 1
                );
            }

            return null;
        }

        // Cuenta cuantos lectores tienen estado = activo.
        // Se usa para el KPI "Usuarios activos" del Dashboard Administrador.
        public int ContarUsuariosActivos()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarUsuariosActivos", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}