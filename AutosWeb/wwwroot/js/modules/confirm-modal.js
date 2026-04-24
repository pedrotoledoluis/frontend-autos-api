// Modal de confirmación accesible basado en el componente `modal` de Bulma.
// Reemplaza el `confirm()` nativo (Security/Code Smell reportado por Sonar:
// `javascript:S1442`). Expone una API asíncrona `confirmDialog(message)`
// que devuelve una promesa con `true` / `false`.
//
// El `message` se inserta con `textContent` (nunca `innerHTML`) para evitar
// inyección de HTML en el diálogo.

const MODAL_ID = 'confirm-modal';
const MESSAGE_ID = 'confirm-modal-message';
const ACCEPT_SELECTOR = '[data-modal-accept]';
const CLOSE_SELECTOR = '[data-modal-close]';
const ACTIVE_CLASS = 'is-active';
const DEFAULT_MESSAGE = '¿Está seguro?';

function getModalElements() {
    const modal = document.getElementById(MODAL_ID);
    if (!modal) {
        return null;
    }

    return {
        modal,
        message: document.getElementById(MESSAGE_ID),
        acceptButton: modal.querySelector(ACCEPT_SELECTOR),
        closeTargets: modal.querySelectorAll(CLOSE_SELECTOR),
    };
}

function openModal(modal) {
    modal.classList.add(ACTIVE_CLASS);
    modal.setAttribute('aria-hidden', 'false');
    document.documentElement.classList.add('is-clipped');
}

function closeModal(modal) {
    modal.classList.remove(ACTIVE_CLASS);
    modal.setAttribute('aria-hidden', 'true');
    document.documentElement.classList.remove('is-clipped');
}

export function confirmDialog(message = DEFAULT_MESSAGE) {
    const elements = getModalElements();
    if (!elements) {
        return Promise.resolve(true);
    }

    const { modal, message: messageEl, acceptButton, closeTargets } = elements;

    return new Promise((resolve) => {
        if (messageEl) {
            messageEl.textContent = message;
        }

        const cleanup = (result) => {
            acceptButton?.removeEventListener('click', onAccept);
            closeTargets.forEach((el) => el.removeEventListener('click', onCancel));
            document.removeEventListener('keydown', onKeydown);
            closeModal(modal);
            resolve(result);
        };

        const onAccept = () => cleanup(true);
        const onCancel = () => cleanup(false);
        const onKeydown = (event) => {
            if (event.key === 'Escape') {
                cleanup(false);
            }
        };

        acceptButton?.addEventListener('click', onAccept);
        closeTargets.forEach((el) => el.addEventListener('click', onCancel));
        document.addEventListener('keydown', onKeydown);

        openModal(modal);
        acceptButton?.focus();
    });
}
