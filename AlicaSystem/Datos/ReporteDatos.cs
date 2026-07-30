using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class ReporteDatos
    {
        private readonly ConexionBD conexionBD;

        public ReporteDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public List<ReporteEstado> ReportePrestamosPorPeriodo(DateTime desde, DateTime hasta)
        {
            var lista = new List<ReporteEstado>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ReportePrestamosPorPeriodo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FechaDesde", desde);
            cmd.Parameters.AddWithValue("@FechaHasta", hasta);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ReporteEstado
                {
                    Estado = dr["estado"].ToString()!,
                    Total = Convert.ToInt32(dr["total"])
                });
            }
            return lista;
        }

        public List<ReporteCatalogo> ReporteDisponibilidadCatalogo()
        {
            var lista = new List<ReporteCatalogo>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ReporteDisponibilidadCatalogo", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ReporteCatalogo
                {
                    Estado = dr["estado"].ToString()!,
                    Total = Convert.ToInt32(dr["total"]),
                    EjemplaresDisponibles = dr["ejemplares_disponibles"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ejemplares_disponibles"]),
                    EjemplaresTotales = dr["ejemplares_totales"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ejemplares_totales"])
                });
            }
            return lista;
        }

        public List<ReporteMulta> ReporteMultasPorPeriodo(DateTime desde, DateTime hasta)
        {
            var lista = new List<ReporteMulta>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ReporteMultasPorPeriodo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FechaDesde", desde);
            cmd.Parameters.AddWithValue("@FechaHasta", hasta);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ReporteMulta
                {
                    Estado = dr["estado"].ToString()!,
                    Total = Convert.ToInt32(dr["total"]),
                    MontoTotal = Convert.ToDecimal(dr["monto_total"])
                });
            }
            return lista;
        }

        public List<ReporteRanking> ReporteLibrosMasPrestados()
        {
            var lista = new List<ReporteRanking>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ReporteLibrosMasPrestados", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ReporteRanking
                {
                    Nombre = dr["titulo"].ToString()!,
                    Cantidad = Convert.ToInt32(dr["cantidad_prestamos"])
                });
            }
            return lista;
        }

        public List<ReporteRanking> ReporteUsuariosMasActivos()
        {
            var lista = new List<ReporteRanking>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ReporteUsuariosMasActivos", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ReporteRanking
                {
                    Nombre = dr["usuario"].ToString()!,
                    Cantidad = Convert.ToInt32(dr["cantidad_prestamos"])
                });
            }
            return lista;
        }
    }
}