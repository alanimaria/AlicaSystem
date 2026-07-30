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

    public class LibroMasPrestado
    {
        public string Libro { get; set; } = string.Empty;
        public int CantidadPrestamos { get; set; }
        public int BarraAncho { get; set; } // porcentaje 0-100, calculado en C#
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

        public (int IdLibro, string Titulo, string CodigoInterno, int CantidadDisponible)? BuscarLibroPorCodigo(string codigoInterno)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_BuscarLibroPorCodigo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodigoInterno", codigoInterno);
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return (
                    Convert.ToInt32(dr["id_libro"]),
                    dr["titulo"].ToString()!,
                    dr["codigo_interno"].ToString()!,
                    Convert.ToInt32(dr["cantidad_disponible"])
                );
            }
            return null;
        }

        public (int IdPrestamo, string Mensaje) RegistrarPrestamo(int idUsuario, int idLibro, int idEmpleado, int diasPlazo = 7)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_RegistrarPrestamo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@IdLibro", idLibro);
            cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
            cmd.Parameters.AddWithValue("@DiasPlazo", diasPlazo);
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                int idPrestamo = Convert.ToInt32(dr["IdPrestamo"]);
                string mensaje = dr["Mensaje"].ToString()!;
                return (idPrestamo, mensaje);
            }
            return (0, "No se pudo registrar el prestamo.");
        }

        public bool RegistrarDevolucion(int idPrestamo, int idEmpleado)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_RegistrarDevolucion", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdPrestamo", idPrestamo);
            cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public int ContarPrestamosActivosPorUsuario(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarPrestamosActivosPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        public List<PrestamoActivoResumen> ListarPrestamosActivosGlobal()
        {
            var lista = new List<PrestamoActivoResumen>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarPrestamosActivosGlobal", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new PrestamoActivoResumen
                {
                    Usuario = dr["usuario"].ToString()!,
                    Matricula = dr["matricula"].ToString()!,
                    Titulo = dr["titulo"].ToString()!,
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    FechaDevEsperada = Convert.ToDateTime(dr["fecha_dev_esperada"]),
                    DiasAtraso = Convert.ToInt32(dr["dias_atraso"])
                });
            }
            return lista;
        }
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
                    CantidadRenovaciones = Convert.ToInt32(dr["cantidad_renovaciones"]),
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    FechaPrestamo = Convert.ToDateTime(dr["fecha_prestamo"]),
                    FechaDevEsperada = Convert.ToDateTime(dr["fecha_dev_esperada"]),
                    Estado = dr["estado"].ToString()!
                });
            }
            return lista;
        }

        public List<Prestamo> ListarPrestamosPorUsuario(int idUsuario)
        {
            var lista = new List<Prestamo>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarPrestamosPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Prestamo
                {
                    IdPrestamo = Convert.ToInt32(dr["id_prestamo"]),
                    Titulo = dr["titulo"].ToString()!,
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    CantidadRenovaciones = Convert.ToInt32(dr["cantidad_renovaciones"]),
                    FechaPrestamo = Convert.ToDateTime(dr["fecha_prestamo"]),
                    FechaDevEsperada = Convert.ToDateTime(dr["fecha_dev_esperada"]),
                    FechaDevReal = dr["fecha_dev_real"] == DBNull.Value ? null : Convert.ToDateTime(dr["fecha_dev_real"]),
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

        // Trae los libros mas prestados para el grafico de barras del
        // Dashboard Administrador. BarraAncho se calcula relativo al
        // libro con mas prestamos (ese queda en 100%).
        public List<LibroMasPrestado> ListarTopLibrosMasPrestados(int top = 4)
        {
            var lista = new List<LibroMasPrestado>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_TopLibrosMasPrestados", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Top", top);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new LibroMasPrestado
                {
                    Libro = dr["Libro"].ToString()!,
                    CantidadPrestamos = Convert.ToInt32(dr["CantidadPrestamos"])
                });
            }

            if (lista.Count > 0)
            {
                int max = lista[0].CantidadPrestamos;
                foreach (var item in lista)
                {
                    item.BarraAncho = max == 0 ? 0 : (int)Math.Round(item.CantidadPrestamos * 100.0 / max);
                }
            }

            return lista;
        }
    }
}