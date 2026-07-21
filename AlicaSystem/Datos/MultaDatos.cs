using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    // Esta clase se encarga de todo lo relacionado a MULTAS que necesita
    // el dashboard y la pantalla de gestion de multas del bibliotecario
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

        // Trae todas las multas pendientes de pago, con el nombre
        // del usuario ya resuelto, para la pantalla de "Multas"
        public List<Multa> ListarMultas()
        {
            var lista = new List<Multa>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ListarMultas", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Multa
                {
                    IdMulta = Convert.ToInt32(dr["id_multa"]),
                    Usuario = dr["Usuario"].ToString()!,
                    Monto = Convert.ToDecimal(dr["monto"]),
                    FechaGeneracion = Convert.ToDateTime(dr["fecha_generacion"]),
                    FechaPago = dr["fecha_pago"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_pago"])
                });
            }

            return lista;
        }

        // Marca una multa como pagada. Devuelve true si funciono
        public bool ActualizarEstadoMulta(int idMulta)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ActualizarEstadoMulta", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdMulta", idMulta);

            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());
            return filasAfectadas > 0;
        }
    }
}