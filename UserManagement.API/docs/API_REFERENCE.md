# API Reference: User Management

This document provides a technical reference for the User Management API. It is intended for Frontend developers and future AI agents working on the system. The API is designed around the core principles of "Identity vs. Role", contextual reputation, and administrative validation trays.

## 1. Authentication and Lifecycle (`AuthController`)

Handles user registration, login, and token management.

### Register Personal User

*   **Endpoint:** `POST /api/Auth/register/personal`
*   **Access:** Public
*   **Business Logic:** Creates a new personal user. The user is created with an `Activo` status and can log in immediately. This corresponds to a "Level 1" identity verification (basic account).
*   **JSON Body Example:**
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

### Register Company

*   **Endpoint:** `POST /api/Auth/register/company`
*   **Access:** Public
*   **Business Logic:** Creates a new company account. The company is created with a `Pendiente` status and **cannot** log in until an administrator validates and approves its legal documentation. The `documentosLegales` must contain documents of type "General" for the validation process to begin.
*   **JSON Body Example:**
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

### Login

*   **Endpoint:** `POST /api/Auth/login`
*   **Access:** Public
*   **Business Logic:** Authenticates a user. It returns a JWT token if credentials are valid and the user account is `Activo`. It will return an error if the user's status is `Pendiente`, `Rechazado`, or `Suspendido`.
*   **JSON Body Example:**
    ```json
    {
      "email": "juan.perez@example.com",
      "password": "Password123!",
      "moduloSeleccionado": "Social"
    }
    ```

### Refresh Token

*   **Endpoint:** `POST /api/Auth/refresh-token`
*   **Access:** Public (Requires a valid Refresh Token)
*   **Business Logic:** Generates a new JWT access token using a valid refresh token.
*   **JSON Body Example:**
    ```json
    {
      "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    }
    ```

## 2. Personal User Management (`UsersController`)

Endpoints for personal users to manage their professional and technical identity.

### Request Social Tag (Role)

*   **Endpoint:** `POST /api/Users/tags/request`
*   **Access:** Authenticated Personal Users
*   **Business Logic:** Allows a personal user to request a social tag or role (e.g., "Vendedor", "Mecánico"). This action sets the user's `TieneSolicitudPendiente` flag to `true`, adding the request to the administrator's validation queue.
    *   **"Vendedor" Tag:** Requires a document of type "FacturaServicioBasico".
    *   **"Oficio" Tags (e.g., "Mecánico"):** If `esEmpirico` is `true`, it requires photos of the workshop/work. If `false`, it requires a title or certificate.
*   **JSON Body Example:**
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

### Request Personal Module

*   **Endpoint:** `POST /api/Users/modules/request`
*   **Access:** Authenticated Personal Users
*   **Business Logic:** Allows a personal user to request access to a specialized software module. This is for future personal-use software and is separate from company modules. This action also sets the `TieneSolicitudPendiente` flag to `true`.
*   **JSON Body Example:**
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

## 3. Business Management (`CompanyManagementController`)

Endpoints for companies to manage their structure, employees, and commercial profiles.

### Create Employee

*   **Endpoint:** `POST /api/CompanyManagement/employees`
*   **Access:** Authenticated Company Users (Main account)
*   **Business Logic:** Creates a new employee (sub-account) under the company. The `permisos` dictionary is crucial for defining what the employee can access. The key is the name of the module (e.g., "Contabilidad"), and the value (`ModuleAccessDto`) specifies if they have access and to which specific functionalities or resources.
*   **JSON Body Example:**
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
> **⚠️ Architecture Rule (Total Delegation):**
> For external business modules (like `Construction.API`), `UserManagement`'s responsibility is strictly limited to **Access (On/Off)**.
> * `acceso`: `true` (Enables the menu button).
> * `funcionalidades`: Send `[]` (Empty). Roles (e.g., Engineer, Resident) are defined in the target microservice.
> * `recursosIds`: Send `[]` (Empty). Project/Resource assignment is handled in the target microservice.

### Request Commercial Profile

*   **Endpoint:** `POST /api/CompanyManagement/profiles/request`
*   **Access:** Authenticated Company Users
*   **Business Logic:** This is a key endpoint for a company to define its "faces" to the public. It can be used to request access to a technical software module or to create a social profile for a specific line of business (rubro).
    *   **To Request a Module:** Provide the `moduloAsociado` name.
    *   **To Request a Social Tag:** Leave `moduloAsociado` as `null` and specify the `rubro`.
