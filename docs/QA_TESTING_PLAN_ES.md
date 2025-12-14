# Plan de Pruebas de QA

Este documento describe escenarios de prueba de extremo a extremo para validar la lógica de negocio central de la API de Gestión de Usuarios. Sirve como guía para los ingenieros de QA y para el desarrollo de pruebas automatizadas.

## Instrucciones

Cada escenario describe un flujo de usuario completo, detallando los pasos con acciones/endpoints específicos, datos clave a utilizar y los resultados esperados, incluidos los códigos de respuesta HTTP y los efectos en la base de datos.

---

## Escenarios

### 🧪 Escenario 1: El Ciclo de Vida del "Mecánico" (Usuario Personal)

**Objetivo:** Probar el registro de usuarios, la solicitud de roles, la validación administrativa y la gestión de la reputación para un usuario personal.

| Paso | Acción/Endpoint                | Datos Clave                                         | Resultado Esperado (Código HTTP + Efecto en DB)                                                                  |
| :--- | :----------------------------- | :-------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------- |
| 1    | `POST /api/Auth/register/personal` | `RegisterPersonalDto` válido para un nuevo usuario "Mecánico". | `200 OK`. Usuario creado en la DB con estado `Activo`.                                                          |
| 2    | `POST /api/Auth/login`         | Correo electrónico y contraseña del usuario.       | `200 OK`. Devuelve el token de autenticación.                                                                   |
| 3    | `POST /api/Users/tags/request`   | `RequestTagDto` para "Mecánico" con `evidencias` relevantes (p. ej., fotos). | `200 OK`. El indicador `TieneSolicitudPendiente` del usuario se establece en `true` en la DB. Etiqueta "Mecánico" agregada con estado `Pendiente`. |
| 4    | `GET /api/Admin/tags/pending`    | Token de autenticación del administrador.           | `200 OK`. El cuerpo de la respuesta contiene la solicitud pendiente de la etiqueta "Mecánico" del usuario recién registrado. |
| 5    | `PUT /api/Admin/personal/tags/decision` | `PersonalTagDecisionDto` para aprobar la etiqueta "Mecánico". | `200 OK`. El estado de la etiqueta "Mecánico" cambia a `Activo`. El indicador `TieneSolicitudPendiente` del usuario permanece `true` (si existen otras solicitudes pendientes) o `false` (si no hay otras solicitudes). |
| 6    | `POST /api/Reviews`            | `CreateReviewDto` para el contexto "Mecánico" (usuario objetivo, `contextoId`: "Mecánico", calificación, comentario). | `200 OK`. Nueva reseña agregada a la DB, asociada con la etiqueta "Mecánico". Calificación promedio de la etiqueta "Mecánico" actualizada. |
| 7    | `POST /api/Reviews`            | El mismo `CreateReviewDto` que el Paso 6, pero con `authorId` = `recipientId`. | `400 Bad Request`. Error que indica que un usuario no puede reseñarse a sí mismo en el mismo contexto.         |

### 🧪 Escenario 2: La Empresa que Rectifica (Flujo Complejo)

**Objetivo:** Probar el registro de la empresa, el bloqueo de inicio de sesión, el rechazo y los procesos de rectificación.

