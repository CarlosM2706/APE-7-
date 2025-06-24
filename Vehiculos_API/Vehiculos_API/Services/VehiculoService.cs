using Vehiculos_API.Excepciones;
using Microsoft.Data.SqlClient;
using Vehiculos_API.Models;


namespace Vehiculos_API.Services
{
    public class VehiculoService
    {
        private readonly string _cadenaConexion;

        public VehiculoService(IConfiguration config)
        {
            _cadenaConexion = config.GetConnectionString("DefaultConnection");
        }

        public void AgregarVehiculo(string marca, string modelo, int anio)
        {
            if (string.IsNullOrEmpty(marca) || string.IsNullOrEmpty(modelo) || anio <= 0)
            {
                throw new ArgumentException("Datos inválidos para el vehículo.");
            }

            SqlExceptionManager.EjecutarConManejoExcepciones(() =>
            {
                using (var conn = new SqlConnection(_cadenaConexion))
                {
                    conn.Open();
                    string query = "INSERT INTO Vehiculos (Marca, Modelo, Año) VALUES (@marca, @modelo, @anio)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@marca", marca);
                        cmd.Parameters.AddWithValue("@modelo", modelo);
                        cmd.Parameters.AddWithValue("@anio", anio);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        public List<Vehiculo> ObtenerVehiculos()
        {
            var lista = new List<Vehiculo>();

            SqlExceptionManager.EjecutarConManejoExcepciones(() =>
            {
                using var conn = new SqlConnection(_cadenaConexion);
                conn.Open();

                var query = "SELECT Id, Marca, Modelo, Año FROM Vehiculos";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Vehiculo
                    {
                        Id = (int)reader["Id"],
                        Marca = reader["Marca"].ToString(),
                        Modelo = reader["Modelo"].ToString(),
                        Año = (int)reader["Año"]
                    });
                }
            });

            return lista;
        }

        public void ActualizarVehiculo(Vehiculo vehiculo)
        {
            SqlExceptionManager.EjecutarConManejoExcepciones(() =>
            {
                using var conn = new SqlConnection(_cadenaConexion);
                conn.Open();

                var query = "UPDATE Vehiculos SET Marca = @marca, Modelo = @modelo, Año = @anio WHERE Id = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@marca", vehiculo.Marca);
                cmd.Parameters.AddWithValue("@modelo", vehiculo.Modelo);
                cmd.Parameters.AddWithValue("@anio", vehiculo.Año);
                cmd.Parameters.AddWithValue("@id", vehiculo.Id);

                int filasAfectadas = cmd.ExecuteNonQuery();

                if (filasAfectadas == 0)
                {
                    throw new KeyNotFoundException($"No se encontró el vehículo con Id = {vehiculo.Id}");
                }
            });
        }

        public void EliminarVehiculo(int id)
        {
            SqlExceptionManager.EjecutarConManejoExcepciones(() =>
            {
                using var conn = new SqlConnection(_cadenaConexion);
                conn.Open();

                var query = "DELETE FROM Vehiculos WHERE Id = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                int filasAfectadas = cmd.ExecuteNonQuery();

                if (filasAfectadas == 0)
                {
                    throw new KeyNotFoundException($"No se encontró el vehículo con Id = {id}");
                }
            });
        }

    }
}