*   **JSON Body Example (Module Request):**
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
*   **JSON Body Example (Social Tag Request):**
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

### Rectify Global Identity

*   **Endpoint:** `PUT /api/CompanyManagement/identity/rectify`
*   **Access:** Authenticated Company Users (only if their account status is `Rechazado`)
*   **Business Logic:** Allows a company whose initial registration was rejected (e.g., for an unreadable NIT) to submit new legal documents and re-enter the validation queue. This action changes the user's global status from `Rechazado` back to `Pendiente`.
*   **JSON Body Example:**
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

## 4. Administrator Panel (`AdminController`)

Endpoints for administrators to validate and manage user and company requests. All `GET` endpoints in this section are highly optimized and only query for users where `TieneSolicitudPendiente == true`, minimizing database reads on Firestore.

### Tray 1: Pending Identities

*   **Endpoint:** `GET /api/Admin/identities/pending`
*   **Access:** Admins (`AdminSistema`, `SuperAdminGlobal`)
*   **Business Logic:** Retrieves a list of new companies that are pending initial legal validation. This is the first entry gate for any company into the system.

*   **Endpoint:** `PUT /api/Admin/identities/decision`
*   **Access:** Admins
*   **Business Logic:** Approves or rejects a company's initial registration. Approving sets the company's status to `Activo`, allowing them to log in.
*   **JSON Body Example:**
    ```json
    {
      "userId": "user_id_of_the_company",
      "approve": true,
      "rejectionReason": null
    }
    ```

### Tray 2: Pending Company Modules

*   **Endpoint:** `GET /api/Admin/companies/modules/pending`
*   **Access:** Admins
*   **Business Logic:** Retrieves a list of commercial profiles from active companies that are requesting access to a technical software module.

*   **Endpoint:** `PUT /api/Admin/companies/modules/decision`
*   **Access:** Admins
*   **Business Logic:** Approves or rejects a company's request for a software module. If approved, the module is added to the company's `ModulosHabilitados` list.
*   **JSON Body Example:**
    ```json
    {
      "companyUserId": "company_user_id",
      "commercialProfileId": "profile_id_requesting_module",
      "approve": true,
      "rejectionReason": "Documentation is not sufficient."
    }
    ```

### Tray 3: Pending Social Tags

*   **Endpoint:** `GET /api/Admin/tags/pending`
*   **Access:** Admins
*   **Business Logic:** Retrieves a mixed list of pending social tag requests from both personal users (e.g., "Vendedor") and companies (e.g., "Restaurante").

*   **Endpoint:** `PUT /api/Admin/companies/tags/decision`
*   **Access:** Admins
*   **Business Logic:** Approves or rejects a company's request for a social tag profile.
*   **JSON Body Example:**
    ```json
    {
      "companyUserId": "company_user_id",
      "commercialProfileId": "profile_id_for_the_tag",
      "approve": true,
      "rejectionReason": null
    }
    ```

*   **Endpoint:** `PUT /api/Admin/personal/tags/decision`
*   **Access:** Admins
*   **Business Logic:** Approves or rejects a personal user's request for a social tag/role.
*   **JSON Body Example:**
    ```json
    {
      "personalUserId": "personal_user_id",
      "tagName": "Mecánico",
      "approve": false,
      "rejectionReason": "Evidence of work is not clear."
    }
    ```

### Tray 4: Pending Personal Modules

*   **Endpoint:** `GET /api/Admin/personal/modules/pending`
*   **Access:** Admins
*   **Business Logic:** Retrieves a list of personal users requesting access to specialized personal software modules.

*   **Endpoint:** `PUT /api/Admin/personal/modules/decision`
*   **Access:** Admins
*   **Business Logic:** Approves or rejects a personal user's request for a software module.
*   **JSON Body Example:**
    ```json
    {
      "personalUserId": "personal_user_id",
      "moduleName": "AnalisisFinancieroPersonal",
      "approve": true,
      "rejectionReason": null
    }
    ```

## 5. Review System (`ReviewsController`)

Endpoint for creating contextual reviews.

### Create Review

