using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class ActividadReciente
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Libro { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
    }

    public class PrestamoDatos
    {
        private readonly ConexionBD conexionBD;

        public PrestamoDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public int ContarPrestamosActivos()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarPrestamosActivos", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<ActividadReciente> ListarActividadReciente(int top = 10)
        {
            var lista = new List<ActividadReciente>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarActividadReciente", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Top", top);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ActividadReciente
                {
                    Fecha = Convert.ToDateTime(dr["Fecha"]),
                    Usuario = dr["Usuario"].ToString()!,
                    Libro = dr["Libro"].ToString()!,
                    Accion = dr["Accion"].ToString()!
                });
            }
            return lista;
        }
        // Cuenta cuántos préstamos activos tiene un usuario específico
        // (un préstamo esta "activo" si todavia no se ha devuelto el libro,
        // es decir, fecha_dev_real IS NULL). Se usa para el KPI
        // "Préstamos activos X/3" del Dashboard Lector.
        public int ContarPrestamosActivosPorUsuario(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarPrestamosActivosPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ---- Métodos agregados para Lector (Mis préstamos) ----

        public List<Prestamo> ListarPrestamosActivosPorUsuario(int idUsuario)
        {
            var lista = new List<Prestamo>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarPrestamosActivosPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Prestamo
                {
                    IdPrestamo = Convert.ToInt32(dr["id_prestamo"]),
                    Titulo = dr["titulo"].ToString()!,
                    Renovado = Convert.ToBoolean(dr["renovado"]),
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    FechaPrestamo = Convert.ToDateTime(dr["fecha_prestamo"]),
                    FechaDevEsperada = Convert.ToDateTime(dr["fecha_dev_esperada"]),
                    Estado = dr["estado"].ToString()!

                });
            }
            return lista;
        }

        public (bool Exito, string Mensaje) RenovarPrestamo(int idPrestamo, int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_RenovarPrestamo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdPrestamo", idPrestamo);
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bool exito = Convert.ToInt32(dr["Exito"]) == 1;
                string mensaje = dr["Mensaje"].ToString()!;
                return (exito, mensaje);
            }
            return (false, "No se pudo renovar el préstamo.");
        }
    }
}