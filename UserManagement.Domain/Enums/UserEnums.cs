using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Enums
{
    public enum UserType
    {
        // 1. NIVEL GLOBAL (Dueños del Software)
        SuperAdminGlobal = 0, // El "Dios" del sistema (Tú). Nadie lo borra.
        AdminSistema = 1,     // Soporte, Validadores de documentos, Editores de precios base.

        // 2. NIVEL CLIENTES (Pagan o usan el servicio)
        Empresa = 10,         // Dueño de la cuenta empresarial.
        Personal = 11,        // Profesional independiente / Usuario normal.

        // 3. NIVEL SUB-CUENTAS (Dependen de una Empresa)
        SubCuentaEmpresa = 20 // Empleado creado por una Empresa.
    }

    public enum UserStatus
    {
        Pendiente = 0,   // Esperando validación (Empresas/Market)
        Activo = 1,      // Puede entrar
        Suspendido = 2,  // Bloqueo temporal (falta de pago, etc.)
        Rechazado = 3,   // Documentos inválidos
        Eliminado = 99   // Soft delete (papelera)
    }
}