| Paso | Acción/Endpoint                  | Datos Clave                                         | Resultado Esperado (Código HTTP + Efecto en DB)                                                                  |
| :--- | :------------------------------- | :-------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------- |
| 1    | `POST /api/Auth/register/company` | `RegisterCompanyDto` válido.                       | `200 OK`. Empresa creada en la DB con estado `Pendiente`. `TieneSolicitudPendiente` establecido en `true`.      |
| 2    | `POST /api/Auth/login`           | Correo electrónico y contraseña de la empresa.     | `401 Unauthorized`. Mensaje de error que indica que la cuenta está pendiente de aprobación.                     |
| 3    | `GET /api/Admin/identities/pending` | Token de autenticación del administrador.           | `200 OK`. El cuerpo de la respuesta contiene la empresa recién registrada.                                       |
| 4    | `PUT /api/Admin/identities/decision` | `IdentityDecisionDto` para **rechazar** la empresa (p. ej., `approve: false`, `rejectionReason: "NIT ilegible"`). | `200 OK`. El estado de la empresa cambia a `Rechazado` en la DB.                                                |
| 5    | `PUT /api/CompanyManagement/identity/rectify` | `RectifyIdentityDto` con `documentosLegales` actualizados (p. ej., un escaneo de NIT más claro). | `200 OK`. El estado de la empresa cambia de `Rechazado` a `Pendiente` en la DB.                                 |
| 6    | `PUT /api/Admin/identities/decision` | `IdentityDecisionDto` para **aprobar** la empresa (`approve: true`). | `200 OK`. El estado de la empresa cambia a `Activo` en la DB. El indicador `TieneSolicitudPendiente` permanece `true` (si existen otras solicitudes pendientes) o `false` (si no hay otras solicitudes). |
| 7    | `POST /api/Auth/login`           | Correo electrónico y contraseña de la empresa.     | `200 OK`. Devuelve el token de autenticación.                                                                   |

### 🧪 Escenario 3: Gestión de Módulos y Perfiles (Empresa)

**Objetivo:** Probar la creación de diferentes "caras" comerciales para una empresa.

| Paso | Acción/Endpoint                      | Datos Clave                                         | Resultado Esperado (Código HTTP + Efecto en DB)                                                                  |
| :--- | :----------------------------------- | :-------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------- |
| 1    | `POST /api/CompanyManagement/profiles/request` | `RequestCommercialProfileDto` para "Restaurante" (etiqueta social, `moduloAsociado: null`, `rubro: "Restaurantes"`). | `200 OK`. Nuevo `PerfilComercial` creado con tipo `TagSocial` y estado `Pendiente`. El indicador `TieneSolicitudPendiente` de la empresa se establece en `true`. |
| 2    | `POST /api/CompanyManagement/profiles/request` | `RequestCommercialProfileDto` para "Construccion" (módulo, `moduloAsociado: "Construccion"`). | `200 OK`. Nuevo `PerfilComercial` creado con tipo `Modulo` y estado `Pendiente`. El indicador `TieneSolicitudPendiente` de la empresa permanece `true`. |
| 3    | `GET /api/Admin/companies/tags/pending`    | Token de autenticación del administrador.           | `200 OK`. El cuerpo de la respuesta contiene la etiqueta social "Restaurante" pendiente.                         |
| 4    | `GET /api/Admin/companies/modules/pending` | Token de autenticación del administrador.           | `200 OK`. El cuerpo de la respuesta contiene la solicitud pendiente del módulo "Construccion".                   |

### 🧪 Escenario 4: Seguridad y Roles

**Objetivo:** Probar que un usuario normal no puede realizar acciones administrativas.

| Paso | Acción/Endpoint                          | Datos Clave                      | Resultado Esperado (Código HTTP + Efecto en DB)           |
| :--- | :--------------------------------------- | :---------------------------- | :------------------------------------------------ |
| 1    | `GET /api/Admin/identities/pending`      | Token de autenticación de usuario personal. | `403 Forbidden`. Sin cambios en la DB.            |
| 2    | `POST /api/Auth/register/personal`         | `RegisterPersonalDto` válido para un nuevo usuario personal. | `200 OK`. Usuario creado en la DB con estado `Activo`. |
| 3    | `POST /api/Auth/login`                   | Credenciales del usuario personal recién registrado. | `200 OK`. Devuelve el token de autenticación para el usuario personal. |
| 4    | `GET /api/Admin/identities/pending`      | Token de autenticación de usuario personal del Paso 3. | `403 Forbidden`. Sin cambios en la DB.            |
