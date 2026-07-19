using System.Data;
using Microsoft.Data.SqlClient;
using AlicaSystem.Models;

namespace AlicaSystem.Datos
{
    public class CategoriaDatos
    {
        private readonly ConexionBD conexionBD;

        public CategoriaDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public List<Categoria> ListarCategorias()
        {
            List<Categoria> lista = new List<Categoria>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ListarCategorias", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Categoria
                {
                    IdCategoria = Convert.ToInt32(dr["id_categoria"]),
                    Nombre = dr["nombre"].ToString()!,
                    Descripcion = dr["descripcion"] == DBNull.Value ? null : dr["descripcion"].ToString()
                });
            }
            return lista;
        }

        public void InsertarCategoria(string nombre, string? descripcion)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_InsertarCategoria", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)descripcion ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarCategoria(int idCategoria, string nombre, string? descripcion)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_ActualizarCategoria", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)descripcion ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void EliminarCategoria(int idCategoria)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlCommand cmd = new SqlCommand("sp_EliminarCategoria", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
            cmd.ExecuteNonQuery();
        }
    }
}