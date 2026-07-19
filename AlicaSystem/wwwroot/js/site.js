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
document.querySelectorAll('.confirm-delete').forEach(function (form) {
    form.addEventListener('submit', function (e) {
        if (!confirm('¿Seguro que quieres eliminar esto? Esta acción no se puede deshacer.')) {
            e.preventDefault();
        }
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
document.addEventListener('click', function () {
    document.querySelectorAll('.dropdown-menu').forEach(m => m.classList.remove('show'));
});