# Referencia de API: User Management

Este documento proporciona una referencia técnica para la API de User Management. Está destinado a los desarrolladores de Frontend y a futuros agentes de IA que trabajen en el sistema. La API está diseñada en torno a los principios básicos de "Identidad vs. Rol", reputación contextual y bandejas de validación administrativa.

## 1. Autenticación y Ciclo de Vida (`AuthController`)

Maneja el registro de usuarios, el inicio de sesión y la gestión de tokens.

### Registrar Usuario Personal

*   **Endpoint:** `POST /api/Auth/register/personal`
*   **Acceso:** Público
*   **Lógica de Negocio:** Crea un nuevo usuario personal. El usuario se crea con un estado `Activo` y puede iniciar sesión de inmediato. Esto corresponde a una verificación de identidad de "Nivel 1" (cuenta básica).
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "email": "juan.perez@example.com",
      "password": "Password123!",
      "confirmPassword": "Password123!",
      "nombres": "Juan",
      "apellidoPaterno": "Perez",
      "apellidoMaterno": "Gomez",
      "userName": "juan.perez",
      "fechaNacimiento": "1990-05-15T00:00:00",
      "ci": "1234567",
      "nit": "1234567011",
      "seprec": "456789-ABC",
      "pais": "Bolivia",
      "departamento": "La Paz",
      "direccion": "Av. Arce 2132",
      "celular": "77712345",
      "profesion": "Ingeniero de Software",
      "linkedInUrl": "https://linkedin.com/in/juanperez",
      "documentos": [
        {
          "tipo": "FotoPerfil",
          "url": "https://firebasestorage.googleapis.com/..."
        }
      ]
    }
    ```

### Registrar Empresa

*   **Endpoint:** `POST /api/Auth/register/company`
*   **Acceso:** Público
*   **Lógica de Negocio:** Crea una nueva cuenta de empresa. La empresa se crea con un estado `Pendiente` y **no puede** iniciar sesión hasta que un administrador valide y apruebe su documentación legal. Los `documentosLegales` deben contener documentos de tipo "General" para que comience el proceso de validación.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "emailEmpresa": "mi.empresa@example.com",
      "password": "Password123!",
      "confirmPassword": "Password123!",
      "razonSocial": "Mi Empresa S.R.L.",
      "tipoEmpresa": "Sociedad de Responsabilidad Limitada",
      "nit": "1020304015",
      "telefonoFijo": "22112233",
      "celularCorporativo": "76543210",
      "representante": {
        "nombres": "Ana",
        "apellidoPaterno": "Valdez",
        "ci": "7654321",
        "fechaNacimiento": "1985-10-20T00:00:00",
        "direccionDomicilio": "Calle 1, No. 100",
        "emailPersonal": "ana.valdez@personal.com",
        "numerosContacto": ["71122334"]
      },
      "sucursales": [
        {
          "nombre": "Oficina Central",
          "direccion": "Av. Principal 123",
          "departamento": "Cochabamba",
          "pais": "Bolivia",
          "latitud": -17.3937,
          "longitud": -66.1571,
          "telefono": "44556677"
        }
      ],
      "documentosLegales": [
        {
          "tipo": "NIT",
          "url": "https://firebasestorage.googleapis.com/...",
          "modulo": "General"
        },
        {
          "tipo": "Fundempresa",
          "url": "https://firebasestorage.googleapis.com/...",
          "modulo": "General"
        }
      ],
      "modulosSolicitados": ["Contabilidad", "Inventario"]
    }
    ```

### Iniciar Sesión

*   **Endpoint:** `POST /api/Auth/login`
*   **Acceso:** Público
*   **Lógica de Negocio:** Autentica a un usuario. Devuelve un token JWT si las credenciales son válidas y la cuenta del usuario está `Activo`. Devolverá un error si el estado del usuario es `Pendiente`, `Rechazado` o `Suspendido`.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "email": "juan.perez@example.com",
      "password": "Password123!",
      "moduloSeleccionado": "Social"
    }
    ```

### Refrescar Token

*   **Endpoint:** `POST /api/Auth/refresh-token`
*   **Acceso:** Público (Requiere un Refresh Token válido)
*   **Lógica de Negocio:** Genera un nuevo token de acceso JWT utilizando un refresh token válido.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    }
    ```

## 2. Gestión de Usuario Personal (`UsersController`)

Endpoints para que los usuarios personales gestionen su identidad profesional y técnica.

### Solicitar Etiqueta Social (Rol)