*   **Endpoint:** `POST /api/Reviews`
*   **Access:** Authenticated Users
*   **Business Logic:** Creates a review for a specific context. This is a critical piece of the reputation system. The `contextoId` is **mandatory** and must point to a specific `Tag` (e.g., "Mecánico") or a company's `PerfilComercial` ID. Reviews are **never** against the user directly, but against the role or profile they are operating under. This allows for contextual reputation (e.g., someone can be a 5-star mechanic but a 3-star salesperson).
*   **JSON Body Example:**
    ```json
    {
      "recipientId": "user_id_of_person_or_company_being_reviewed",
      "contextoId": "Mecánico", // This could also be a commercial profile ID
      "rating": 5,
      "comment": "Excellent service, very professional and solved the issue quickly."
    }
    ```

## 6. My Profile (Self-Management)

Endpoints for a logged-in user to manage their own profile and security settings.

### Get My Profile

*   **Endpoint:** `GET /api/Users/profile/me`
*   **Access:** Authenticated
*   **Description (ES):** Obtiene el perfil completo y detallado del usuario actualmente logueado. La respuesta es polimórfica y cambia según el tipo de usuario (Personal, Empresa, etc.).
*   **Description (EN):** Gets the full, detailed profile of the currently logged-in user. The response is polymorphic and changes depending on the user type (Personal, Company, etc.).
*   **JSON Response Example (Personal User):**
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

### Update My Profile

*   **Endpoint:** `PUT /api/Users/profile/me`
*   **Access:** Authenticated
*   **Description (ES):** Actualiza datos no sensibles del perfil del usuario, como su foto, biografía o información de contacto. Permite actualizaciones parciales.
*   **Description (EN):** Updates non-sensitive user profile data, such as photo, bio, or contact information. Allows for partial updates.
*   **JSON Body Example:**
    ```json
    {
      "biografia": "Desarrollador de software y entusiasta de la tecnología.",
      "celular": "77754321"
    }
    ```

### Change Password

*   **Endpoint:** `POST /api/Auth/change-password`
*   **Access:** Authenticated
*   **Description (ES):** Permite a un usuario logueado cambiar su propia contraseña. Requiere la contraseña actual para verificación.
*   **Description (EN):** Allows a logged-in user to change their own password. Requires the current password for verification.
*   **JSON Body Example:**
    ```json
    {
      "currentPassword": "Password123!",
      "newPassword": "NewSecurePassword456!",
      "confirmNewPassword": "NewSecurePassword456!"
    }
    ```

## 7. Human Resources (Company Management)

Endpoints for a company administrator to manage their employees (sub-accounts).

### List Employees

*   **Endpoint:** `GET /api/CompanyManagement/employees`
*   **Access:** Authenticated Company Admins
*   **Description (ES):** Obtiene una lista resumen de todos los empleados (sub-cuentas) que pertenecen a la empresa.
*   **Description (EN):** Gets a summary list of all employees (sub-accounts) belonging to the company.
*   **JSON Response Example:**
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

### Get Employee Detail

*   **Endpoint:** `GET /api/CompanyManagement/employees/{id}`
*   **Access:** Authenticated Company Admins
*   **Description (ES):** Obtiene la vista detallada de un empleado específico, incluyendo sus permisos.
*   **Description (EN):** Gets the detailed view of a specific employee, including their permissions.
*   **JSON Response Example:**
    ```json
    {
      "id": "employee_id_1",
      "nombreCompleto": "Carlos Solis",
      "email": "empleado1@example.com",
      "cargo": "Ventas",
      // ...other summary fields...
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

### Update Employee Permissions

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/permissions`
*   **Access:** Authenticated Company Admins
*   **Description (ES):** Actualiza el área de trabajo y la matriz de permisos de un empleado.
*   **Description (EN):** Updates an employee's work area and permissions matrix.
*   **JSON Body Example:**
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

### Update Employee Status

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/status`
*   **Access:** Authenticated Company Admins
*   **Description (ES):** Cambia el estado de un empleado (ej. `Activo`, `Suspendido`). Útil para despidos.
*   **Description (EN):** Changes an employee's status (e.g., `Activo`, `Suspendido`). Useful for terminations.
*   **JSON Body Example:**
    ```json
    {
      "nuevoEstado": "Suspendido"
    }
    ```

### Reset Employee Password

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/reset-password`
*   **Access:** Authenticated Company Admins
*   **Description (ES):** Permite a un administrador de empresa resetear manualmente la contraseña de un empleado.
*   **Description (EN):** Allows a company administrator to manually reset an employee's password.
*   **JSON Body Example:**
    ```json
    {
      "newPassword": "TemporaryPasswordForEmployee123!"
    }
    ```

