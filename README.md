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
- jQuery mínimo (cierre de notificaciones + confirmación de borrado).

