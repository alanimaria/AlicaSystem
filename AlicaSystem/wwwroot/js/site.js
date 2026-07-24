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

// Confirmación antes de eliminar (cualquier form con la clase "confirm-delete")
// Muestra un modal con el estilo del sistema en vez del confirm() nativo del navegador
function mostrarModalConfirmacion(mensaje) {
    return new Promise(function (resolve) {
        const overlay = document.createElement('div');
        overlay.className = 'modal-overlay';
        overlay.innerHTML = `
            <div class="modal-box">
                <p>${mensaje}</p>
                <div class="modal-actions">
                    <button type="button" class="btn btn-ghost" data-accion="cancelar">Cancelar</button>
                    <button type="button" class="btn btn-danger" data-accion="confirmar">Eliminar</button>
                </div>
            </div>
        `;
        document.body.appendChild(overlay);

        overlay.addEventListener('click', function (e) {
            if (e.target.dataset.accion === 'confirmar') {
                document.body.removeChild(overlay);
                resolve(true);
            } else if (e.target.dataset.accion === 'cancelar' || e.target === overlay) {
                document.body.removeChild(overlay);
                resolve(false);
            }
        });
    });
}

document.querySelectorAll('.confirm-delete').forEach(function (form) {
    form.addEventListener('submit', function (e) {
        if (form.dataset.confirmado === 'true') return; // ya confirmado, deja pasar

        e.preventDefault();
        mostrarModalConfirmacion('¿Seguro que quieres eliminar esto? Esta acción no se puede deshacer.')
            .then(function (confirmado) {
                if (confirmado) {
                    form.dataset.confirmado = 'true';
                    form.requestSubmit();
                }
            });
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
