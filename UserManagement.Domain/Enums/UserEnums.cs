using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Enums
{
    // Define QUÉ es el usuario dentro del sistema
    public enum UserType
    {
        // Cuenta personal: Acceso a Social, Wallet y Market (como comprador/vendedor)
        Personal = 0,

        // Cuenta empresarial principal: Dueño de los datos, paga los módulos, crea subcuentas
        Empresa = 1,

        // Cuentas creadas por la Empresa: Sus permisos dependen de lo que la empresa les asigne
        SubCuenta = 2,

        // Super Administrador: Gestiona validaciones de empresas y reseteos
        AdminSistema = 3
    }

    // Define si el usuario PUEDE ENTRAR o no al sistema
    public enum UserStatus
    {
        // Estado inicial para EMPRESAS. No pueden entrar hasta que el Admin verifique sus documentos (NIT, SEPREC).
        Pendiente = 0,

        // Usuario validado (o personal recién creado). Puede iniciar sesión normalmente.
        Activo = 1,

        // Usuario bloqueado temporalmente (por falta de pago o comportamiento indebido).
        Suspendido = 2,

        // El Admin revisó los documentos de la empresa y los denegó (no son válidos).
        Rechazado = 3
    }
}