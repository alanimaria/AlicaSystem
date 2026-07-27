using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    // Esta clase se encarga de todo lo relacionado a MULTAS, tanto para
    // el Bibliotecario (Dashboard + pantalla "Multas") como para el
    // Lector (Dashboard + "Mis multas").
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

        // ---- Metodos para el Bibliotecario (pantalla "Multas") ----

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

        // ---- Metodos para el Lector (Dashboard + "Mis multas") ----

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