### Update Employee Profile

*   **Endpoint:** `PUT /api/CompanyManagement/employees/{id}/profile`
*   **Access:** Authenticated Company Admins
*   **Description (ES):** Permite a un administrador corregir datos personales de un empleado (ej. errores de tipeo en el CI o Nombre).
*   **Description (EN):** Allows an administrator to correct an employee's personal data (e.g., typos in ID or Name).
*   **JSON Body Example:**
    ```json
    {
      "nombres": "Carlos Alberto",
      "ci": "8889991"
    }
    ```

## 8. Discovery & Reviews (Public)

Publicly accessible endpoints for finding users and seeing their reputation.

### Public Search

*   **Endpoint:** `GET /api/Users/public/search`
*   **Access:** Public (`AllowAnonymous`)
*   **Description (ES):** Buscador público de perfiles. Acepta un texto de búsqueda (`q`) y una ciudad opcional.
*   **Description (EN):** Public profile search. Accepts a query text (`q`) and an optional city.
*   **Example URL:** `/api/Users/public/search?q=Software&ciudad=Cochabamba`

### Get Public User Profile

*   **Endpoint:** `GET /api/Users/public/{userId}`
*   **Access:** Public (`AllowAnonymous`)
*   **Description (ES):** Obtiene la "tarjeta de presentación" pública de un usuario, ocultando datos sensibles.
*   **Description (EN):** Gets the public "business card" of a user, hiding sensitive data.
*   **JSON Response Example:**
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

### Get Public Company Profiles

*   **Endpoint:** `GET /api/CompanyManagement/profiles/{companyId}/public`
*   **Access:** Public (`AllowAnonymous`)
*   **Description (ES):** Lista los perfiles comerciales activos de una empresa específica.
*   **Description (EN):** Lists the active commercial profiles for a specific company.

### List Reviews

*   **Endpoint:** `GET /api/Reviews`
*   **Access:** Public (`AllowAnonymous`)
*   **Description (ES):** Obtiene las reseñas para un `recipientId` y un `contextoId` (Tag o Perfil Comercial).
*   **Description (EN):** Gets the reviews for a `recipientId` and `contextoId` (Tag or Commercial Profile).
*   **Example URL:** `/api/Reviews?recipientId=user_id_123&contextoId=Ingeniero%20de%20Software`
*   **JSON Response Example:**
    ```json
    [
      {
        "autorNombre": "Carlos Solis",
        "rating": 5,
        "comentario": "Excellent work, very professional and recommended.",
        "fecha": "2023-11-20T15:30:00Z"
      }
    ]
    ```

## 9. Support & Moderation (SuperAdmin)

High-privilege endpoints for system administrators.

### Admin User Search

*   **Endpoint:** `GET /api/Admin/users/search`
*   **Access:** SuperAdmins (`AdminSistema`, `SuperAdminGlobal`)
*   **Description (ES):** Búsqueda en cascada por un término exacto: Email > Username > CI > NIT.
*   **Description (EN):** Cascading search for an exact term: Email > Username > CI > NIT.
*   **Example URL:** `/api/Admin/users/search?term=juan.perez@example.com`

### Get Full User Detail

*   **Endpoint:** `GET /api/Admin/users/{id}`
*   **Access:** SuperAdmins
*   **Description (ES):** Obtiene el perfil completo de un usuario sin NINGÚN tipo de enmascaramiento o filtro de privacidad.
*   **Description (EN):** Gets the complete user profile with NO privacy masking or filtering.
*   **JSON Response Example:**
    ```json
    {
      "id": "user_id_123",
      "email": "juan.perez@example.com",
      // ... all fields ...
      "datosPersonales": {
        "nombres": "Juan",
        "apellidoPaterno": "Perez",
        "ci": "1234567", // Unmasked
        // ... all fields ...
      }
    }
    ```

### Admin Change User Status

