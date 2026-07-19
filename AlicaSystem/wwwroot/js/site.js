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