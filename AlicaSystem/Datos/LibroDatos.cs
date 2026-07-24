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
                    EstadoDisponibilidad = dr["estado_disponibilidad"].ToString()!
                };
            }
            return null;
        }
    }
}