*   **Endpoint:** `POST /api/Users/tags/request`
*   **Acceso:** Usuarios Personales Autenticados
*   **Lógica de Negocio:** Permite a un usuario personal solicitar una etiqueta o rol social (ej. "Vendedor", "Mecánico"). Esta acción establece la bandera `TieneSolicitudPendiente` del usuario a `true`, agregando la solicitud a la cola de validación del administrador.
    *   **Etiqueta "Vendedor":** Requiere un documento de tipo "FacturaServicioBasico".
    *   **Etiquetas de "Oficio" (ej. "Mecánico"):** Si `esEmpirico` es `true`, requiere fotos del taller/trabajo. Si es `false`, requiere un título o certificado.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "tagNombre": "Mecánico",
      "esEmpirico": true,
      "evidencias": [
        {
          "tipo": "FotoTaller",
          "url": "https://firebasestorage.googleapis.com/..."
        }
      ]
    }
    ```

### Solicitar Módulo Personal

*   **Endpoint:** `POST /api/Users/modules/request`
*   **Acceso:** Usuarios Personales Autenticados
*   **Lógica de Negocio:** Permite a un usuario personal solicitar acceso a un módulo de software especializado. Esto es para futuro software de uso personal y es independiente de los módulos de empresa. Esta acción también establece la bandera `TieneSolicitudPendiente` a `true`.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "nombreModulo": "AnalisisFinancieroPersonal",
      "documentosEvidencia": [
        {
          "tipo": "CertificadoInversion",
          "url": "https://firebasestorage.googleapis.com/..."
        }
      ]
    }
    ```

## 3. Gestión Empresarial (`CompanyManagementController`)

Endpoints para que las empresas gestionen su estructura, empleados y perfiles comerciales.

### Crear Empleado

*   **Endpoint:** `POST /api/CompanyManagement/employees`
*   **Acceso:** Usuarios de Empresa Autenticados (Cuenta principal)
*   **Lógica de Negocio:** Crea un nuevo empleado (subcuenta) bajo la empresa. El diccionario `permisos` es crucial para definir a qué puede acceder el empleado. La clave es el nombre del módulo (ej. "Contabilidad"), y el valor (`ModuleAccessDto`) especifica si tienen acceso y a qué funcionalidades o recursos específicos.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "email": "empleado1@example.com",
      "passwordTemporal": "TempPass123!",
      "nombres": "Carlos",
      "apellidoPaterno": "Solis",
      "ci": "8889990",
      "fechaNacimiento": "1995-02-10T00:00:00",
      "direccionEscrita": "Calle Falsa 123",
      "celular": "78899001",
      "areaTrabajo": "Ventas",
      "esSuperAdmin": false,
      "permisos": {
        "Social": {
          "acceso": true,
          "funcionalidades": ["Publicar", "Moderar"],
          "recursosIds": ["perfil_social_id_1"]
        },
        "Inventario": {
          "acceso": true,
          "funcionalidades": [],
          "recursosIds": []
        }
      }
    }
    ```
> **⚠️ Regla de Arquitectura (Delegación Total):**
> Para módulos de negocio externos (como `Construccion.API`), la responsabilidad de `UserManagement` se limita estrictamente al **Acceso (On/Off)**.
> * `acceso`: `true` (Habilita el botón en el menú).
> * `funcionalidades`: Envíe `[]` (Vacío). Los roles (ej. Ingeniero, Residente) se definen en el microservicio destino.
> * `recursosIds`: Envíe `[]` (Vacío). La asignación de Obras/Proyectos se realiza en el microservicio destino.

### Solicitar Perfil Comercial

*   **Endpoint:** `POST /api/CompanyManagement/profiles/request`
*   **Acceso:** Usuarios de Empresa Autenticados
*   **Lógica de Negocio:** Este es un endpoint clave para que una empresa defina sus "caras" al público. Puede ser usado para solicitar acceso a un módulo de software técnico o para crear un perfil social para una línea de negocio específica (rubro).
    *   **Para Solicitar un Módulo:** Provea el nombre del `moduloAsociado`.
    *   **Para Solicitar una Etiqueta Social:** Deje `moduloAsociado` como `null` y especifique el `rubro`.
*   **Ejemplo de Cuerpo JSON (Solicitud de Módulo):**
    ```json
    {
      "nombreComercial": "Constructora Acme",
      "moduloAsociado": "Construccion",
      "logoUrl": "https://firebasestorage.googleapis.com/...",
      "documentos": [
        {
          "tipo": "CertificadoIngenieria",
          "url": "https://firebasestorage.googleapis.com/..."
        }
      ]
    }
    ```
*   **Ejemplo de Cuerpo JSON (Solicitud de Etiqueta Social):**
    ```json
    {
      "nombreComercial": "Restaurante La Sazón",
      "rubro": "Restaurantes",
      "logoUrl": "https://firebasestorage.googleapis.com/...",
      "documentos": [
        {
          "tipo": "MenuDigital",
          "url": "https://firebasestorage.googleapis.com/..."
        }
      ]
    }
    ```

### Rectificar Identidad Global

*   **Endpoint:** `PUT /api/CompanyManagement/identity/rectify`
*   **Acceso:** Usuarios de Empresa Autenticados (solo si el estado de su cuenta es `Rechazado`)
*   **Lógica de Negocio:** Permite a una empresa cuyo registro inicial fue rechazado (ej. por un NIT ilegible) enviar nuevos documentos legales y reingresar a la cola de validación. Esta acción cambia el estado global del usuario de `Rechazado` a `Pendiente`.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "razonSocial": "Mi Empresa S.R.L. (Corregido)",
      "nit": "1020304015",
      "documentosLegales": [
        {
          "tipo": "NIT",
          "url": "https://firebasestorage.googleapis.com/...(new_url)"
        }
      ]
    }
    ```

