# PLAN DE IMPLEMENTACIÓN: ARQUITECTURA FINAL DE IDENTIDAD Y REPUTACIÓN (IO GAMA)

## 1. Visión General
Transformación del microservicio `UserManagement` para soportar un modelo de "Identidad vs. Roles". Se busca separar la validación legal (quién eres) de la validación operativa (qué haces), permitiendo múltiples roles, reputación contextual y flujos de rectificación ante rechazos.

**Principios Clave Actualizados:**
1.  **Biografía Global:** Tanto Empresas como Personas tienen una descripción pública.
2.  **Rectificación Universal:** Cualquier solicitud (Tag o Módulo) rechazada puede ser reintentada enviando nueva evidencia.
3.  **Separación Admin Total:** No mezclar listas de Personas con Empresas.
4.  **Fotos:** Son cosméticas y editables, no bloquean validación legal.

---

## 2. Modificaciones en el Modelo de Dominio (Entidades)

### A. Entidad Raíz (`User`)
* **Nuevo Campo:** `Biografia` (String). Mover aquí para que sirva tanto a `Personal` como a `Company`.
* **Banderas de Estado:** `TieneSolicitudPendiente` (Para agilizar consultas).

### B. Usuario Personal (`PersonalProfile`)
* **Colección `Tags`:** Lista de roles sociales/oficios (Vendedor, Mecánico).
    * *Campos:* NombreTag, Estado (Pendiente/Activo/Rechazado), EsEmpirico, Evidencias, Calificación, MotivoRechazo.
* **Colección `SolicitudesModulos`:** (NUEVO) Lista de solicitudes para software técnico personal.
    * *Campos:* NombreModulo, Estado, Evidencias, MotivoRechazo.
* **Campos Geográficos:** `UbicacionLaboral` (Lat/Lng), `DireccionVisible`.

### C. Usuario Empresa (`CompanyProfile`)
* **Colección `PerfilesComerciales`:** Lista de "caras" de la empresa.
    * *Campos:* IdPerfil, NombreComercial, Tipo (TagSocial vs ModuloTecnico), ModuloAsociado, Estado, Evidencias, MotivoRechazo, Calificación.

---

## 3. Endpoints y Lógica de Negocio (Usuarios)

### GRUPO A: Gestión de Usuarios Personales

#### 1. Solicitar o Rectificar Tag (Rol)
**Endpoint:** `POST /api/Users/tags/request`
* **Acción:** Solicitar ser "Vendedor", "Mecánico", etc.
* **Lógica de Rectificación:**
    * Si el Tag **no existe**: Lo crea con estado `Pendiente`.
    * Si el Tag **existe y está 'Rechazado'**: Actualiza las evidencias (fotos/docs), borra el motivo de rechazo anterior y cambia el estado a `Pendiente`.
* **Validaciones:**
    * Si es "Vendedor": Exige documento "FacturaServicioBasico".
    * Si es "Oficio" (Mecánico, etc): Valida flag `EsEmpirico`. Si true, pide fotos de trabajo; si false, pide título.

#### 2. Solicitar o Rectificar Módulo Personal (NUEVO)
**Endpoint:** `POST /api/Users/modules/request`
* **Acción:** Solicitar acceso a un módulo de software futuro para personas.
* **Lógica de Rectificación:** Igual que en Tags. Si está rechazado, permite re-enviar documentos para pasar a `Pendiente` nuevamente.

---

### GRUPO B: Gestión de Empresas

#### 3. Configurar/Rectificar Perfil Comercial (Módulos y Tags)
**Endpoint:** `POST /api/CompanyManagement/profiles/request`
* **Acción:** Crear un perfil para Red Social (Tag) o para Software (Módulo).
* **Body:** `{ perfilId (opcional), nombre, tipo, modulo, documentos... }`
* **Lógica de Rectificación:**
    * Si se envía un `perfilId` de un perfil previamente **Rechazado**, el sistema actualiza los documentos y cambia el estado a `Pendiente`.
    * Esto aplica tanto si fue rechazado por ser un Módulo mal documentado o un Tag social mal presentado.

#### 4. Rectificar Identidad Global
**Endpoint:** `PUT /api/CompanyManagement/identity/rectify`
* **Acción:** Si la empresa fue rechazada en el Registro (Nivel 0) por NIT ilegible.
* **Efecto:** Actualiza documentos legales y pasa `User.Estado` a `Pendiente`.

---

## 4. Panel del Administrador (Endpoints Separados)

El Admin requiere bandejas diferenciadas. No consolidar listas.

### CATEGORÍA 1: Identidad Legal (Empresas)
* **Leer:** `GET /api/Admin/companies/identity/pending`
    * Empresas nuevas esperando validación de existencia (NIT).
* **Decidir:** `PUT /api/Admin/companies/identity/decision`
    * Aprueba (Pasa a Activo) o Rechaza (Pasa a Rechazado Global).

### CATEGORÍA 2: Módulos Técnicos (Software)

#### Empresas
* **Leer:** `GET /api/Admin/companies/modules/pending`
    * Perfiles de empresas activas que piden Módulos (ej. Construcción).
* **Decidir:** `PUT /api/Admin/companies/modules/decision`
    * Aprueba: Habilita Software + Perfil Social.
    * Rechaza: Guarda motivo (Permite rectificar).

#### Personales (NUEVO)
* **Leer:** `GET /api/Admin/personal/modules/pending`
    * Usuarios que piden módulos personales.
* **Decidir:** `PUT /api/Admin/personal/modules/decision`
    * Aprueba/Rechaza acceso al software personal.

### CATEGORÍA 3: Tags y Reputación Social (Separado)

#### Empresas (Tags Sociales)
* **Leer:** `GET /api/Admin/companies/tags/pending`
    * Empresas que piden presencia social sin módulo técnico (ej. Restaurantes).
* **Decidir:** `PUT /api/Admin/companies/tags/decision`
    * Aprueba: Habilita visibilidad en Feed + Reseñas.
    * Rechaza: Guarda motivo.

#### Personales (Oficios/Roles)
* **Leer:** `GET /api/Admin/personal/tags/pending`
    * Usuarios pidiendo ser Vendedor, Mecánico, etc.
* **Decidir:** `PUT /api/Admin/personal/tags/decision`
    * Aprueba: Habilita Insignia en perfil + Reseñas Contextuales.
    * Rechaza: Guarda motivo.

---

## 5. Sistema de Reseñas (Contextual)

**Endpoint:** `POST /api/Reviews`
* **Lógica:**
    * Valida que el `TargetId` (Usuario destino) tenga activo el `Contexto` (Tag o Perfil Comercial) sobre el que se opina.
    * Guarda la reseña apuntando al contexto específico.
    * Recalcula el promedio de estrellas de ese Tag/Perfil (no del usuario global).

---

## 6. Reglas de Negocio Adicionales

1.  **Multicuentas Empresa:** Las empresas gestionan N perfiles comerciales bajo una sola cuenta de usuario. Cada perfil tiene su propia reputación.
2.  **Fotos de Perfil:** Se tratan como strings (URLs) editables en el perfil. No pasan por el proceso de validación de `UploadedDocument`.
3.  **Estado "Rechazado":** Nunca bloquea el acceso a la corrección. Un usuario rechazado debe poder loguearse (con acceso limitado) para ver el motivo y usar los endpoints de rectificación (`request`).

