// Datos/ListaDatos.cs (reemplaza el anterior completo)
using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class ListaDatos
    {
        private readonly ConexionBD conexionBD;
        public ListaDatos(ConexionBD conexionBD) { this.conexionBD = conexionBD; }

        public List<ListaResumen> ListarListasPorUsuario(int idUsuario, string? busqueda)
        {
            var resultado = new List<ListaResumen>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarListasPorUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@Busqueda", (object?)busqueda ?? DBNull.Value);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                resultado.Add(new ListaResumen
                {
                    NombreLista = dr["nombre_lista"].ToString()!,
                        CantidadLibros = Convert.ToInt32(dr["cantidad_libros"]),
                        UltimaActualizacion = dr["ultima_actualizacion"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(dr["ultima_actualizacion"])
                });
            }
            return resultado;
        }
        public bool CrearLista(int idUsuario, string nombreLista, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_CrearLista", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@NombreLista", nombreLista);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message;
                return false;
            }
        }
        public void QuitarLibroDeLista(int idUsuario, int idLibro, string nombreLista)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_QuitarLibroDeLista", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@IdLibro", idLibro);
            cmd.Parameters.AddWithValue("@NombreLista", nombreLista);
            cmd.ExecuteNonQuery();
        }
        public List<string> ObtenerNombresListas(int idUsuario)
        {
            var resultado = new List<string>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarNombresListas", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                resultado.Add(dr["nombre_lista"].ToString()!);
            }
            return resultado;
        }

        public int ContarListasActivas(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ContarListasActivas", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<LibroEnLista> ListarLibrosDeLista(int idUsuario, string nombreLista)
        {
            var resultado = new List<LibroEnLista>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarLibrosDeLista", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@NombreLista", nombreLista);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                resultado.Add(new LibroEnLista
                {
                    IdLibro = Convert.ToInt32(dr["id_libro"]),
                    Titulo = dr["titulo"].ToString()!,
                    FechaAgregado = Convert.ToDateTime(dr["fecha_libro_agregado"])
                });
            }
            return resultado;
        }
        public (bool Exito, string Mensaje) AgregarLibroALista(int idUsuario, int idLibro, string nombreLista)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_AgregarLibroALista", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@IdLibro", idLibro);
            cmd.Parameters.AddWithValue("@NombreLista", nombreLista);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bool exito = Convert.ToInt32(dr["Exito"]) == 1;
                string mensaje = dr["Mensaje"].ToString()!;
                return (exito, mensaje);
            }
            return (false, "No se pudo agregar el libro a la lista.");
        }
        public void RenombrarLista(int idUsuario, string nombreActual, string nombreNuevo)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_RenombrarLista", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@NombreActual", nombreActual);
            cmd.Parameters.AddWithValue("@NombreNuevo", nombreNuevo);
            cmd.ExecuteNonQuery();
        }

        public void EliminarLista(int idUsuario, string nombreLista)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_EliminarLista", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@NombreLista", nombreLista);
            cmd.ExecuteNonQuery();
        }
    }
}