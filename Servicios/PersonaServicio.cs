using AnhApi.Datos; 
using AnhApi.Modelos;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class PersonaServicio : GenericoServicio<Persona, Guid>
    {
        private readonly AppDbContext _contexto; 
        private readonly ILogger<PersonaServicio> _logger; 

        public PersonaServicio(AppDbContext contexto, ILogger<PersonaServicio> logger)
            : base(contexto, logger) // Llama al constructor de GenericoServicio
        {
            _contexto = contexto;
            _logger = logger;
        }

        /// <summary>
        /// Busca personas utilizando un procedimiento almacenado en la base de datos PostgreSQL.
        /// </summary>
        /// <param name="criterio">El texto a buscar en el procedimiento almacenado.</param>
        /// <returns>Una colección de Personas que coinciden con el criterio.</returns>
        public async Task<IEnumerable<Persona>> Buscar(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    return await ObtenerTodosAsync();
                }

                var personas = await _contexto.Personas
                                           .FromSqlRaw("SELECT * FROM \"public\".\"buscar_personas\"(@criterio)", // Asegúrate del esquema y nombre exactos del SP
                                                       new NpgsqlParameter("criterio", criterio))
                                           .ToListAsync();
                return personas;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, $"Error de PostgreSQL al buscar personas con criterio '{criterio}' usando el procedimiento almacenado.");
                throw new ApplicationException($"Error al ejecutar el procedimiento de búsqueda de personas.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al buscar personas con criterio '{criterio}'.");
                throw;
            }
        }
    }
}