*   **Endpoint:** `PUT /api/Admin/users/{id}/status`
*   **Access:** SuperAdmins
*   **Description (ES):** Permite a un admin cambiar forzosamente el estado de cualquier usuario (ej. `Activo`, `Suspendido`, `Eliminado`).
*   **Description (EN):** Allows an admin to forcibly change any user's status (e.g., `Activo`, `Suspendido`, `Eliminado`).
*   **JSON Body Example:**
    ```json
    {
      "nuevoEstado": "Suspendido",
      "motivo": "Violación de términos de servicio."
    }
    ```

### Admin Reset Password

*   **Endpoint:** `PUT /api/Admin/users/{id}/reset-password`
*   **Access:** SuperAdmins
*   **Description (ES):** Permite a un admin resetear la contraseña de cualquier usuario sin necesidad de la contraseña actual.
*   **Description (EN):** Allows an admin to reset any user's password without needing the current password.
*   **JSON Body Example:**
    ```json
    {
      "newPassword": "NewPasswordSetBySupport123!"
    }
    ```

## 10. Mi Perfil (Self-Management)

Endpoints for a logged-in user to manage their own profile and security settings.

### `GET /api/Users/profile/me`
*   **Desc:** Obtiene el perfil completo del usuario logueado. Respuesta polimórfica (Personal/Empresa/Empleado).
*   **Desc (EN):** Retrieves the complete profile of the logged-in user. Polymorphic response (Personal/Company/Employee).

    **JSON Response Example (Personal User):**
    ```json
    {
      "id": "user_id_123",
      "email": "user.email@example.com",
      "userType": "Personal",
      "status": "Active",
      "profilePictureUrl": "https://example.com/profile/picture.jpg",
      "bio": "Experienced software engineer with a passion for innovation.",
      "personalData": {
        "fullName": "John Doe",
        "ci": "1234567",
        "phoneNumber": "77788990",
        "address": "Av. Principal #123"
      }
    }
    ```

    **JSON Response Example (Company User):**
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
          "name": "Headquarters",
          "address": "Calle Falsa #456",
          "city": "La Paz"
        }
      ]
    }
    ```

### `PUT /api/Users/profile/me`
*   **Desc:** Actualiza datos no sensibles (Foto, Bio, Celular, Dirección).
*   **Desc (EN):** Updates non-sensitive data (Photo, Bio, Phone, Address).

    **JSON Body Example:**
    ```json
    {
      "profilePictureUrl": "https://example.com/new_picture.jpg",
      "bio": "Updated bio: Always learning and growing.",
      "phoneNumber": "77711223",
      "address": "Av. Secundaria #456"
    }
    ```

### `POST /api/Auth/change-password`
*   **Desc:** Cambia la contraseña. Requiere `CurrentPassword` y `NewPassword`.
*   **Desc (EN):** Changes the password. Requires `CurrentPassword` and `NewPassword`.

    **JSON Body Example:**
    ```json
    {
      "currentPassword": "OldSecurePassword1!",
      "newPassword": "NewSecurePassword2@",
      "confirmNewPassword": "NewSecurePassword2@"
    }
    ```

## 11. Recursos Humanos (Company Management)

### `GET /api/CompanyManagement/employees`
*   **Desc:** Lista todos los empleados (sub-cuentas) de la empresa.
*   **Desc (EN):** Lists all employees (sub-accounts) of the company.

    **JSON Response Example:**
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
        "workArea": "Sales",
        "status": "Active"
      }
    ]
    ```

### `GET /api/CompanyManagement/employees/{id}`
*   **Desc:** Ver detalle y permisos de un empleado específico.
*   **Desc (EN):** View details and permissions of a specific employee.

    **JSON Response Example:**
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

    **JSON Body Example:**
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

    **JSON Body Example:**
    ```json
    {
      "newStatus": "Suspended"
    }
    ```

### `PUT /api/CompanyManagement/employees/{id}/reset-password`
*   **Desc:** (Admin Empresa) Resetea la contraseña de un empleado manualmente.
*   **Desc (EN):** (Company Admin) Manually resets an employee's password.

    **JSON Body Example:**
    ```json
    {
      "newPassword": "EmployeeNewPass123!"
    }
    ```

