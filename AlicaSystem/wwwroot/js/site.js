// // Alterna mostrar/ocultar cualquier campo de contraseña que tenga
// un botón con la clase "toggle-password" al lado.
// Funciona para todos los campos de contraseña del sitio, no solo login.
document.querySelectorAll('.toggle-password').forEach(function (boton) {
    boton.addEventListener('click', function () {
        // data-target apunta al id del input que este botón controla
        const idCampo = boton.getAttribute('data-target');
        const campo = document.getElementById(idCampo);
        const icono = boton.querySelector('i');

        const estaOculta = campo.type === 'password';
        campo.type = estaOculta ? 'text' : 'password';

        icono.classList.toggle('bi-eye', !estaOculta);
        icono.classList.toggle('bi-eye-slash', estaOculta);
    });
});

// Modal de confirmación reutilizable (reemplaza el confirm() nativo del navegador)
let formPendienteDeConfirmar = null;

function crearModalConfirmacion() {
    if (document.getElementById('modal-confirmacion')) return;

    const modal = document.createElement('div');
    modal.id = 'modal-confirmacion';
    modal.className = 'modal-overlay';
    modal.innerHTML = `
        <div class="modal-card">
            <p id="modal-confirmacion-texto">¿Seguro que quieres continuar?</p>
            <div class="modal-acciones">
                <button type="button" class="btn btn-ghost" id="modal-cancelar">Cancelar</button>
                <button type="button" class="btn btn-danger" id="modal-confirmar">Confirmar</button>
            </div>
        </div>`;
    document.body.appendChild(modal);

    document.getElementById('modal-cancelar').addEventListener('click', function () {
        modal.classList.remove('show');
        formPendienteDeConfirmar = null;
    });

    document.getElementById('modal-confirmar').addEventListener('click', function () {
        modal.classList.remove('show');
        if (formPendienteDeConfirmar) {
            formPendienteDeConfirmar.submit();
        }
    });
}

document.querySelectorAll('.confirm-delete').forEach(function (form) {
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        crearModalConfirmacion();
        const mensaje = form.getAttribute('data-confirm-message') || '¿Seguro que quieres eliminar esto? Esta acción no se puede deshacer.';
        document.getElementById('modal-confirmacion-texto').textContent = mensaje;
        formPendienteDeConfirmar = form;
        document.getElementById('modal-confirmacion').classList.add('show');
    });
});

// Menú de 3 puntos: abre/cierra el menú al lado del botón que se clickeó
document.querySelectorAll('.dropdown-toggle').forEach(function (boton) {
    boton.addEventListener('click', function (e) {
        e.stopPropagation();
        const menu = boton.nextElementSibling;
        document.querySelectorAll('.dropdown-menu').forEach(function (m) {
            if (m !== menu) m.classList.remove('show');
        });
        menu.classList.toggle('show');
    });
});
document.addEventListener('click', function (e) {
    if (!e.target.closest('.dropdown-menu') && !e.target.closest('.dropdown-toggle')) {
        document.querySelectorAll('.dropdown-menu').forEach(m => m.classList.remove('show'));
    }
});
// Boton onpost para renombrar: abre un prompt para ingresar el nuevo nombre y luego envía el form correspondiente
// em el formulario de mi lista
document.querySelectorAll('.rename-toggle').forEach(function (boton) {
    boton.addEventListener('click', function (e) {
        e.stopPropagation();
        const form = document.getElementById(boton.getAttribute('data-target'));
        form.style.display = form.style.display === 'none' ? 'block' : 'none';
    });
});

// contador para limite de texto en el nombre de la lista
document.querySelectorAll('[data-maxlen]').forEach(function (input) {
    const contador = document.getElementById(input.getAttribute('data-maxlen'));
    input.addEventListener('input', function () {
        contador.textContent = input.value.length + '/25';
    });
});
// ---- Registrar devolución (Bibliotecario) ----

async function buscarLector() {
    const matricula = document.getElementById('matriculaLector').value;
    const resp = await fetch('?handler=BuscarLector&matricula=' + encodeURIComponent(matricula));
    const data = await resp.json();
    const texto = document.getElementById('resultadoLector');
    const contenedor = document.getElementById('listaPrestamos');
    contenedor.innerHTML = '';

    if (!data.encontrado) {
        texto.style.color = '#b3261e';
        texto.innerText = 'Usuario no encontrado.';
        return;
    }

    texto.style.color = '';
    texto.innerText = data.nombreCompleto;

    if (data.prestamos.length === 0) {
        contenedor.innerHTML = '<div class="empty-state" style="margin-top:14px;"><h4>Sin préstamos activos</h4><p>Este usuario no tiene libros pendientes por devolver.</p></div>';
        return;
    }

    let html = '<table style="margin-top:14px;"><thead><tr><th>Libro</th><th>Devolución esperada</th><th>Atraso</th><th>Estado del libro</th><th></th></tr></thead><tbody>';
    data.prestamos.forEach(p => {
        const atraso = p.diasAtraso > 0
            ? `<span class="stamp danger">${p.diasAtraso} día(s)</span>`
            : `<span class="stamp ok">A tiempo</span>`;
        html += `<tr>
            <td>${p.titulo}<div class="sub">${p.codigoInterno}</div></td>
            <td>${p.fechaDevEsperada}</td>
            <td>${atraso}</td>
            <td>
                <label style="font-size:11px; display:flex; align-items:center; gap:4px;">
                    <input type="checkbox" id="danado-${p.idPrestamo}"> Dañado
                </label>
                <input type="number" id="monto-${p.idPrestamo}" class="field" placeholder="Monto RD$" style="width:90px; font-size:11px; padding:4px 6px; margin-top:2px;">
            </td>
            <td><button type="button" class="btn btn-primary btn-sm" onclick="registrarDevolucion(${p.idPrestamo})"><i class="bi bi-check-lg"></i> Devolver</button></td>
        </tr>`;
    });
    html += '</tbody></table>';
    contenedor.innerHTML = html;
}

async function registrarDevolucion(idPrestamo) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    const danado = document.getElementById('danado-' + idPrestamo).checked;
    const monto = document.getElementById('monto-' + idPrestamo).value || 0;

    const body = new URLSearchParams();
    body.append('idPrestamo', idPrestamo);
    body.append('libroDanado', danado);
    body.append('montoDano', monto);
    body.append('__RequestVerificationToken', token);

    const resp = await fetch('?handler=RegistrarConEstado', { method: 'POST', body: body });
    const data = await resp.json();

    const resultado = document.getElementById('resultadoFinal');
    resultado.style.color = data.exito ? '' : '#b3261e';
    resultado.innerText = data.mensaje;

    if (data.exito) {
        buscarLector();
    }
}