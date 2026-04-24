// Entrypoint del cliente. Sólo orquesta los módulos vanilla que componen
// la lógica de UI. No depende de jQuery — jQuery queda únicamente como
// dependencia de `jquery-validation-unobtrusive` (requisito de ASP.NET MVC).

import { enableDismissibleNotifications } from './modules/notifications.js';
import { enableConfirmSubmit } from './modules/confirm-submit.js';

function bootstrap() {
    enableDismissibleNotifications();
    enableConfirmSubmit();
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', bootstrap, { once: true });
} else {
    bootstrap();
}