### `PUT /api/CompanyManagement/employees/{id}/profile`
*   **Desc:** (Admin Empresa) Corrige datos personales del empleado (CI, Nombre).
*   **Desc (EN):** (Company Admin) Corrects employee's personal data (ID, Name).

    **JSON Body Example:**
    ```json
    {
      "fullName": "Jane A. Smith",
      "ci": "11223345"
    }
    ```

## 12. Discovery & Reviews (Public)

### `GET /api/Users/public/search`
*   **Desc:** Buscador público. Params: `q` (texto), `ciudad`. Retorna Personas y Empresas.
*   **Desc (EN):** Public search engine. Params: `q` (text), `city`. Returns People and Companies.

    **Example URL:** `/api/Users/public/search?q=developer&ciudad=Cochabamba`

    **JSON Response Example:**
    ```json
    [
      {
        "id": "user_id_1",
        "name": "Alice Wonderland",
        "type": "Personal",
        "description": "Software developer specializing in web applications.",
        "profileImageUrl": "https://example.com/profile/alice.jpg"
      },
      {
        "id": "company_id_2",
        "name": "Global Tech Solutions",
        "type": "Company",
        "description": "Leading tech company offering IT services.",
        "profileImageUrl": "https://example.com/profile/globaltech.jpg"
      }
    ]
    ```

### `GET /api/Users/public/{userId}`
*   **Desc:** Tarjeta de presentación pública. Oculta datos sensibles según privacidad.
*   **Desc (EN):** Public business card. Hides sensitive data according to privacy settings.

    **JSON Response Example:**
    ```json
    {
      "id": "user_id_123",
      "name": "John Doe",
      "userType": "Personal",
      "bio": "Passionate about open-source projects and contributing to the community.",
      "city": "La Paz",
      "contactInfo": {
        "email": "john.doe.public@example.com"
      },
      "publicTags": [
        {
          "tagName": "Web Developer",
          "rating": 4.9,
          "reviewCount": 50
        }
      ]
    }
    ```

### `GET /api/CompanyManagement/profiles/{companyId}/public`
*   **Desc:** Lista sucursales y perfiles comerciales activos de una empresa.
*   **Desc (EN):** Lists active branches and commercial profiles of a company.

    **JSON Response Example:**
    ```json
    [
      {
        "profileId": "commercial_profile_1",
        "profileName": "Main Branch Services",
        "description": "General IT services for businesses.",
        "address": "Av. Ayacucho #100",
        "phoneNumber": "22233445",
        "avgRating": 4.7,
        "totalReviews": 120
      },
      {
        "profileId": "commercial_profile_2",
        "profileName": "Software Development Division",
        "description": "Custom software solutions.",
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

    **Example URL:** `/api/Reviews?recipientId=user_id_123&contextId=Web%20Developer`

    **JSON Response Example:**
    ```json
    [
      {
        "reviewId": "review_id_1",
        "reviewerName": "Client A",
        "rating": 5,
        "comment": "Excellent work, delivered on time and exceeded expectations.",
        "date": "2023-11-01T10:00:00Z"
      },
      {
        "reviewId": "review_id_2",
        "reviewerName": "Client B",
        "rating": 4,
        "comment": "Good communication, minor delays but overall satisfied.",
        "date": "2023-10-25T14:30:00Z"
      }
    ]
    ```

## 13. Soporte y Moderación (SuperAdmin)

### `GET /api/Admin/users/search`
*   **Desc:** Búsqueda por Email, CI, NIT o Username.
*   **Desc (EN):** Search by Email, ID, NIT, or Username.

    **Example URL:** `/api/Admin/users/search?query=admin@example.com`

    **JSON Response Example:**
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

    **JSON Response Example:**
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
      "privateNotes": "User has a history of suspicious login attempts."
    }
    ```

### `PUT /api/Admin/users/{id}/status`
*   **Desc:** Banear/Suspender o Reactivar cualquier usuario.
*   **Desc (EN):** Ban/Suspend or Reactivate any user.

    **JSON Body Example:**
    ```json
    {
      "newStatus": "Banned",
      "reason": "Repeated violations of terms of service."
    }
    ```

### `PUT /api/Admin/users/{id}/reset-password`
*   **Desc:** (Soporte) Reseteo de emergencia de contraseña.
*   **Desc (EN):** (Support) Emergency password reset.

    **JSON Body Example:**
    ```json
    {
      "newPassword": "EmergencyResetPass#123",
      "notifyUser": true
    }
    ```
