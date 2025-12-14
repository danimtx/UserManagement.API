# QA Testing Plan

This document outlines End-to-End testing scenarios to validate the core business logic of the User Management API. It serves as a guide for QA engineers and for the development of automated tests.

## Instructions

Each scenario describes a full user flow, detailing steps with specific actions/endpoints, key data to be used, and the expected outcomes including HTTP response codes and database effects.

---

## Scenarios

### 🧪 Scenario 1: The "Mechanic" Life Cycle (Personal User)

**Objective:** To test user registration, role request, administrative validation, and reputation management for a personal user.

| Step | Action/Endpoint                | Key Data                                            | Expected Result (HTTP Code + DB Effect)                                                                         |
| :--- | :----------------------------- | :-------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------- |
| 1    | `POST /api/Auth/register/personal` | Valid `RegisterPersonalDto` for a new "Mechanic" user. | `200 OK`. User created in DB with `Activo` status.                                                               |
| 2    | `POST /api/Auth/login`         | User's email and password.                          | `200 OK`. Returns authentication token.                                                                           |
| 3    | `POST /api/Users/tags/request`   | `RequestTagDto` for "Mecánico" with relevant `evidencias` (e.g., photos). | `200 OK`. User's `TieneSolicitudPendiente` flag set to `true` in DB. "Mecánico" tag added with `Pendiente` status. |
| 4    | `GET /api/Admin/tags/pending`    | Admin's authentication token.                       | `200 OK`. Response body contains the newly registered user's pending "Mecánico" tag request.                      |
| 5    | `PUT /api/Admin/personal/tags/decision` | `PersonalTagDecisionDto` to approve "Mecánico" tag. | `200 OK`. "Mecánico" tag status changes to `Activo`. User's `TieneSolicitudPendiente` flag remains `true` (if other pending requests exist) or `false` (if no other requests). |
| 6    | `POST /api/Reviews`            | `CreateReviewDto` for the "Mechanic" context (target user, `contextoId`: "Mecánico", rating, comment). | `200 OK`. New review added to DB, associated with the "Mecánico" tag. Average rating for "Mecánico" tag updated. |
| 7    | `POST /api/Reviews`            | Same `CreateReviewDto` as Step 6, but with `authorId` = `recipientId`. | `400 Bad Request`. Error indicating a user cannot review themselves in the same context.                        |

### 🧪 Scenario 2: The Rectifying Company (Complex Flow)

**Objective:** To test company registration, login blocking, rejection, and rectification processes.

| Step | Action/Endpoint                  | Key Data                                            | Expected Result (HTTP Code + DB Effect)                                                                         |
| :--- | :------------------------------- | :-------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------- |
| 1    | `POST /api/Auth/register/company` | Valid `RegisterCompanyDto`.                         | `200 OK`. Company created in DB with `Pendiente` status. `TieneSolicitudPendiente` set to `true`.                 |
| 2    | `POST /api/Auth/login`           | Company's email and password.                       | `401 Unauthorized`. Error message indicating account is pending approval.                                         |
| 3    | `GET /api/Admin/identities/pending` | Admin's authentication token.                       | `200 OK`. Response body contains the newly registered company.                                                    |
| 4    | `PUT /api/Admin/identities/decision` | `IdentityDecisionDto` to **reject** the company (e.g., `approve: false`, `rejectionReason: "Unreadable NIT"`). | `200 OK`. Company's status changes to `Rechazado` in DB.                                                          |
| 5    | `PUT /api/CompanyManagement/identity/rectify` | `RectifyIdentityDto` with updated `documentosLegales` (e.g., a clearer NIT scan). | `200 OK`. Company's status changes from `Rechazado` to `Pendiente` in DB.                                         |
| 6    | `PUT /api/Admin/identities/decision` | `IdentityDecisionDto` to **approve** the company (`approve: true`). | `200 OK`. Company's status changes to `Activo` in DB. `TieneSolicitudPendiente` flag remains `true` (if other pending requests exist) or `false` (if no other requests). |
| 7    | `POST /api/Auth/login`           | Company's email and password.                       | `200 OK`. Returns authentication token.                                                                           |

### 🧪 Scenario 3: Module and Profile Management (Company)

**Objective:** To test the creation of different commercial "faces" for a company.

| Step | Action/Endpoint                      | Key Data                                            | Expected Result (HTTP Code + DB Effect)                                                                         |
| :--- | :----------------------------------- | :-------------------------------------------------- | :---------------------------------------------------------------------------------------------------------------- |
| 1    | `POST /api/CompanyManagement/profiles/request` | `RequestCommercialProfileDto` for "Restaurante" (social tag, `moduloAsociado: null`, `rubro: "Restaurantes"`). | `200 OK`. New `PerfilComercial` created with `TagSocial` type and `Pendiente` status. Company's `TieneSolicitudPendiente` flag set to `true`. |
| 2    | `POST /api/CompanyManagement/profiles/request` | `RequestCommercialProfileDto` for "Construccion" (module, `moduloAsociado: "Construccion"`). | `200 OK`. New `PerfilComercial` created with `Modulo` type and `Pendiente` status. Company's `TieneSolicitudPendiente` flag remains `true`. |
| 3    | `GET /api/Admin/companies/tags/pending`    | Admin's authentication token.                       | `200 OK`. Response body contains the pending "Restaurante" social tag.                                            |
| 4    | `GET /api/Admin/companies/modules/pending` | Admin's authentication token.                       | `200 OK`. Response body contains the pending "Construccion" module request.                                       |

### 🧪 Scenario 4: Security and Roles

**Objective:** To test that a normal user cannot perform administrative actions.

| Step | Action/Endpoint                          | Key Data                      | Expected Result (HTTP Code + DB Effect)           |
| :--- | :--------------------------------------- | :---------------------------- | :------------------------------------------------ |
| 1    | `GET /api/Admin/identities/pending`      | Personal user's authentication token. | `403 Forbidden`. No change in DB.                 |
| 2    | `POST /api/Auth/register/personal`         | Valid `RegisterPersonalDto` for a new personal user. | `200 OK`. User created in DB with `Activo` status. |
| 3    | `POST /api/Auth/login`                   | Newly registered personal user's credentials. | `200 OK`. Returns authentication token for the personal user. |
| 4    | `GET /api/Admin/identities/pending`      | Personal user's authentication token from Step 3. | `403 Forbidden`. No change in DB.                 |
