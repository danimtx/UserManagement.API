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
          "funcionalidades": ["VerStock", "SolicitarProducto"],
          "recursosIds": []
        }
      }
    }
    ```

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

