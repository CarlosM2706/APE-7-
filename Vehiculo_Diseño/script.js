const API_URL = 'https://localhost:7128/api/Vehiculo';

const tableBody = document.querySelector('#vehiculosTable tbody');
const form = document.getElementById('vehiculoForm');
const idInput = document.getElementById('vehiculoId');
const marcaInput = document.getElementById('marca');
const modeloInput = document.getElementById('modelo');
const anioInput = document.getElementById('anio');
const messageDiv = document.getElementById('message');
const cancelEditBtn = document.getElementById('cancelEdit');


async function cargarVehiculos() {
  try {
    const res = await fetch(API_URL);
    const vehiculos = await res.json();
    tableBody.innerHTML = '';
    vehiculos.forEach(v => {
      const row = document.createElement('tr');
      row.innerHTML = `
        <td data-label="ID">${v.id}</td>
        <td data-label="Marca">${v.marca}</td>
        <td data-label="Modelo">${v.modelo}</td>
        <td data-label="Año">${v.año}</td>
        <td>
            <button class="edit" onclick="editarVehiculo(${v.id}, '${v.marca}', '${v.modelo}', ${v.año})">📝 Editar</button>
            <button class="delete" onclick="eliminarVehiculo(${v.id})">🗑️ Eliminar</button>
        </td>
      `;
      tableBody.appendChild(row);
    });
  } catch (error) {
    mostrarMensaje('Error al cargar vehículos', 'error');
  }
}

// Mostrar mensaje
function mostrarMensaje(msg, tipo='success') {
  messageDiv.textContent = msg;
  messageDiv.className = `message ${tipo}`;
  setTimeout(() => messageDiv.textContent = '', 4000);
}


form.addEventListener('submit', async e => {
  e.preventDefault();

  // Limpiar mensajes previos
  document.getElementById('errorMarca').textContent = '';
  document.getElementById('errorModelo').textContent = '';
  document.getElementById('errorAnio').textContent = '';
  messageDiv.textContent = '';
  messageDiv.className = 'message';

  const marca = marcaInput.value.trim();
  const modelo = modeloInput.value.trim();
  const anio = parseInt(anioInput.value);

  let valid = true;

  if (!marca) {
    document.getElementById('errorMarca').textContent = 'La marca es obligatoria.';
    valid = false;
  }

  if (!modelo) {
    document.getElementById('errorModelo').textContent = 'El modelo es obligatorio.';
    valid = false;
  }

  if (isNaN(anio) || anio < 1900 || anio > 2100) {
    document.getElementById('errorAnio').textContent = 'El año es obligatorio.';
    valid = false;
  }

  if (!valid) return; 

  const vehiculo = {
    id: idInput.value ? parseInt(idInput.value) : 0,
    marca,
    modelo,
    año: anio,
  };

  try {
    let res;
    if (vehiculo.id === 0) {
      res = await fetch(API_URL, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(vehiculo),
      });
    } else {
      res = await fetch(API_URL, {
        method: 'PUT',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(vehiculo),
      });
    }

    if (!res.ok) {
      const error = await res.text();
      mostrarMensaje(`Error: ${error}`, 'error');
      return;
    }

    mostrarMensaje(vehiculo.id === 0 ? 'Vehículo agregado' : 'Vehículo actualizado');
    form.reset();
    idInput.value = '';
    cancelEditBtn.style.display = 'none';
    cargarVehiculos();
  } catch (error) {
    mostrarMensaje('Error en la comunicación con el servidor', 'error');
  }
});



// Editar vehículo (carga datos en el formulario)
window.editarVehiculo = (id, marca, modelo, anio) => {
  idInput.value = id;
  marcaInput.value = marca;
  modeloInput.value = modelo;
  anioInput.value = anio;
  cancelEditBtn.style.display = 'inline';
  marcaInput.focus();
};

// Cancelar edición
cancelEditBtn.addEventListener('click', () => {
  form.reset();
  idInput.value = '';
  cancelEditBtn.style.display = 'none';
  messageDiv.textContent = '';
});

// Eliminar vehículo
window.eliminarVehiculo = async (id) => {
  if (!confirm('¿Seguro que deseas eliminar este vehículo?')) return;

  try {
    const res = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
    if (!res.ok) {
      const error = await res.text();
      mostrarMensaje(`Error: ${error}`, 'error');
      return;
    }
    mostrarMensaje('Vehículo eliminado');
    cargarVehiculos();
  } catch {
    mostrarMensaje('Error en la comunicación con el servidor', 'error');
  }
};

cargarVehiculos();
