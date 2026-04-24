// Cierra las notificaciones Bulma (`.notification`) al hacer click en su
// botón `.delete`. Sólo aplica a notificaciones marcadas con el atributo
// `data-dismissible`, para no interferir con modales u otros componentes
// que también usan `.delete` (por ejemplo el header del modal).

const FADE_OUT_MS = 200;

function fadeOutAndRemove(element) {
    element.style.transition = `opacity ${FADE_OUT_MS}ms ease`;
    element.style.opacity = '0';
    setTimeout(() => element.remove(), FADE_OUT_MS);
}

export function enableDismissibleNotifications(root = document) {
    const closeButtons = root.querySelectorAll('.notification[data-dismissible] > .delete');

    closeButtons.forEach((button) => {
        button.addEventListener('click', () => {
            const notification = button.closest('.notification');
            if (notification) {
                fadeOutAndRemove(notification);
            }
        });
    });
}
