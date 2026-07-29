using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class ResumenMultasPendientes
    {
        public decimal TotalPendiente { get; set; }
        public int UsuariosConSaldo { get; set; }
    }

    // Esta clase se encarga de todo lo relacionado a MULTAS, tanto para
    // el Bibliotecario (Dashboard + pantalla "Multas") como para el
    // Lector (Dashboard + "Mis multas") y el Administrador (KPI del
    // Dashboard).
    public class MultaDatos
    {
        private readonly ConexionBD conexionBD;

        public MultaDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public int ContarMultasPendientes()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ContarMultasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ---- Metodos para el Bibliotecario (pantalla "Multas") ----

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

        public int ContarMultasPendientesPorUsuario(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarMultasPendientesPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

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

        // ---- Metodo para el Administrador (Dashboard) ----

        public ResumenMultasPendientes ObtenerResumenMultasPendientes()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_SumarMultasPendientes", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new ResumenMultasPendientes
                {
                    TotalPendiente = Convert.ToDecimal(dr["TotalPendiente"]),
                    UsuariosConSaldo = Convert.ToInt32(dr["UsuariosConSaldo"])
                };
            }
            return new ResumenMultasPendientes();
        }
    }
}