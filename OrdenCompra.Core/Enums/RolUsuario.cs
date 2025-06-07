namespace OrdenCompra.Core.Enums
{
    public static class RolUsuario
    {
        public const string Administrador = "Administrador";
        public const string Usuario = "Usuario";
        public const string Supervisor = "Supervisor";

        public static readonly string[] TodosLosRoles = { Administrador, Usuario, Supervisor };

        public static bool EsRolValido(string rol)
        {
            return TodosLosRoles.Contains(rol);
        }
    }
}
