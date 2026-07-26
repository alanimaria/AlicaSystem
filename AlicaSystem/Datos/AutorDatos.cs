// Datos/AutorDatos.cs
using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class AutorDatos
    {
        private readonly ConexionBD conexionBD;

        public AutorDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }
        public List<Autor> BuscarAutores(string texto)
        {
            var lista = new List<Autor>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_BuscarAutores", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Texto", texto);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Autor
                {
                    IdAutor = Convert.ToInt32(dr["id_autor"]),
                    NombreCompleto = dr["nombre_completo"].ToString()!,
                    PaisOrigen = dr["pais_origen"].ToString()!
                });
            }
            return lista;
        }
        public List<Autor> ListarAutores()
        {
            var lista = new List<Autor>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarAutores", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Autor
                {
                    IdAutor = Convert.ToInt32(dr["id_autor"]),
                    NombreCompleto = dr["nombre_completo"].ToString()!,
                    PaisOrigen = dr["pais_origen"].ToString()!
                });
            }
            return lista;
        }

        public void InsertarAutor(string nombreCompleto, string paisOrigen)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_InsertarAutor", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);
            cmd.Parameters.AddWithValue("@PaisOrigen", paisOrigen);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarAutor(int idAutor, string nombreCompleto, string paisOrigen)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ActualizarAutor", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdAutor", idAutor);
            cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);
            cmd.Parameters.AddWithValue("@PaisOrigen", paisOrigen);
            cmd.ExecuteNonQuery();
        }

        public bool EliminarAutor(int idAutor, out string? error)
        {
            error = null;
            try
            {
                using SqlConnection cn = conexionBD.ObtenerConexion();
                cn.Open();
                using SqlCommand cmd = new SqlCommand("sp_EliminarAutor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAutor", idAutor);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}