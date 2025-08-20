// Archivo: AnhApi.Esquemas/PersonaCreacion.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    #region CreacionPersona
    /// <summary>
    /// Esquema para la creación de una nueva persona.
    /// Contiene los campos básicos para registrar una persona.
    /// </summary>
    public class PersonaCreacion
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(60, ErrorMessage = "El nombre no puede exceder los 60 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El primer apellido es requerido.")]
        [StringLength(30, ErrorMessage = "El primer apellido no puede exceder los 30 caracteres.")]
        public string PrimerApellido { get; set; } = null!;

        [Required(ErrorMessage = "El segundo apellido es requerido.")]
        [StringLength(30, ErrorMessage = "El segundo apellido no puede exceder los 30 caracteres.")]
        public string SegundoApellido { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El número de identificación es requerido.")]
        [StringLength(20, ErrorMessage = "El número de identificación no puede exceder los 20 caracteres.")]
        public string NumeroIdentificacion { get; set; } = null!;

        [StringLength(10, ErrorMessage = "El complemento no puede exceder los 10 caracteres.")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "El género es requerido.")]
        public int Genero { get; set; }

        // Estos campos son parte de la creación, pero podrían no estar en el listado si se quiere un DTO más ligero
        public object? Direccion { get; set; }
        public object? Telefono { get; set; }
        public object? Correo { get; set; }
    }

    #endregion

    #region PersonaListado
    /// <summary>
    /// Esquema para listar una persona (vista reducida).
    /// Contiene solo los campos esenciales para un listado, excluyendo detalles de contacto.
    /// </summary>
    public class PersonaListado
    {
        [Required(ErrorMessage = "El ID de la persona es requerido.")]
        public Guid IdPersona { get; set; } // Identificador único de la persona

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El primer apellido es requerido.")]
        public string PrimerApellido { get; set; } = null!;

        [Required(ErrorMessage = "El segundo apellido es requerido.")]
        public string SegundoApellido { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El número de identificación es requerido.")]
        public string NumeroIdentificacion { get; set; } = null!;

        public string? Complemento { get; set; }

        [Required(ErrorMessage = "El género es requerido.")]
        public int Genero { get; set; }
        // Aquí NO se incluyen Direccion, Telefono ni Correo.
    }

    #endregion PersonaListado

    #region PersonaEsq
    /// <summary>
    /// Esquema completo de una persona, usado para respuestas detalladas y actualizaciones.
    /// Incluye todos los campos de creación y auditoría.
    /// </summary>
    public class EsqPersona : PersonaCreacion // Ahora hereda de PersonaCreacion que SÍ tiene Direccion, Telefono, Correo
    {
        [Required(ErrorMessage = "El ID de la persona es requerido.")]
        public Guid IdPersona { get; set; }

        // Campos de auditoría que son opcionales
        public int? AudEstado { get; set; }

        [Required(ErrorMessage = "El usuario de auditoría es requerido.")]
        [StringLength(30, ErrorMessage = "El usuario de auditoría no puede exceder los 30 caracteres.")]
        public string AudUsuario { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de auditoría es requerida.")]
        public DateTime AudFecha { get; set; }

        [Required(ErrorMessage = "La IP de auditoría es requerida.")]
        [StringLength(15, ErrorMessage = "La IP de auditoría no puede exceder los 15 caracteres.")]
        public string AudIp { get; set; } = null!;
    }

    #endregion PersonaEsq

}