using Microsoft.Data.SqlClient; 


namespace Vehiculos_API.Excepciones
{
    public class SqlExceptionManager
    {
        public static void EjecutarConManejoExcepciones(Action accion)
        {
            try
            {
                accion();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[ERROR SQL] {ex.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GENERAL] {ex.Message}");
                throw;
            }
        }
    }
}