## 4. Panel de Administrador (`AdminController`)

Endpoints para que los administradores validen y gestionen las solicitudes de usuarios y empresas. Todos los endpoints `GET` en esta sección están altamente optimizados y solo consultan a usuarios donde `TieneSolicitudPendiente == true`, minimizando las lecturas de la base de datos en Firestore.

### Bandeja 1: Identidades Pendientes

*   **Endpoint:** `GET /api/Admin/identities/pending`
*   **Acceso:** Admins (`AdminSistema`, `SuperAdminGlobal`)
*   **Lógica de Negocio:** Recupera una lista de nuevas empresas que están pendientes de validación legal inicial. Esta es la primera puerta de entrada para cualquier empresa al sistema.

*   **Endpoint:** `PUT /api/Admin/identities/decision`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Aprueba o rechaza el registro inicial de una empresa. La aprobación establece el estado de la empresa a `Activo`, permitiéndoles iniciar sesión.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "userId": "user_id_of_the_company",
      "approve": true,
      "rejectionReason": null
    }
    ```

### Bandeja 2: Módulos de Empresa Pendientes

*   **Endpoint:** `GET /api/Admin/companies/modules/pending`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Recupera una lista de perfiles comerciales de empresas activas que solicitan acceso a un módulo de software técnico.

*   **Endpoint:** `PUT /api/Admin/companies/modules/decision`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Aprueba o rechaza la solicitud de una empresa para un módulo de software. Si se aprueba, el módulo se agrega a la lista `ModulosHabilitados` de la empresa.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "companyUserId": "company_user_id",
      "commercialProfileId": "profile_id_requesting_module",
      "approve": true,
      "rejectionReason": "La documentación no es suficiente."
    }
    ```

### Bandeja 3: Etiquetas Sociales Pendientes

*   **Endpoint:** `GET /api/Admin/tags/pending`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Recupera una lista mixta de solicitudes de etiquetas sociales pendientes de usuarios personales (ej. "Vendedor") y empresas (ej. "Restaurante").

*   **Endpoint:** `PUT /api/Admin/companies/tags/decision`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Aprueba o rechaza la solicitud de una empresa para un perfil de etiqueta social.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "companyUserId": "company_user_id",
      "commercialProfileId": "profile_id_for_the_tag",
      "approve": true,
      "rejectionReason": null
    }
    ```

*   **Endpoint:** `PUT /api/Admin/personal/tags/decision`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Aprueba o rechaza la solicitud de un usuario personal para una etiqueta/rol social.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "personalUserId": "personal_user_id",
      "tagName": "Mecánico",
      "approve": false,
      "rejectionReason": "La evidencia de trabajo no es clara."
    }
    ```

### Bandeja 4: Módulos Personales Pendientes

*   **Endpoint:** `GET /api/Admin/personal/modules/pending`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Recupera una lista de usuarios personales que solicitan acceso a módulos de software personales especializados.

