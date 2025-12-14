# 🌟 Contexto del Proyecto: IO GAMA - UserManagement Microservice

## 1. Visión General y Filosofía
**UserManagement.API** es el **Núcleo de Identidad y Reputación** del ecosistema IO GAMA.

Su propósito va más allá del registro de usuarios; es una plataforma diseñada bajo la filosofía **"AI-First" (Inteligencia Artificial Primero)**.

### Principios Fundamentales
1.  **Identidad vs. Capacidad:** Separamos estrictamente la validación legal ("Quién eres") de la validación operativa ("Qué sabes hacer").
2.  **Reputación Contextual:** Las estrellas no pertenecen a la persona, sino al rol. Un usuario puede ser un excelente Mecánico (⭐5.0) pero un mal Vendedor (⭐2.0).
3.  **Automatización Total:** La arquitectura está preparada para que Agentes de IA (n8n) realicen el trabajo pesado de validación, dejando a los humanos solo las excepciones.

---

## 2. El "Admin" del Futuro: Automatización con n8n

El sistema está diseñado para reducir la carga operativa humana mediante **Agentes Autónomos**.

### ¿Cómo funciona la Validación Automática?
Dado que los endpoints están segregados por "Bandejas", un flujo de n8n puede actuar como un **AdminSistema** virtual:

1.  **Polling Inteligente:** El Agente consulta `GET /identities/pending`. Gracias al flag `TieneSolicitudPendiente`, la consulta es instantánea y barata.
2.  **Análisis de Visión:** El Agente extrae las URLs de los documentos (ej. Foto de CI, Factura de Luz) y las pasa por un modelo de Visión (GPT-4o / Google Vision).
3.  **Toma de Decisión:**
    * *Caso:* Solicita Tag "Vendedor".
    * *IA:* "¿La dirección en la factura de luz coincide con la del perfil?"
    * *Acción:* Si coincide, el Agente llama a `PUT /tags/decision` con `Approve=true`. Si no, rechaza con motivo detallado.
4.  **Resultado:** El usuario recibe respuesta en segundos, 24/7, sin intervención humana.

---

## 3. Actores y Jerarquía del Sistema

### A. Nivel Administración (Híbrido)
1.  **SuperAdminGlobal:** Acceso total y configuración del sistema.
2.  **AdminSistema (Humanos + IA):**
    * **Agentes n8n:** Procesan el 90% de las solicitudes estándar (validación de fotos, documentos legibles).
    * **Operadores Humanos:** Atienden solo los casos que la IA marca como "Dudosos" o reclamos de soporte.

### B. Nivel Clientes (Empresas)
La "Empresa" es una entidad legal única con múltiples "Caras".
* **Cuenta Principal:** Representante Legal.
* **Perfiles Comerciales (Multicuentas):** Una sola empresa gestiona varios negocios (ej. una Ferretería y un Restaurante) bajo el mismo NIT, cada uno con su propia reputación.
* **Sub-Cuentas:** Empleados con permisos granulares.

### C. Nivel Usuarios Personales (La Fuerza Laboral)
Usuarios que evolucionan mediante "Insignias" (Tags).
* **Nivel 1:** Ciudadano (Ver/Comprar).
* **Nivel 2:** Identidad Verificada (Puede Vender). Requiere validación de domicilio.
* **Nivel 3:** Oficio (Puede ofrecer Servicios). Requiere validación de Título o Evidencia Empírica (fotos de taller).

---

## 4. Arquitectura de Negocio

### 4.1 El Motor de Flujo Circular
El rechazo nunca es el final. El sistema fomenta la rectificación.
1.  **Solicitud:** Usuario envía datos. -> `User.TieneSolicitudPendiente = true`.
2.  **Revisión (IA/Humano):**
    * **Aprobado:** Recurso activo inmediatamente.
    * **Rechazado:** Se guarda el motivo. El usuario puede reenviar la solicitud (usando el mismo endpoint) con documentos corregidos.
3.  **Cierre:** El flag `TieneSolicitudPendiente` solo se apaga cuando no quedan tareas pendientes para ese usuario.

### 4.2 Reputación Contextual (Reviews)
Las reseñas se vinculan al **Contexto**, nunca al usuario global.
* `Review { ContextoId: "Mecanico" }` -> Afecta solo al promedio del Tag "Mecánico".
* `Review { ContextoId: "Perfil_Restaurante_123" }` -> Afecta solo a esa sucursal de la empresa.

### 4.3 Diferencia Módulo vs. Tag
* **Módulo:** Software técnico (ej. Precios Unitarios). Validación estricta.
* **Tag:** Presencia social y reputación. Validación de identidad/oficio.

---

## 5. Glosario de Entidades Técnicas

