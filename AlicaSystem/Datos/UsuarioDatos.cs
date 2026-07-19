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
    }
}