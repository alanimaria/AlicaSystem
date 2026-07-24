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
