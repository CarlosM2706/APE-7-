using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Vehiculos_API.Models;
using Vehiculos_API.Services;

namespace Vehiculos_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class VehiculoController : ControllerBase
    {
        private readonly VehiculoService _service;

        public VehiculoController(VehiculoService service)
        {
            _service = service;
        }


        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var lista = _service.ObtenerVehiculos();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener vehículos: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody] Vehiculo vehiculo)
        {
            if (string.IsNullOrWhiteSpace(vehiculo.Marca) ||
                string.IsNullOrWhiteSpace(vehiculo.Modelo) ||
                vehiculo.Año <= 0) 
            {
                return BadRequest("Datos del vehículo inválidos");
            }

            try
            {
                _service.AgregarVehiculo(vehiculo.Marca, vehiculo.Modelo, vehiculo.Año);
                return Ok("Vehículo agregado correctamente");
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error SQL al insertar vehículo: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error general: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] Vehiculo vehiculo)
        {
            try
            {
                _service.ActualizarVehiculo(vehiculo);
                return Ok("Vehículo actualizado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar vehículo: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.EliminarVehiculo(id);
                return Ok("Vehículo eliminado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar vehículo: {ex.Message}");
            }
        }

    }
}