*   **Endpoint:** `PUT /api/Admin/personal/modules/decision`
*   **Acceso:** Admins
*   **Lógica de Negocio:** Aprueba o rechaza la solicitud de un usuario personal para un módulo de software.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "personalUserId": "personal_user_id",
      "moduleName": "AnalisisFinancieroPersonal",
      "approve": true,
      "rejectionReason": null
    }
    ```

## 5. Sistema de Reseñas (`ReviewsController`)

Endpoint para crear reseñas contextuales.

### Crear Reseña

*   **Endpoint:** `POST /api/Reviews`
*   **Acceso:** Usuarios Autenticados
*   **Lógica de Negocio:** Crea una reseña para un contexto específico. Esta es una pieza crítica del sistema de reputación. El `contextoId` es **obligatorio** y debe apuntar a una `Tag` (ej. "Mecánico") o al ID de un `PerfilComercial` de una empresa. Las reseñas **nunca** son contra el usuario directamente, sino contra el rol o perfil bajo el cual están operando. Esto permite una reputación contextual (ej. alguien puede ser un mecánico de 5 estrellas pero un vendedor de 3 estrellas).
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "recipientId": "user_id_of_person_or_company_being_reviewed",
      "contextoId": "Mecánico", // Esto también podría ser un ID de perfil comercial
      "rating": 5,
      "comment": "Excelente servicio, muy profesional y resolvió el problema rápidamente."
    }
    ```

## 6. Mi Perfil (Autogestión)

Endpoints para que un usuario conectado gestione su propio perfil y configuración de seguridad.

### Obtener Mi Perfil

*   **Endpoint:** `GET /api/Users/profile/me`
*   **Acceso:** Autenticado
*   **Descripción (ES):** Obtiene el perfil completo y detallado del usuario actualmente logueado. La respuesta es polimórfica y cambia según el tipo de usuario (Personal, Empresa, etc.).
*   **Descripción (EN):** Gets the full, detailed profile of the currently logged-in user. The response is polymorphic and changes depending on the user type (Personal, Company, etc.).
*   **Ejemplo de Respuesta JSON (Usuario Personal):**
    ```json
    {
      "id": "user_id_123",
      "email": "juan.perez@example.com",
      "tipoUsuario": "Personal",
      "estado": "Activo",
      "fotoPerfilUrl": "https://example.com/photo.jpg",
      "biografia": "Desarrollador de software con 5 años de experiencia.",
      "datosPersonales": {
        "nombreCompleto": "Juan Perez Gomez",
        "ci": "XXXX567",
        "celular": "77712345",
        "direccion": "Av. Arce 2132"
      },
      "datosEmpresa": null
    }
    ```

### Actualizar Mi Perfil

*   **Endpoint:** `PUT /api/Users/profile/me`
*   **Acceso:** Autenticado
*   **Descripción (ES):** Actualiza datos no sensibles del perfil del usuario, como su foto, biografía o información de contacto. Permite actualizaciones parciales.
*   **Descripción (EN):** Updates non-sensitive user profile data, such as photo, bio, or contact information. Allows for partial updates.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "biografia": "Desarrollador de software y entusiasta de la tecnología.",
      "celular": "77754321"
    }
    ```

### Cambiar Contraseña

*   **Endpoint:** `POST /api/Auth/change-password`
*   **Acceso:** Autenticado
*   **Descripción (ES):** Permite a un usuario logueado cambiar su propia contraseña. Requiere la contraseña actual para verificación.
*   **Descripción (EN):** Allows a logged-in user to change their own password. Requires the current password for verification.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "currentPassword": "Password123!",
      "newPassword": "NewSecurePassword456!",
      "confirmNewPassword": "NewSecurePassword456!"
    }
    ```

## 7. Recursos Humanos (Gestión de Empresa)

Endpoints para que un administrador de empresa gestione a sus empleados (sub-cuentas).

### Listar Empleados

*   **Endpoint:** `GET /api/CompanyManagement/employees`
*   **Acceso:** Admins de Empresa Autenticados
*   **Descripción (ES):** Obtiene una lista resumen de todos los empleados (sub-cuentas) que pertenecen a la empresa.
*   **Descripción (EN):** Gets a summary list of all employees (sub-accounts) belonging to the company.
*   **Ejemplo de Respuesta JSON:**
    ```json
    [
      {
        "id": "employee_id_1",
        "nombreCompleto": "Carlos Solis",
        "email": "empleado1@example.com",
        "cargo": "Ventas",
        "esSuperAdmin": false,
        "estado": "Activo",
        "fechaRegistro": "2023-10-01T10:00:00Z"
      }
    ]
    ```

### Obtener Detalle de Empleado

*   **Endpoint:** `GET /api/CompanyManagement/employees/{id}`
*   **Acceso:** Admins de Empresa Autenticados
*   **Descripción (ES):** Obtiene la vista detallada de un empleado específico, incluyendo sus permisos.
*   **Descripción (EN):** Gets the detailed view of a specific employee, including their permissions.
*   **Ejemplo de Respuesta JSON:**
    ```json
    {
      "id": "employee_id_1",
      "nombreCompleto": "Carlos Solis",
      "email": "empleado1@example.com",
      "cargo": "Ventas",
      // ...otros campos de resumen...
      "ci": "8889990",
      "celular": "78899001",
      "permisos": {
        "Social": {
          "acceso": true,
          "funcionalidades": ["Publicar", "Moderar"],
          "recursosIds": ["perfil_social_id_1"]
        }
      }
    }
    ```

