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
    // la lista de actividad reciente), y tambien de registrar prestamos
    // y devoluciones nuevas
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

        // Busca un libro por su codigo interno (el que el bibliotecario
        // escribe o escanea al momento de prestar).
        // Devuelve null si no se encuentra ningun libro con ese codigo.
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

        // Registra un prestamo nuevo.
        // El stored procedure ahora valida ADEMAS de las copias disponibles:
        // que el usuario no tenga multas pendientes, y que no tenga ya
        // 3 prestamos activos. Por eso ahora devolvemos tambien el
        // "Mensaje" que arma el SP explicando el motivo exacto (sea
        // exito o el motivo del rechazo), para no tener que adivinarlo
        // aqui en C#.
        // IdPrestamo viene en 0 si el prestamo no se pudo registrar
        // por cualquiera de esas razones.
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

        // Registra la devolucion de un prestamo activo.
        // Devuelve true si se registro bien, false si el prestamo no existe
        // o ya estaba devuelto.
        public bool RegistrarDevolucion(int idPrestamo)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("sp_RegistrarDevolucion", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdPrestamo", idPrestamo);

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }
    }
}