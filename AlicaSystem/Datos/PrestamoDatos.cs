using System.Data;
using Microsoft.Data.SqlClient;

namespace AlicaSystem.Datos
{
    // Esta clase representa una sola fila de "actividad" en el dashboard
    // (un prestamo, una devolucion o una reserva), para mostrarla en la tabla
    // de "Actividad reciente" del panel del bibliotecario
    public class ActividadReciente
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Libro { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty; // "Prestamo", "Devolucion" o "Reserva"
    }

    // Esta clase se encarga de todo lo relacionado a PRESTAMOS que necesita
    // el dashboard del bibliotecario (contar cuantos hay activos, y traer
    // la lista de actividad reciente)
    public class PrestamoDatos
    {
        // Guardamos la conexion a la base de datos para poder usarla
        // en los metodos de abajo
        private readonly ConexionBD conexionBD;

        // El constructor recibe la conexion ya lista (inyeccion de dependencias)
        public PrestamoDatos(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        // Devuelve cuantos prestamos estan activos ahora mismo
        // (un prestamo esta "activo" si todavia no se ha devuelto el libro)
        public int ContarPrestamosActivos()
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            // Llamamos al stored procedure que hace el conteo en la base de datos
            using SqlCommand cmd = new SqlCommand("sp_ContarPrestamosActivos", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            // ExecuteScalar() se usa cuando el resultado es un solo valor (un numero),
            // no una tabla completa
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Trae los ultimos movimientos (prestamos, devoluciones, reservas)
        // para mostrarlos en la tabla de "Actividad reciente"
        // "top" define cuantos registros traer como maximo (por defecto 10)
        public List<ActividadReciente> ListarActividadReciente(int top = 10)
        {
            var lista = new List<ActividadReciente>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_ListarActividadReciente", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Top", top);

            // ExecuteReader() se usa cuando el resultado es una tabla con varias filas
            using SqlDataReader dr = cmd.ExecuteReader();

            // Recorremos cada fila que devuelve el stored procedure
            // y la convertimos en un objeto ActividadReciente
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
    }
}