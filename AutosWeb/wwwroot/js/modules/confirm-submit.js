// Intercepta el submit de los formularios marcados con `data-confirm` y
// muestra el modal de confirmación antes de dejar que continúen.
//
// El mensaje se obtiene de `dataset.confirm` y siempre se trata como texto
// plano por el modal (ver `confirm-modal.js`).

import { confirmDialog } from './confirm-modal.js';

const DEFAULT_MESSAGE = '¿Está seguro?';

export function enableConfirmSubmit(root = document) {
    const forms = root.querySelectorAll('form[data-confirm]');

    forms.forEach((form) => {
        let confirmed = false;

        form.addEventListener('submit', async (event) => {
            if (confirmed) {
                return;
            }

            event.preventDefault();
            const message = form.dataset.confirm || DEFAULT_MESSAGE;
            const accepted = await confirmDialog(message);

            if (accepted) {
                confirmed = true;
                form.submit();
            }
        });
    });
}
