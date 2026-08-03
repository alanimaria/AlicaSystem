using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    // Esta clase se encarga de todo lo relacionado a RESERVAS,
    // tanto para el Bibliotecario (Reservas pendientes) como
    // para el Lector (Mis reservas).
    public class ReservaDatos
    {
        private readonly ConexionBD conexionBD;

        public ReservaDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        // Devuelve cuantas reservas estan pendientes de ser recogidas
        // (todavia no expiraron ni se entregaron). Usado en ambos dashboards.
        public int ContarReservasPendientes()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ContarReservasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ---- Metodos para el Bibliotecario (pantalla "Reservas pendientes") ----

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
        public (bool Exito, string Mensaje) ActualizarEstadoReserva(int idReserva, string nuevoEstado, int? idEmpleado)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ActualizarEstadoReserva", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);
            cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
            cmd.Parameters.AddWithValue("@IdEmpleado", (object?)idEmpleado ?? DBNull.Value);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bool exito = Convert.ToInt32(dr["FilasAfectadas"]) > 0;
                string mensaje = dr["Mensaje"].ToString()!;
                return (exito, mensaje);
            }
            return (false, "No se pudo actualizar la reserva.");
        }

        // ---- Metodos para el Lector (pantalla "Mis reservas") ----

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
        // que es el conteo GLOBAL para el dashboard del Bibliotecario.
        public int ContarReservasPendientesPorUsuario(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarReservasPendientesPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Registra una reserva y obtiene el mensaje de resultado
        // (limites de reserva, disponibilidad, etc, validados en el SP).
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