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
        public int ContarMultasPendientes()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ContarMultasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Trae TODAS las multas (Pendiente, Pagada, Perdonada) con el
        // detalle del prestamo asociado, para la pantalla "Multas".
        // El filtrado por pestana se hace en el frontend con JS.
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
                    Matricula = dr["Matricula"].ToString()!,
                    IdPrestamo = Convert.ToInt32(dr["id_prestamo"]),
                    Libro = dr["Libro"].ToString()!,
                    FechaEsperada = Convert.ToDateTime(dr["FechaEsperada"]),
                    DiasAtraso = Convert.ToInt32(dr["DiasAtraso"]),
                    Monto = Convert.ToDecimal(dr["monto"]),
                    FechaGeneracion = Convert.ToDateTime(dr["fecha_generacion"]),
                    FechaPago = dr["fecha_pago"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_pago"]),
                    Estado = dr["Estado"].ToString()!
                });
            }

            return lista;
        }

        // Marca una multa como Pagada o Perdonada. Devuelve true si funciono.
        // Solo se puede aplicar sobre una multa que este en estado Pendiente.
        public bool ActualizarEstadoMulta(int idMulta, string estadoDestino)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ActualizarEstadoMulta", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdMulta", idMulta);
            cmd.Parameters.AddWithValue("@EstadoDestino", estadoDestino);

            int filasAfectadas = Convert.ToInt32(cmd.ExecuteScalar());
            return filasAfectadas > 0;
        }
    }
}