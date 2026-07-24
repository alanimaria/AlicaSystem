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
        // Cuenta cuántas reservas en estado "Pendiente" tiene un usuario
        // específico (esperando ser recogidas). Se usa para el KPI
        // "Reservas pendientes" del Dashboard Lector.
        //
        // NOTA: no confundir con ContarReservasPendientes() (sin usuario),
        // que ya existe en esta misma clase para el dashboard GLOBAL del
        // Bibliotecario -- este metodo es distinto, filtra por un usuario.
        public int ContarReservasPendientesPorUsuario(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarReservasPendientesPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        //Lector de datos para registrar una reserva y obtener el mensaje de resultado + limites de reserva de libros a 3
        public bool RegistrarReserva(int idUsuario, int idLibro, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_RegistrarReserva", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLibro", idLibro);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message;
                return false;
            }
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