| Entidad | Descripción | Ubicación |
| :--- | :--- | :--- |
| **`User`** | Raíz de la cuenta. Contiene el Login y el flag maestro `TieneSolicitudPendiente`. | `User.cs` |
| **`PerfilComercial`** | "Cara" pública de una empresa. Tiene Logo y Reputación propia. | `PerfilComercial.cs` |
| **`Tag`** | Rol de una persona (Mecánico, Vendedor). Tiene Reputación propia. | `Tag.cs` |
| **`ModuleRequest`** | Solicitud para usar software técnico (Personas o Empresas). | `ModuleRequest.cs` |
| **`UploadedDocument`** | Contenedor de URLs de evidencia (Fotos, PDFs). No valida, solo almacena. | `UploadedDocument.cs` |
| **`Review`** | Calificación vinculada a un Contexto específico. | `Review.cs` |

---

## 6. Estrategia de Optimización (Performance)

Esta estrategia es **crítica** para que los Agentes de n8n no saturen la base de datos (Firestore) ni disparen los costos.

1.  **Escritura (Trigger):** Al crear cualquier solicitud (`POST`), el backend setea `TieneSolicitudPendiente = true`.
2.  **Lectura (Polling):** Los Agentes n8n y el Panel Admin consultan **EXCLUSIVAMENTE** `Where("TieneSolicitudPendiente", "==", true)`.
    * *Beneficio:* De 100,000 usuarios, el Agente solo lee los 50 que requieren atención.
3.  **Resolución:** Cuando se aprueba/rechaza la última solicitud pendiente de un usuario, el sistema setea el flag a `false` automáticamente.

## 7. Diccionario de Datos y Estrategia de Etiquetado (Documentos)

Para que la automatización funcione, cada documento subido debe tener una **Etiqueta de Contexto** (`ModuloObjetivo`) precisa. Esto permite al Admin (o al Agente n8n) filtrar solo lo que necesita revisar en ese momento.

El sistema maneja 3 etiquetas maestras:

### A. Etiqueta "General" (Identidad Legal)
Se usa exclusivamente para la **Existencia del Usuario**.
* **Aplica a:** Empresas (principalmente) y Personas (en registro inicial).
* **Cuándo se usa:** Registro (`RegisterCompany`) y Rectificación de Identidad (`RectifyIdentity`).
* **Documentos típicos:**
    * Empresas: NIT, Matrícula de Comercio (Seprec), Poder del Representante, CI del Representante.
    * Personas: CI (Anverso/Reverso).
* **Regla:** Si `ModuloObjetivo == "General"`, esto bloquea/desbloquea el acceso global a la cuenta (`User.Estado`).

### B. Etiqueta "NombreDelModulo" (Software Técnico)
Se usa para validar el acceso a herramientas de software (funcionalidad dura). La etiqueta es dinámica y debe coincidir exactamente con el ID del módulo.
* **Aplica a:** Empresas y Personas.
* **Cuándo se usa:** `RequestCommercialProfile` (tipo Módulo) o `RequestPersonalModule`.
* **Ejemplos de Etiquetas:**
    * `"Construccion"` -> Requiere: Licencia Ambiental, Registro de Ingeniero.
    * `"Ferreteria"` -> Requiere: Licencia de Funcionamiento Comercial.
    * `"Contabilidad"` -> Requiere: Registro de Contador.
* **Regla:** Estos documentos solo se revisan en la "Bandeja de Módulos". Si se rechazan, solo se bloquea el software, no la cuenta.

### C. Etiqueta "NombreDelTag" (Reputación Social)
Se usa para validar roles, oficios y categorías en el Marketplace/Red Social.
* **Aplica a:** Personas (Oficios) y Empresas (Rubros Sociales).
* **Cuándo se usa:** `RequestTag` (Personas) o `RequestCommercialProfile` (tipo Tag).
* **Ejemplos de Etiquetas:**
    * `"Vendedor"` -> Requiere: Factura de Luz/Agua (Prueba de Domicilio).
    * `"Mecanico"` -> Requiere: Foto del Taller, Foto de Herramientas (Evidencia Empírica).
    * `"Restaurante"` -> Requiere: Menú, Fotos del Local.
    * `"Electricista"` -> Requiere: Certificado/Título Técnico.
* **Regla:** Estos documentos validan la "Insignia" pública.

---

### Resumen de Mapeo (Guía para Frontend/Agentes)

| Tipo de Solicitud | Endpoint | Valor a enviar en `ModuloObjetivo` |
| :--- | :--- | :--- |
| **Activar Empresa** | `/register/company` | `"General"` |
| **Rectificar Empresa** | `/identity/rectify` | `"General"` |
| **Pedir Software** | `/profiles/request` (Empresa) | `"Construccion"`, `"Ferreteria"`, etc. |
| **Pedir Software** | `/modules/request` (Personal) | `"FinanzasPersonal"`, `"Inventario"`, etc. |
| **Ser Vendedor** | `/tags/request` | `"Vendedor"` |
| **Validar Oficio** | `/tags/request` | `"Mecanico"`, `"Plomero"`, `"Abogado"` |
| **Rubro Social** | `/profiles/request` (Empresa) | `"Restaurante"`, `"Hotel"`, `"TiendaRopa"` |