### Actualizar Permisos de Empleado

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/permissions`
*   **Acceso:** Admins de Empresa Autenticados
*   **Descripción (ES):** Actualiza el área de trabajo y la matriz de permisos de un empleado.
*   **Descripción (EN):** Updates an employee's work area and permissions matrix.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "areaTrabajo": "Líder de Ventas",
      "esSuperAdmin": true,
      "permisos": {
        "Social": {
          "acceso": true,
          "funcionalidades": ["Publicar", "Moderar", "VerReportes"],
          "recursosIds": ["perfil_social_id_1", "perfil_social_id_2"]
        }
      }
    }
    ```

### Actualizar Estado de Empleado

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/status`
*   **Acceso:** Admins de Empresa Autenticados
*   **Descripción (ES):** Cambia el estado de un empleado (ej. `Activo`, `Suspendido`). Útil para despidos.
*   **Descripción (EN):** Changes an employee's status (e.g., `Activo`, `Suspendido`). Useful for terminations.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "nuevoEstado": "Suspendido"
    }
    ```

### Resetear Contraseña de Empleado

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/reset-password`
*   **Acceso:** Admins de Empresa Autenticados
*   **Descripción (ES):** Permite a un administrador de empresa resetear manualmente la contraseña de un empleado.
*   **Descripción (EN):** Allows a company administrator to manually reset an employee's password.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "newPassword": "TemporaryPasswordForEmployee123!"
    }
    ```

### Actualizar Perfil de Empleado

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/profile`
*   **Acceso:** Admins de Empresa Autenticados
*   **Descripción (ES):** Permite a un administrador corregir datos personales de un empleado (ej. errores de tipeo en el CI o Nombre).
*   **Descripción (EN):** Allows an administrator to correct an employee's personal data (e.g., typos in ID or Name).
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "nombres": "Carlos Alberto",
      "ci": "8889991"
    }
    ```

## 8. Descubrimiento y Reseñas (Público)

Endpoints de acceso público para encontrar usuarios y ver su reputación.

### Búsqueda Pública

*   **Endpoint:** `GET /api/Users/public/search`
*   **Acceso:** Público (`AllowAnonymous`)
*   **Descripción (ES):** Buscador público de perfiles. Acepta un texto de búsqueda (`q`) y una ciudad opcional.
*   **Descripción (EN):** Public profile search. Accepts a query text (`q`) and an optional city.
*   **Ejemplo de URL:** `/api/Users/public/search?q=Software&ciudad=Cochabamba`

### Obtener Perfil de Usuario Público

*   **Endpoint:** `GET /api/Users/public/{userId}`
*   **Acceso:** Público (`AllowAnonymous`)
*   **Descripción (ES):** Obtiene la "tarjeta de presentación" pública de un usuario, ocultando datos sensibles.
*   **Descripción (EN):** Gets the public "business card" of a user, hiding sensitive data.
*   **Ejemplo de Respuesta JSON:**
    ```json
    {
      "id": "user_id_123",
      "tipoUsuario": "Personal",
      "nombreMostrar": "Juan Perez",
      "fotoUrl": "https://example.com/photo.jpg",
      "biografia": "Desarrollador de software...",
      "ciudad": "La Paz",
      "esVerificado": true,
      "contacto": {
        "direccion": "Av. Arce 2132",
        "telefono": "77712345"
      },
      "etiquetas": [
        {
          "nombre": "Ingeniero de Software",
          "estrellas": 4.8,
          "totalResenas": 25
        }
      ]
    }
    ```

### Obtener Perfiles de Empresa Públicos

*   **Endpoint:** `GET /api/CompanyManagement/profiles/{companyId}/public`
*   **Acceso:** Público (`AllowAnonymous`)
*   **Descripción (ES):** Lista los perfiles comerciales activos de una empresa específica.
*   **Descripción (EN):** Lists the active commercial profiles for a specific company.

### Listar Reseñas

*   **Endpoint:** `GET /api/Reviews`
*   **Acceso:** Público (`AllowAnonymous`)
*   **Descripción (ES):** Obtiene las reseñas para un `recipientId` y un `contextoId` (Tag o Perfil Comercial).
*   **Descripción (EN):** Gets the reviews for a `recipientId` and `contextoId` (Tag or Commercial Profile).
*   **Ejemplo de URL:** `/api/Reviews?recipientId=user_id_123&contextoId=Ingeniero%20de%20Software`
*   **Ejemplo de Respuesta JSON:**
    ```json
    [
      {
        "autorNombre": "Carlos Solis",
        "rating": 5,
        "comentario": "Excelente trabajo, muy recomendable.",
        "fecha": "2023-11-20T15:30:00Z"
      }
    ]
    ```

