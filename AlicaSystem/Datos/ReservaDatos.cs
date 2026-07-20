using System.Data;
using Microsoft.Data.SqlClient;

namespace AlicaSystem.Datos
{
    // Esta clase se encarga de todo lo relacionado a RESERVAS que necesita
    // el dashboard del bibliotecario
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
    }
}