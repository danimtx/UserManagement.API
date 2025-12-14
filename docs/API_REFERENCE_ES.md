# Referencia de API: Gestión de Usuarios

Este documento proporciona una referencia técnica para la API de Gestión de Usuarios. Está destinado a los desarrolladores de Frontend y a futuros agentes de IA que trabajen en el sistema. La API está diseñada en torno a los principios básicos de "Identidad vs. Rol", reputación contextual y bandejas de validación administrativa.

## 1. Autenticación y Ciclo de Vida (`AuthController`)

Maneja el registro de usuarios, el inicio de sesión y la gestión de tokens.

### Registrar Usuario Personal

*   **Endpoint:** `POST /api/Auth/register/personal`
*   **Acceso:** Público
*   **Lógica de Negocio:** Crea un nuevo usuario personal. El usuario se crea con el estado `Activo` y puede iniciar sesión de inmediato. Esto corresponde a una verificación de identidad de "Nivel 1" (cuenta básica).
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
*   **Lógica de Negocio:** Crea una nueva cuenta de empresa. La empresa se crea con el estado `Pendiente` y **no puede** iniciar sesión hasta que un administrador valide y apruebe su documentación legal. Los `documentosLegales` deben contener documentos de tipo "General" para que comience el proceso de validación.
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
*   **Lógica de Negocio:** Autentica a un usuario. Devuelve un token JWT si las credenciales son válidas y la cuenta de usuario está `Activo`. Devolverá un error si el estado del usuario es `Pendiente`, `Rechazado` o `Suspendido`.
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
*   **Lógica de Negocio:** Permite a un usuario personal solicitar una etiqueta o rol social (p. ej., "Vendedor", "Mecánico"). Esta acción activa el indicador `TieneSolicitudPendiente` del usuario a `true`, añadiendo la solicitud a la cola de validación del administrador.
    *   **Etiqueta "Vendedor":** Requiere un documento de tipo "FacturaServicioBasico".
    *   **Etiquetas de "Oficio" (p. ej., "Mecánico"):** Si `esEmpirico` es `true`, requiere fotos del taller/trabajo. Si es `false`, requiere un título o certificado.
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
*   **Lógica de Negocio:** Permite a un usuario personal solicitar acceso a un módulo de software especializado. Esto es para futuro software de uso personal y es independiente de los módulos de empresa. Esta acción también activa el indicador `TieneSolicitudPendiente` a `true`.
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
*   **Lógica de Negocio:** Crea un nuevo empleado (subcuenta) bajo la empresa. El diccionario `permisos` es crucial para definir a qué puede acceder el empleado. La clave es el nombre del módulo (p. ej., "Contabilidad"), y el valor (`ModuleAccessDto`) especifica si tienen acceso y a qué funcionalidades o recursos específicos.
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
          "funcionalidades": ["VerStock", "SolicitarProducto"],
          "recursosIds": []
        }
      }
    }
    ```

### Solicitar Perfil Comercial

*   **Endpoint:** `POST /api/CompanyManagement/profiles/request`
*   **Acceso:** Usuarios de Empresa Autenticados
*   **Lógica de Negocio:** Este es un endpoint clave para que una empresa defina sus "caras" al público. Se puede usar para solicitar acceso a un módulo de software técnico o para crear un perfil social para una línea de negocio específica (rubro).
    *   **Para Solicitar un Módulo:** Proporcione el nombre del `moduloAsociado`.
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
*   **Lógica de Negocio:** Permite a una empresa cuyo registro inicial fue rechazado (p. ej., por un NIT ilegible) enviar nuevos documentos legales y volver a entrar en la cola de validación. Esta acción cambia el estado global del usuario de `Rechazado` a `Pendiente`.
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

Endpoints para que los administradores validen y gestionen las solicitudes de usuarios y empresas. Todos los endpoints `GET` de esta sección están altamente optimizados y solo consultan por usuarios donde `TieneSolicitudPendiente == true`, minimizando las lecturas de la base de datos en Firestore.

### Bandeja 1: Identidades Pendientes

*   **Endpoint:** `GET /api/Admin/identities/pending`
*   **Acceso:** Administradores (`AdminSistema`, `SuperAdminGlobal`)
*   **Lógica de Negocio:** Recupera una lista de nuevas empresas que están pendientes de validación legal inicial. Esta es la primera puerta de entrada para cualquier empresa al sistema.

*   **Endpoint:** `PUT /api/Admin/identities/decision`
*   **Acceso:** Administradores
*   **Lógica de Negocio:** Aprueba o rechaza el registro inicial de una empresa. La aprobación establece el estado de la empresa en `Activo`, permitiéndoles iniciar sesión.
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
*   **Acceso:** Administradores
*   **Lógica de Negocio:** Recupera una lista de perfiles comerciales de empresas activas que solicitan acceso a un módulo de software técnico.

*   **Endpoint:** `PUT /api/Admin/companies/modules/decision`
*   **Acceso:** Administradores
*   **Lógica de Negocio:** Aprueba o rechaza la solicitud de un módulo de software por parte de una empresa. Si se aprueba, el módulo se añade a la lista `ModulosHabilitados` de la empresa.
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
*   **Acceso:** Administradores
*   **Lógica de Negocio:** Recupera una lista mixta de solicitudes de etiquetas sociales pendientes tanto de usuarios personales (p. ej., "Vendedor") como de empresas (p. ej., "Restaurante").

*   **Endpoint:** `PUT /api/Admin/companies/tags/decision`
*   **Acceso:** Administradores
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
*   **Acceso:** Administradores
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
*   **Acceso:** Administradores
*   **Lógica de Negocio:** Recupera una lista de usuarios personales que solicitan acceso a módulos de software personal especializados.

*   **Endpoint:** `PUT /api/Admin/personal/modules/decision`
*   **Acceso:** Administradores
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
*   **Lógica de Negocio:** Crea una reseña para un contexto específico. Esta es una pieza crítica del sistema de reputación. El `contextoId` es **obligatorio** y debe apuntar a una `Etiqueta` específica (p. ej., "Mecánico") o al ID de un `PerfilComercial` de una empresa. Las reseñas **nunca** son contra el usuario directamente, sino contra el rol o perfil bajo el cual están operando. Esto permite una reputación contextual (p. ej., alguien puede ser un mecánico de 5 estrellas pero un vendedor de 3 estrellas).
*   **Ejemplo de Cuerpo JSON:**
    ```json
    {
      "recipientId": "user_id_of_person_or_company_being_reviewed",
      "contextoId": "Mecánico", // Esto también podría ser un ID de perfil comercial
      "rating": 5,
      "comment": "Excelente servicio, muy profesional y resolvió el problema rápidamente."
    }
    ```

