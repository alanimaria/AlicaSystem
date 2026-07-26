using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class UsuarioDatos
    {
        private readonly ConexionBD conexionBD;

        // Este constructor recibe un ConexionBD ya armado
        // (en vez de crearlo él mismo con "new")
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

        // Busca un lector por su matricula, solo entre usuarios activos.
        // Se usa en Registrar Prestamo, para que el bibliotecario
        // confirme visualmente que es la persona correcta antes de prestar.
        //
        // Devuelve una tupla en vez de un Usuario completo porque
        // PrestamosActivos y TieneMultaPendiente son datos calculados
        // (no columnas de la tabla usuario), y son especificos de esta
        // pantalla.
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
    }
}