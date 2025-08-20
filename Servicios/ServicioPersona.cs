using AnhApi.Datos; 
using AnhApi.Modelos;
using AnhApi.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AnhApi.Esquemas;
using AutoMapper;

namespace AnhApi.Servicios
{
    public class ServicioPersona : ServicioAuditoria<Persona, Guid>, IServicioPersona
    {
        private readonly ContextoAppBD _contexto; 
        private readonly ILogger<ServicioPersona> _logger;
        private readonly IMapper _mapper;

        public ServicioPersona(ContextoAppBD contexto, ILogger<ServicioPersona> logger, IMapper mapper)
            : base(contexto, logger) // Llama al constructor de GenericoServicio
        {
            _contexto = contexto;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Busca personas utilizando un procedimiento almacenado en la base de datos PostgreSQL.
        /// </summary>
        /// <param name="criterio">El texto a buscar en el procedimiento almacenado, puede ser Paterno, Nombre Paterno, Fecha nacimiento, CI.</param>
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

        public async Task<PaginacionResultado<PersonaListado>> BuscarPaginado(string criterio, PaginacionParametros paginacion)
        {
            try
            {
                if (paginacion == null)
                {
                    paginacion = new PaginacionParametros(); // Asigna valores por defecto si es nulo
                }

                IQueryable<Persona> query;

                if (string.IsNullOrWhiteSpace(criterio))
                {
                    query = _contexto.Personas.Where(p => p.aud_estado == 0); // Filtra por estado activo
                }
                else
                {
                    query = _contexto.Personas
                                     .FromSqlRaw("SELECT * FROM \"public\".\"buscar_personas\"(@criterio)", // Asegúrate del esquema y nombre exactos del SP
                                                 new NpgsqlParameter("criterio", criterio));
                }
                var totalRegistros = await query.CountAsync();
                var resultadoPaginado = await query
                    .Skip(paginacion.ElementosAOmitir)
                    .Take(paginacion.TamanoPagina)
                    .ToListAsync();

                var personasListado = _mapper.Map<IEnumerable<PersonaListado>>(resultadoPaginado);

                return new PaginacionResultado<PersonaListado>
                {
                    Elementos = personasListado,
                    TotalRegistros = totalRegistros,
                    PaginaActual = paginacion.PaginaNumero,
                    TamanoPagina = paginacion.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)totalRegistros / paginacion.TamanoPagina)
                };

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