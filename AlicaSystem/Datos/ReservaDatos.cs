using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class ReservaDatos
    {
        private readonly ConexionBD conexionBD;

        public ReservaDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public int ContarReservasPendientes()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarReservasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Reserva> ListarReservasPorUsuario(int idUsuario)
        {
            var lista = new List<Reserva>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarReservasPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Reserva
                {
                    IdReserva = Convert.ToInt32(dr["id_reserva"]),
                    Titulo = dr["titulo"].ToString()!,
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    FechaReserva = Convert.ToDateTime(dr["fecha_reserva"]),
                    FechaExpiracion = Convert.ToDateTime(dr["fecha_expiracion"]),
                    Estado = dr["estado"].ToString()!
                });
            }
            return lista;
        }

        public void CancelarReservaUsuario(int idReserva, int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_CancelarReservaUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.ExecuteNonQuery();
        }
    }
}