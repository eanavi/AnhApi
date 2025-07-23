namespace AnhApi.Esquemas
{ /// <summary>
  /// Opciones de configuración para la integración con LDAP/Active Directory.
  /// </summary>
    public class LdapOptions
    {
        public string DirectoryEntry { get; set; } = null!;
        public string Cn { get; set; } = null!; // Common Name attribute, usually "cn"
        public string Domain { get; set; } = null!;
        public string PhysicalDeliveryOfficeName { get; set; } = null!;
        public string Filter { get; set; } = null!;
    }
}