using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    // Esta clase se encarga de todo lo relacionado a MULTAS que necesita
    // el dashboard del bibliotecario

    public class MultaDatos
    {
        private readonly ConexionBD conexionBD;

        public MultaDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        // Devuelve cuantos usuarios tienen multas sin pagar todavia
        // (fecha_pago = NULL significa que la multa sigue pendiente)
        public int ContarMultasPendientes()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ContarMultasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        public (bool Exito, string Mensaje) RegistrarMultaPorEstadoLibro(int idPrestamo, int idEmpleado, decimal monto)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_RegistrarMultaPorEstadoLibro", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdPrestamo", idPrestamo);
            cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
            cmd.Parameters.AddWithValue("@Monto", monto);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bool exito = Convert.ToInt32(dr["Exito"]) == 1;
                string mensaje = dr["Mensaje"].ToString()!;
                return (exito, mensaje);
            }
            return (false, "No se pudo registrar la multa.");
        }

        // Cuenta cuántas multas sin pagar (fecha_pago = NULL) tiene un
        // usuario específico. Se usa para el KPI "Multas pendientes"
        // del Dashboard Lector.
        public int ContarMultasPendientesPorUsuario(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarMultasPendientesPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Lista las multas de un usuario específico, con el libro que
        // las generó (vista Lector: Mis préstamos, pestaña Multas)
        public List<Multa> ListarMultasPorUsuario(int idUsuario)
        {
            var lista = new List<Multa>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarMultasPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Multa
                {
                    IdMulta = Convert.ToInt32(dr["id_multa"]),
                    Titulo = dr["titulo"].ToString()!,
                    Monto = Convert.ToDecimal(dr["monto"]),
                    FechaGeneracion = Convert.ToDateTime(dr["fecha_generacion"]),
                    FechaPago = dr["fecha_pago"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_pago"]),
                    Estado = dr["estado"].ToString()!,
                    Motivo = dr["motivo"] == DBNull.Value ? null : dr["motivo"].ToString()
                });
            }
            return lista;
        }
    }
}