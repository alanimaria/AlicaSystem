using AlicaSystem.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AlicaSystem.Datos
{
    // Esta clase se encarga de todo lo relacionado a RESERVAS
    public class ReservaDatos
    {
        private readonly ConexionBD conexionBD;

        public ReservaDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        // Devuelve cuantas reservas estan pendientes de ser recogidas
        // por el lector (todavia no expiraron ni se entregaron)
        public int ContarReservasPendientes()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ContarReservasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Trae la lista de reservas pendientes + expiradas recientes (ultimas 24h),
        // con los datos de usuario y libro ya resueltos, para mostrar en
        // la pantalla "Reservas pendientes" del bibliotecario.
        // El SP tambien auto-expira (y restaura inventario) las reservas vencidas
        // cada vez que se llama, ya que no tenemos SQL Agent Job en Azure.
        public List<Reserva> ListarReservasPendientes()
        {
            var lista = new List<Reserva>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ListarReservasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Reserva
                {
                    IdReserva = Convert.ToInt32(dr["id_reserva"]),
                    Usuario = dr["Usuario"].ToString()!,
                    Libro = dr["Libro"].ToString()!,
                    FechaReserva = Convert.ToDateTime(dr["fecha_reserva"]),
                    FechaExpiracion = Convert.ToDateTime(dr["fecha_expiracion"]),
                    Estado = dr["Estado"].ToString()!
                });
            }

            return lista;
        }

        // Cambia el estado de una reserva (a "COMPLETADA" o "CANCELADA").
        // idEmpleado es obligatorio cuando nuevoEstado es "COMPLETADA", porque
        // el SP crea un prestamo real y necesita saber quien lo registro.
        // Devuelve true si se logro actualizar, false si no
        // (por ejemplo, si la reserva ya no estaba pendiente)
        public bool ActualizarEstadoReserva(int idReserva, string nuevoEstado, int? idEmpleado)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ActualizarEstadoReserva", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);
            cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
            cmd.Parameters.AddWithValue("@IdEmpleado", (object?)idEmpleado ?? DBNull.Value);

            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());
            return filasAfectadas > 0;
        }
    }
}