## 9. Soporte y Moderación (SuperAdmin)

Endpoints de alto privilegio para administradores del sistema.

### Búsqueda de Usuario Admin

*   **Endpoint:** `GET /api/Admin/users/search`
*   **Acceso:** SuperAdmins (`AdminSistema`, `SuperAdminGlobal`)
*   **Descripción (ES):** Búsqueda en cascada por un término exacto: Email > Username > CI > NIT.
*   **Descripción (EN):** Cascading search for an exact term: Email > Username > CI > NIT.
*   **Ejemplo de URL:** `/api/Admin/users/search?term=juan.perez@example.com`

### Obtener Detalle Completo de Usuario

*   **Endpoint:** `GET /api/Admin/users/{id}`
*   **Acceso:** SuperAdmins
*   **Descripción (ES):** Obtiene el perfil completo de un usuario sin NINGÚN tipo de enmascaramiento o filtro de privacidad.
*   **Descripción (EN):** Gets the complete user profile with NO privacy masking or filtering.
*   **Ejemplo de Respuesta JSON:**
    ```json
    {
      "id": "user_id_123",
      "email": "juan.perez@example.com",
      // ... todos los campos ...
      "datosPersonales": {
        "nombres": "Juan",
        "apellidoPaterno": "Perez",
        "ci": "1234567", // Sin máscara
        // ... todos los campos ...
      }
    }
    ```

### Cambiar Estado de Usuario Admin

*   **Endpoint:** `PUT /api/Admin/users/{id}/status`
*   **Acceso:** SuperAdmins
*   **Descripción (ES):** Permite a un admin cambiar forzosamente el estado de cualquier usuario (ej. `Activo`, `Suspendido`, `Eliminado`).
*   **Descripción (EN):** Allows an admin to forcibly change any user's status (e.g., `Activo`, `Suspendido`, `Eliminado`).
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "nuevoEstado": "Suspendido",
      "motivo": "Violación de términos de servicio."
    }
    ```

### Resetear Contraseña Admin

*   **Endpoint:** `PUT /api/Admin/users/{id}/reset-password`
*   **Acceso:** SuperAdmins
*   **Descripción (ES):** Permite a un admin resetear la contraseña de cualquier usuario sin necesidad de la contraseña actual.
*   **Descripción (EN):** Allows an admin to reset any user's password without needing the current password.
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "newPassword": "NewPasswordSetBySupport123!"
    }
    ```

## 10. Mi Perfil (Autogestión)

Endpoints para que un usuario conectado gestione su propio perfil y configuración de seguridad.

### `GET /api/Users/profile/me`
*   **Desc:** Obtiene el perfil completo del usuario logueado. Respuesta polimórfica (Personal/Empresa/Empleado).
*   **Desc (EN):** Retrieves the complete profile of the logged-in user. Polymorphic response (Personal/Company/Employee).

    **Ejemplo de Respuesta JSON (Usuario Personal):**
    ```json
    {
      "id": "user_id_123",
      "email": "user.email@example.com",
      "userType": "Personal",
      "status": "Active",
      "profilePictureUrl": "https://example.com/profile/picture.jpg",
      "bio": "Ingeniero de software experimentado con pasión por la innovación.",
      "personalData": {
        "fullName": "John Doe",
        "ci": "1234567",
        "phoneNumber": "77788990",
        "address": "Av. Principal #123"
      }
    }
    ```

    **Ejemplo de Respuesta JSON (Usuario de Empresa):**
    ```json
    {
      "id": "company_id_456",
      "email": "company.email@example.com",
      "userType": "Company",
      "status": "Active",
      "companyName": "Tech Solutions Inc.",
      "legalName": "Tech Solutions S.R.L.",
      "nit": "987654321",
      "corporatePhoneNumber": "21234567",
      "branches": [
        {
          "name": "Sede Principal",
          "address": "Calle Falsa #456",
          "city": "La Paz"
        }
      ]
    }
    ```

