using System.Data;
using Microsoft.Data.SqlClient;

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

        // Cuenta cuántas multas sin pagar (fecha_pago = NULL) tiene un
        // usuario específico. Se usa para el KPI "Multas pendientes"
        // del Dashboard Lector.
        //
        // NOTA: no confundir con ContarMultasPendientes() (sin usuario),
        // que ya existe en esta misma clase para el dashboard GLOBAL del
        // Bibliotecario -- este metodo es distinto, filtra por un usuario.
        public int ContarMultasPendientesPorUsuario(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarMultasPendientesPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}