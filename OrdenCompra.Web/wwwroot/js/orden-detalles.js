document.addEventListener('DOMContentLoaded', function () {
    let detalleIndex = document.querySelectorAll('.detalle-row').length;

    // Función para calcular subtotal
    function calcularSubtotal(row) {
        const cantidad = parseFloat(row.querySelector('.cantidad').value) || 0;
        const precio = parseFloat(row.querySelector('.precio').value) || 0;
        const subtotal = cantidad * precio;
        row.querySelector('.subtotal').value = subtotal.toLocaleString('es-PE', {
            style: 'currency',
            currency: 'PEN'
        });
        calcularTotal();
    }

    // Función para calcular total
    function calcularTotal() {
        let total = 0;
        document.querySelectorAll('.detalle-row').forEach(function (row) {
            const cantidad = parseFloat(row.querySelector('.cantidad').value) || 0;
            const precio = parseFloat(row.querySelector('.precio').value) || 0;
            total += cantidad * precio;
        });

        document.getElementById('total-orden').textContent = total.toLocaleString('es-PE', {
            style: 'currency',
            currency: 'PEN'
        });
    }

    // Eventos para calcular subtotales
    document.addEventListener('input', function (e) {
        if (e.target.classList.contains('cantidad') || e.target.classList.contains('precio')) {
            const row = e.target.closest('.detalle-row');
            calcularSubtotal(row);
        }
    });

    // Agregar nuevo detalle
    document.getElementById('agregar-detalle').addEventListener('click', function () {
        const container = document.getElementById('detalles-container');
        const newRow = document.createElement('div');
        newRow.className = 'detalle-row border rounded p-3 mb-3 bg-light';
        newRow.innerHTML = `
            <div class="row">
                <div class="col-md-4">
                    <label class="form-label">Producto</label>
                    <input name="Detalles[${detalleIndex}].Producto" class="form-control" placeholder="Nombre del producto" />
                    <span class="text-danger"></span>
                </div>
                <div class="col-md-2">
                    <label class="form-label">Cantidad</label>
                    <input name="Detalles[${detalleIndex}].Cantidad" class="form-control cantidad" type="number" min="1" />
                    <span class="text-danger"></span>
                </div>
                <div class="col-md-3">
                    <label class="form-label">Precio Unitario</label>
                    <input name="Detalles[${detalleIndex}].PrecioUnitario" class="form-control precio" type="number" step="0.01" min="0" />
                    <span class="text-danger"></span>
                </div>
                <div class="col-md-2">
                    <label class="form-label">Subtotal</label>
                    <input class="form-control subtotal" type="text" readonly />
                </div>
                <div class="col-md-1">
                    <label class="form-label">&nbsp;</label>
                    <button type="button" class="btn btn-danger btn-sm d-block remover-detalle">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
        `;
        container.appendChild(newRow);
        detalleIndex++;
    });

    // Remover detalle
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('remover-detalle') || e.target.closest('.remover-detalle')) {
            const rows = document.querySelectorAll('.detalle-row');
            if (rows.length > 1) {
                const row = e.target.closest('.detalle-row');
                row.remove();
                calcularTotal();
                reindexarDetalles();
            } else {
                alert('Debe mantener al menos un detalle en la orden.');
            }
        }
    });

    // Reindexar nombres de campos después de eliminar
    function reindexarDetalles() {
        document.querySelectorAll('.detalle-row').forEach(function (row, index) {
            const inputs = row.querySelectorAll('input[name*="Detalles"]');
            inputs.forEach(function (input) {
                const name = input.getAttribute('name');
                if (name) {
                    const newName = name.replace(/Detalles\[\d+\]/, `Detalles[${index}]`);
                    input.setAttribute('name', newName);
                }
            });
        });
        detalleIndex = document.querySelectorAll('.detalle-row').length;
    }

    // Calcular total inicial
    calcularTotal();
});