### `PUT /api/Users/profile/me`
*   **Desc:** Actualiza datos no sensibles (Foto, Bio, Celular, Dirección).
*   **Desc (EN):** Updates non-sensitive data (Photo, Bio, Phone, Address).

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "profilePictureUrl": "https://example.com/new_picture.jpg",
      "bio": "Biografía actualizada: Siempre aprendiendo y creciendo.",
      "phoneNumber": "77711223",
      "address": "Av. Secundaria #456"
    }
    ```

### `POST /api/Auth/change-password`
*   **Desc:** Cambia la contraseña. Requiere `CurrentPassword` y `NewPassword`.
*   **Desc (EN):** Changes the password. Requires `CurrentPassword` and `NewPassword`.

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "currentPassword": "OldSecurePassword1!",
      "newPassword": "NewSecurePassword2@",
      "confirmNewPassword": "NewSecurePassword2@"
    }
    ```

## 11. Recursos Humanos (Gestión de Empresa)

### `GET /api/CompanyManagement/employees`
*   **Desc:** Lista todos los empleados (sub-cuentas) de la empresa.
*   **Desc (EN):** Lists all employees (sub-accounts) of the company.

    **Ejemplo de Respuesta JSON:**
    ```json
    [
      {
        "id": "employee_id_1",
        "fullName": "Jane Smith",
        "email": "jane.smith@company.com",
        "workArea": "Marketing",
        "status": "Active"
      },
      {
        "id": "employee_id_2",
        "fullName": "Peter Jones",
        "email": "peter.jones@company.com",
        "workArea": "Ventas",
        "status": "Active"
      }
    ]
    ```

### `GET /api/CompanyManagement/employees/{id}`
*   **Desc:** Ver detalle y permisos de un empleado específico.
*   **Desc (EN):** View details and permissions of a specific employee.

    **Ejemplo de Respuesta JSON:**
    ```json
    {
      "id": "employee_id_1",
      "fullName": "Jane Smith",
      "email": "jane.smith@company.com",
      "ci": "11223344",
      "workArea": "Marketing",
      "isSuperAdmin": false,
      "permissions": {
        "Social": {
          "access": true,
          "functionalities": ["CreatePost", "ManageComments"]
        },
        "Analytics": {
          "access": true,
          "functionalities": ["ViewReports"]
        }
      }
    }
    ```

### `PUT /api/CompanyManagement/employees/{id}/permissions`
*   **Desc:** Reasigna área de trabajo y permisos de acceso.
*   **Desc (EN):** Reassigns work area and access permissions.

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "workArea": "Senior Marketing",
      "isSuperAdmin": true,
      "permissions": {
        "Social": {
          "access": true,
          "functionalities": ["CreatePost", "ManageComments", "ApproveContent"]
        },
        "Analytics": {
          "access": true,
          "functionalities": ["ViewReports", "ExportData"]
        }
      }
    }
    ```

### `PUT /api/CompanyManagement/employees/{id}/status`
*   **Desc:** Suspende (Despido) o Reactiva un empleado.
*   **Desc (EN):** Suspends (Dismissal) or Reactivates an employee.

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "newStatus": "Suspended"
    }
    ```

### `PUT /api/CompanyManagement/employees/{id}/reset-password`
*   **Desc:** (Admin Empresa) Resetea la contraseña de un empleado manualmente.
*   **Desc (EN):** (Company Admin) Manually resets an employee's password.

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "newPassword": "EmployeeNewPass123!"
    }
    ```

### `PUT /api/CompanyManagement/employees/{id}/profile`
*   **Desc:** (Admin Empresa) Corrige datos personales del empleado (CI, Nombre).
*   **Desc (EN):** (Company Admin) Corrects employee's personal data (ID, Name).

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "fullName": "Jane A. Smith",
      "ci": "11223345"
    }
    ```

## 12. Descubrimiento y Reseñas (Público)

### `GET /api/Users/public/search`
*   **Desc:** Buscador público. Params: `q` (texto), `ciudad`. Retorna Personas y Empresas.
*   **Desc (EN):** Public search engine. Params: `q` (text), `city`. Returns People and Companies.

    **Ejemplo de URL:** `/api/Users/public/search?q=developer&ciudad=Cochabamba`

    **Ejemplo de Respuesta JSON:**
    ```json
    [
      {
        "id": "user_id_1",
        "name": "Alice Wonderland",
        "type": "Personal",
        "description": "Desarrolladora de software especializada en aplicaciones web.",
        "profileImageUrl": "https://example.com/profile/alice.jpg"
      },
      {
        "id": "company_id_2",
        "name": "Global Tech Solutions",
        "type": "Company",
        "description": "Empresa líder en tecnología que ofrece servicios de TI.",
        "profileImageUrl": "https://example.com/profile/globaltech.jpg"
      }
    ]
    ```

