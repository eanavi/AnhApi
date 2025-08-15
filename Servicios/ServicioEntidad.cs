using AnhApi.Esquemas;
using AnhApi.Modelos;
using Npgsql;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AnhApi.Interfaces;

namespace AnhApi.Servicios
{
    public class ServicioEntidad : ServicioAuditoria<Entidad, Guid>, Interfaces.IServicioEntidad
    {
        private readonly Datos.ContextoAppBD _contexto;
        private readonly ILogger<ServicioEntidad> _logger;
        private readonly IMapper _mapper;
        public ServicioEntidad(Datos.ContextoAppBD contexto, ILogger<ServicioEntidad> logger, IMapper mapper)
            : base(contexto, logger) // Llama al constructor de GenericoServicio
        {
            _contexto = contexto;
            _logger = logger;
            _mapper = mapper;
        }



        public async Task<PaginacionResultado<EntidadListado>> BuscarPaginado(string criterio, PaginacionParametros paginacion)
        {
            try
            {
                if (paginacion == null)
                {
                    paginacion = new PaginacionParametros();
                }

                var listado = await _contexto.EntidadesListado.FromSqlRaw("select * from \"public\".\"buscar_entidad\"(@criterio)", 
                    new NpgsqlParameter("criterio", criterio)).ToListAsync();
                var totalRegistros = listado.Count;
                var resultado = listado
                    .Skip(paginacion.ElementosAOmitir)
                    .Take(paginacion.TamanoPagina)
                    .ToList();
                return new PaginacionResultado<EntidadListado>
                {
                    Elementos = resultado,
                    TotalRegistros = totalRegistros,
                    PaginaActual = paginacion.PaginaNumero,
                    TamanoPagina = paginacion.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)totalRegistros / paginacion.TamanoPagina)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar entidades con criterio '{Criterio}'.", criterio);
                throw new ApplicationException($"Error al buscar entidades con criterio '{criterio}'.", ex);
            }
        }

        public async Task<PaginacionResultado<EntidadListado>> listarPaginado(PaginacionParametros paginacion)
        {
            try
            {
                if (paginacion == null)
                {
                    paginacion = new PaginacionParametros();
                }

                var listado = await _contexto.EntidadesListado.FromSqlRaw("select * from \"public\".\"buscar_entidad\"('')").ToListAsync();
                var total = listado.Count;
                var resultado = listado
                    .Skip(paginacion.ElementosAOmitir)
                    .Take(paginacion.TamanoPagina)
                    .ToList();

                return new PaginacionResultado<EntidadListado>
                {
                    Elementos = resultado,
                    TotalRegistros = total,
                    PaginaActual = paginacion.PaginaNumero,
                    TamanoPagina = paginacion.TamanoPagina,
                    TotalPaginas = (int)Math.Ceiling((double)total / paginacion.TamanoPagina)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ");
                throw new ApplicationException($"Error al buscar entidades", ex);
            }
        }

        public async Task<EntidadListadoRepresentante> EntidadConRepresentante(Guid id)
        {
            try
            {
                var entidad = await _contexto.EntidadesListado
                    .FromSqlInterpolated($"select * from \"public\".\"entidad_listado\"({id})")
                    .Select(e => new EntidadListadoRepresentante
                    {
                        Id_Entidad = e.Id_Entidad,
                        Tipo = e.Tipo,
                        Sociedad = e.Sociedad,
                        Area = e.Area,
                        Localidad = e.Localidad,
                        Municipio = e.Municipio,
                        Provincia = e.Provincia,
                        Departamento = e.Departamento,
                        Denominacion = e.Denominacion,
                        Sigla = e.Sigla,
                        Identificacion = e.Identificacion,
                        Tipo_Identificacion = e.Tipo_Identificacion,
                        Estado_Operacion = e.Estado_Operacion,
                        Empadronado = e.Empadronado
                    })
                    .FirstOrDefaultAsync();

                if (entidad == null)
                    return null;

                var repres = await _contexto.RepresentantesEntidad
                    .Include(r => r.Persona)
                    .Where(r => r.id_entidad == id)
                    .ToListAsync();

                var prs  = await _contexto.Parametros
                    .Where(r => r.grupo == "tipo_representante")
                    .ToListAsync();

                foreach(var r in repres)
                {
                    var tipo = prs.FirstOrDefault(p => p.codigo == r.tipo_representante);
                    
                    entidad.Representantes.Add(new EsqRepresentanteEntidad
                    {
                        IdRepresentanteEntidad = r.id_representante_entidad,
                        TipoRepresentante = tipo?.descripcion ?? "Desconocido",
                        IdPersona = r.id_persona,
                        IdEntidad = r.id_entidad,
                        NombreRepresentante = r.Persona != null ? $"{r.Persona.nombre} {r.Persona.primer_apellido} {r.Persona.segundo_apellido}".Trim() : "Desconocido"
                        
                    });
                }

                return entidad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la entidad con representantes con id {id}", id);
                throw new ApplicationException($"Error al obtener la entidad con representantes con id {id}", ex);
            }
        }
    }
}
