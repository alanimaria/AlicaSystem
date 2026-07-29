using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class LibroDatos
    {
        private readonly ConexionBD conexionBD;

        public LibroDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public List<Libro> ListarCatalogo(string? busqueda, int? idCategoria, bool soloDisponibles)
        {
            var lista = new List<Libro>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarCatalogo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Busqueda", (object?)busqueda ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdCategoria", (object?)idCategoria ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SoloDisponibles", soloDisponibles);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Libro
                {
                    IdLibro = Convert.ToInt32(dr["id_libro"]),
                    Titulo = dr["titulo"].ToString()!,
                    Isbn = dr["isbn"] == DBNull.Value ? null : dr["isbn"].ToString(),
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    Categoria = dr["categoria"].ToString()!,
                    EstadoLibro = dr["estado_libro"].ToString()!,
                    CantidadDisponible = dr["cantidad_disponible"] == DBNull.Value ? 0 : Convert.ToInt32(dr["cantidad_disponible"]),
                    CantidadTotal = dr["cantidad_total"] == DBNull.Value ? 0 : Convert.ToInt32(dr["cantidad_total"]),
                    Autores = dr["lista_autores"] == DBNull.Value ? null : dr["lista_autores"].ToString(),
                    EstadoDisponibilidad = dr["estado_disponibilidad"].ToString()!
                });
            }
            return lista;
        }
        // Métodos nuevos para agregar libros en administrador

        public List<Libro> ListarLibrosAdmin()
        {
            var lista = new List<Libro>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarLibrosAdmin", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Libro
                {
                    IdLibro = Convert.ToInt32(dr["id_libro"]),
                    Titulo = dr["titulo"].ToString()!,
                    Isbn = dr["isbn"] == DBNull.Value ? null : dr["isbn"].ToString(),
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    IdCategoria = Convert.ToInt32(dr["id_categoria"]),
                    Categoria = dr["categoria"].ToString()!,
                    IdEstadoLibro = Convert.ToInt32(dr["id_estado_libro"]),
                    EstadoLibro = dr["estado_libro"].ToString()!,
                    CantidadDisponible = dr["cantidad_disponible"] == DBNull.Value ? 0 : Convert.ToInt32(dr["cantidad_disponible"]),
                    CantidadTotal = dr["cantidad_total"] == DBNull.Value ? 0 : Convert.ToInt32(dr["cantidad_total"]),
                    Ubicacion = dr["ubicacion"] == DBNull.Value ? null : dr["ubicacion"].ToString(),
                    Autores = dr["lista_autores"] == DBNull.Value ? null : dr["lista_autores"].ToString()
                });
            }
            return lista;
        }

        public int InsertarLibro(string titulo, string? isbn, string codigoInterno, int idCategoria, int idEstadoLibro, int cantidadTotal, string? ubicacion)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_InsertarLibro", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Titulo", titulo);
            cmd.Parameters.AddWithValue("@Isbn", (object?)isbn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CodigoInterno", codigoInterno);
            cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
            cmd.Parameters.AddWithValue("@IdEstadoLibro", idEstadoLibro);
            cmd.Parameters.AddWithValue("@CantidadTotal", cantidadTotal);
            cmd.Parameters.AddWithValue("@Ubicacion", (object?)ubicacion ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void AsociarAutorLibro(int idLibro, int idAutor)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_AsociarAutorLibro", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdLibro", idLibro);
            cmd.Parameters.AddWithValue("@IdAutor", idAutor);
            cmd.ExecuteNonQuery();
        }

        public void QuitarAutorLibro(int idLibro, int idAutor)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_QuitarAutorLibro", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdLibro", idLibro);
            cmd.Parameters.AddWithValue("@IdAutor", idAutor);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarLibro(int idLibro, string titulo, string? isbn, string codigoInterno, int idCategoria, int idEstadoLibro, int cantidadTotal, string? ubicacion)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ActualizarLibro", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdLibro", idLibro);
            cmd.Parameters.AddWithValue("@Titulo", titulo);
            cmd.Parameters.AddWithValue("@Isbn", (object?)isbn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CodigoInterno", codigoInterno);
            cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
            cmd.Parameters.AddWithValue("@IdEstadoLibro", idEstadoLibro);
            cmd.Parameters.AddWithValue("@CantidadTotal", cantidadTotal);
            cmd.Parameters.AddWithValue("@Ubicacion", (object?)ubicacion ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public bool EliminarLibro(int idLibro, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_EliminarLibro", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLibro", idLibro);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message;
                return false;
            }
        }
        public Libro? ObtenerLibroPorId(int idLibro)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ObtenerLibroPorId", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdLibro", idLibro);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new Libro
                {
                    IdLibro = Convert.ToInt32(dr["id_libro"]),
                    Titulo = dr["titulo"].ToString()!,
                    Isbn = dr["isbn"] == DBNull.Value ? null : dr["isbn"].ToString(),
                    CodigoInterno = dr["codigo_interno"].ToString()!,
                    Categoria = dr["categoria"].ToString()!,
                    EstadoLibro = dr["estado_libro"].ToString()!,
                    CantidadDisponible = dr["cantidad_disponible"] == DBNull.Value ? 0 : Convert.ToInt32(dr["cantidad_disponible"]),
                    CantidadTotal = dr["cantidad_total"] == DBNull.Value ? 0 : Convert.ToInt32(dr["cantidad_total"]),
                    Ubicacion = dr["ubicacion"] == DBNull.Value ? null : dr["ubicacion"].ToString(),
                    Autores = dr["lista_autores"] == DBNull.Value ? null : dr["lista_autores"].ToString(),
                    EstadoDisponibilidad = dr["estado_disponibilidad"].ToString()!,
                    Descripcion = dr["descripcion"] == DBNull.Value ? null : dr["descripcion"].ToString()
                };
            }
            return null;
        }

        // Cuenta el total de libros registrados en el catalogo.
        // Se usa para el KPI "Libros en catalogo" del Dashboard Administrador.
        public int ContarLibrosCatalogo()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarLibrosCatalogo", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}