# AutosWeb — Frontend (.NET 10 + Bulma)

Aplicación web **ASP.NET Core MVC** (.NET 10) que consume la API REST `AutosApi` y presenta el CRUD de autos con una UI basada en **Bulma CSS** y **jQuery**.

## Stack

- .NET 10 — ASP.NET Core MVC
- [Bulma CSS 1.x](https://bulma.io) vía CDN 
- jQuery 3.7 vía CDN
- `HttpClient` tipado (`IAutosApiClient`) con `AddHttpClient<T>`
- Configuración de `ApiSettings:BaseUrl` por ambiente

## Requisitos previos

- SDK de [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- El backend `AutosApi` corriendo (ver el repo `backend-autos-api`)

## Cómo se ejecuta localmente

1. Asegurarse de que la **API** esté en ejecución. Por defecto el frontend la busca en `http://localhost:5086` (ver `appsettings.json` → `ApiSettings:BaseUrl`).
2. Ejecutar el frontend:

   ```powershell
   cd AutosWeb
   dotnet run
   ```

3. Abrir el navegador en:
   - HTTP: `http://localhost:5096`
   - HTTPS: `https://localhost:7189`

El layout se construye con Bulma vía CDN (no hay dependencias node/npm).

## Configuración de la URL de la API por ambiente

La URL base de la API es configurable:

| Archivo                          | Uso                            |
|----------------------------------|---------------------------------|
| `appsettings.json`               | Valor por defecto (Production) |
| `appsettings.Development.json`   | Override en desarrollo          |
| Variable de entorno `ApiSettings__BaseUrl` | Override en cualquier ambiente |


## Cómo el frontend consume la API

El consumo está **desacoplado** en la interfaz `IAutosApiClient` (`Services/IAutosApiClient.cs`) y su implementación `AutosApiClient` que usa `HttpClient` inyectado vía `AddHttpClient<IAutosApiClient, AutosApiClient>`. La controladora MVC (`Controllers/AutosController.cs`) nunca toca `HttpClient` directamente.

### Endpoints que consume

| Acción MVC       | Método API | Endpoint                  |
|------------------|------------|----------------------------|
| `Index`          | GET        | `/api/autos`              |
| `Details(id)`    | GET        | `/api/autos/{id}`         |
| `Create (POST)`  | POST       | `/api/autos`              |
| `Edit (POST)`    | PUT        | `/api/autos/{id}`         |
| `Delete (POST)`  | DELETE     | `/api/autos/{id}`         |

Contrato JSON de entrada/salida:

```json
{
  "id": 1,
  "marca": "Toyota",
  "modelo": "Corolla",
  "anio": 2022,
  "tipoAuto": "Sedan",
  "cantidadAsientos": 5,
  "color": "Blanco",
  "precio": 18500.00,
  "activo": true,
  "fechaCreacion": "2026-04-16T12:00:00Z"
}
```

## Buenas prácticas aplicadas

- Consumo desacoplado de la API (interfaz + `HttpClient` tipado).
- Validaciones con Data Annotations en el `AutoViewModel` + `asp-validation-for` en las vistas.
- Manejo de errores con `try/catch` en el controller y página de error Bulma.
- Notificaciones (`TempData`) con componentes Bulma.
- AntiForgeryToken en todos los formularios mutativos (POST/PUT/DELETE).
- Accesibilidad básica: `lang="es"`, `aria-label`, etiquetas asociadas a inputs.
- Catálogo de `TiposAuto` centralizado en `Models/TiposAuto.cs` (sin duplicación en vistas).

## Organización del JavaScript

JQuery queda únicamente como dependencia de `jquery-validation-unobtrusive`, que es requisito de ASP.NET MVC.

```
wwwroot/js/
├── site.js                     ← entrypoint (ES module)
└── modules/
    ├── notifications.js        ← cerrar notificaciones Bulma
    ├── confirm-modal.js        ← diálogo modal accesible (reemplaza confirm())
    └── confirm-submit.js       ← intercepta form[data-confirm] → modal
```

- `site.js` se carga con `<script type="module">` y sólo orquesta los módulos.
- Cada módulo tiene una responsabilidad única y expone una función pura (`enableDismissibleNotifications`, `confirmDialog`, `enableConfirmSubmit`).
- El diálogo de confirmación usa el componente `modal` de Bulma (`#confirm-modal` en `_Layout.cshtml`) en vez del `confirm()` nativo (cierra el code smell `javascript:S1442`).

## Seguridad (Hotspots Sonar cubiertos)

### Subresource Integrity (SRI)

Todos los recursos cargados desde CDN llevan `integrity` + `crossorigin="anonymous"` + `referrerpolicy="no-referrer"`:

| Recurso | SRI |
|---------|-----|
| Bulma `1.0.2` | `sha384-tl5h4XuWmVzPeVWU0x8bx0j/5iMwCBduLEgZ+2lH4Wjda+4+q3mpCww74dgAB3OX` |
| jQuery `3.7.1` | `sha256-/JqT3SQfawRcv/BIHPThkBvs0OEvtFFmqPF/lYI/Cxo=` |
| jquery-validation `1.19.5` | `sha384-aEDtD4n2FLrMdE9psop0SHdNyy/W9cBjH22rSRp+3wPHd62Y32uijc0H2eLmgaSn` |
| jquery-validation-unobtrusive `4.0.0` | `sha384-DU2a51mTHKDhpXhTyJQ++hP8L9L8Gc48TlvbzBmUof71V7kNVs4ELmaVJKPxcAGn` |

### Security Headers

Se agregó un middleware propio `Infrastructure/Security/SecurityHeadersMiddleware.cs` que inyecta en cada respuesta:

- `Content-Security-Policy` (sin `unsafe-inline` ni `unsafe-eval`; sólo permite los CDN usados con SRI).
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Referrer-Policy: no-referrer`
- `Permissions-Policy` (deshabilita cámara, micrófono, geolocalización, pagos).
- `Cross-Origin-Opener-Policy: same-origin` + `Cross-Origin-Resource-Policy: same-origin`.

`HSTS` continúa activado sólo en Production.

### Cookies endurecidas

- **AntiForgery cookie** con prefijo `__Host-`, `HttpOnly`, `Secure=Always`, `SameSite=Strict` y header `X-XSRF-TOKEN`.
- **CookiePolicy** global en `Strict` para cualquier cookie que emita la app.

## Unit tests

El proyecto `AutosWeb.Tests/` (xUnit + FluentAssertions) cubre la pieza más crítica del frontend: la comunicación HTTP con el backend encapsulada en `AutosApiClient`.

```
AutosWeb.Tests/
├── Fakes/
│   └── FakeHttpMessageHandler.cs   ← HttpMessageHandler inyectable para mockear respuestas
└── Services/
    └── AutosApiClientTests.cs      ← 2 tests sobre GetByIdAsync
```

### Casos cubiertos

| Test | Por qué es crítico |
|------|---------------------|
| `GetByIdAsync_devuelve_null_cuando_la_api_responde_404` | El controller MVC depende de que un 404 se traduzca a `null` (no a excepción) para devolver `NotFound()` en la UI. |
| `GetByIdAsync_deserializa_correctamente_cuando_la_api_responde_200` | Verifica el contrato JSON con la API: si el backend renombra un campo, el test falla antes de llegar a producción. |

### Cómo ejecutarlos

```powershell
dotnet test AutosWeb.Tests/AutosWeb.Tests.csproj
```

Los tests no dependen de una API en ejecución ni de una base de datos — usan un `HttpMessageHandler` falso que simula la respuesta de la API en memoria.