### `GET /api/Users/public/{userId}`
*   **Desc:** Tarjeta de presentación pública. Oculta datos sensibles según privacidad.
*   **Desc (EN):** Public business card. Hides sensitive data according to privacy settings.

    **Ejemplo de Respuesta JSON:**
    ```json
    {
      "id": "user_id_123",
      "name": "John Doe",
      "userType": "Personal",
      "bio": "Apasionado por los proyectos de código abierto y contribuir a la comunidad.",
      "city": "La Paz",
      "contactInfo": {
        "email": "john.doe.public@example.com"
      },
      "publicTags": [
        {
          "tagName": "Desarrollador Web",
          "rating": 4.9,
          "reviewCount": 50
        }
      ]
    }
    ```

### `GET /api/CompanyManagement/profiles/{companyId}/public`
*   **Desc:** Lista sucursales y perfiles comerciales activos de una empresa.
*   **Desc (EN):** Lists active branches and commercial profiles of a company.

    **Ejemplo de Respuesta JSON:**
    ```json
    [
      {
        "profileId": "commercial_profile_1",
        "profileName": "Servicios de Sucursal Principal",
        "description": "Servicios generales de TI para empresas.",
        "address": "Av. Ayacucho #100",
        "phoneNumber": "22233445",
        "avgRating": 4.7,
        "totalReviews": 120
      },
      {
        "profileId": "commercial_profile_2",
        "profileName": "División de Desarrollo de Software",
        "description": "Soluciones de software a medida.",
        "address": "Calle Comercio #200",
        "phoneNumber": "22255667",
        "avgRating": 4.9,
        "totalReviews": 80
      }
    ]
    ```

### `GET /api/Reviews`
*   **Desc:** Lista reseñas. Params: `recipientId`, `contextId`.
*   **Desc (EN):** Lists reviews. Params: `recipientId`, `contextId`.

    **Ejemplo de URL:** `/api/Reviews?recipientId=user_id_123&contextId=Web%20Developer`

    **Ejemplo de Respuesta JSON:**
    ```json
    [
      {
        "reviewId": "review_id_1",
        "reviewerName": "Cliente A",
        "rating": 5,
        "comment": "Excelente trabajo, entregado a tiempo y superó las expectativas.",
        "date": "2023-11-01T10:00:00Z"
      },
      {
        "reviewId": "review_id_2",
        "reviewerName": "Cliente B",
        "rating": 4,
        "comment": "Buena comunicación, pequeños retrasos pero en general satisfecho.",
        "date": "2023-10-25T14:30:00Z"
      }
    ]
    ```

## 13. Soporte y Moderación (SuperAdmin)

### `GET /api/Admin/users/search`
*   **Desc:** Búsqueda por Email, CI, NIT o Username.
*   **Desc (EN):** Search by Email, ID, NIT, or Username.

    **Ejemplo de URL:** `/api/Admin/users/search?query=admin@example.com`

    **Ejemplo de Respuesta JSON:**
    ```json
    [
      {
        "id": "user_id_123",
        "email": "admin@example.com",
        "username": "adminUser",
        "fullName": "Admin One",
        "userType": "Admin",
        "status": "Active"
      }
    ]
    ```

### `GET /api/Admin/users/{id}`
*   **Desc:** Ver perfil completo SIN censura (incluye docs y datos privados).
*   **Desc (EN):** View full uncensored profile (includes documents and private data).

    **Ejemplo de Respuesta JSON:**
    ```json
    {
      "id": "user_id_123",
      "email": "juan.perez@example.com",
      "username": "juan.perez",
      "userType": "Personal",
      "status": "Active",
      "personalData": {
        "fullName": "Juan Perez Gomez",
        "ci": "1234567",
        "dateOfBirth": "1990-05-15T00:00:00Z",
        "address": "Av. Arce 2132",
        "phoneNumber": "77712345",
        "emergencyContact": "Maria Perez - 77700011"
      },
      "documents": [
        {
          "type": "IdentityCard",
          "url": "https://firebasestorage.googleapis.com/id_card.jpg",
          "status": "Approved"
        }
      ],
      "privateNotes": "El usuario tiene un historial de intentos de inicio de sesión sospechosos."
    }
    ```

### `PUT /api/Admin/users/{id}/status`
*   **Desc:** Banear/Suspender o Reactivar cualquier usuario.
*   **Desc (EN):** Ban/Suspend or Reactivate any user.

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "newStatus": "Banned",
      "reason": "Violaciones repetidas de los términos de servicio."
    }
    ```

### `PUT /api/Admin/users/{id}/reset-password`
*   **Desc:** (Soporte) Reseteo de emergencia de contraseña.
*   **Desc (EN):** (Support) Emergency password reset.

    **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "newPassword": "EmergencyResetPass#123",
      "notifyUser": true
    }
    ```
