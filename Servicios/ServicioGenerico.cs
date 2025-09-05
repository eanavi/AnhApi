using AnhApi.Datos;
using AnhApi.Esquemas;
using AnhApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnhApi.Servicios
{
    public class ServicioGenerico<T, TKey> : IServicioGenerico<T, TKey> where T : class, ICampoEstado
    {
        private readonly ContextoAppBD _contextoBD;
        private readonly ILogger<ServicioGenerico<T, TKey>> _logger;
        private readonly DbSet<T> _dbSet;

        #region Constructor
        public ServicioGenerico(ContextoAppBD contextoBd, ILogger<ServicioGenerico<T, TKey>> logger)
        {
            _contextoBD = contextoBd ?? throw new ArgumentNullException(nameof(contextoBd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = _contextoBD.Set<T>();
        }
        #endregion Constructor


        #region Metodos
        public async Task<bool> ActualizarAsync(TKey id, T entidad)
        {
            try
            {
                var regExistente = await _dbSet.FindAsync(id);
                if (regExistente == null || regExistente.aud_estado != 0)
                {
                    _logger.LogWarning($"No se puede actualizar un registro de tipo {typeof(T).Name} con id {id} no encontrado o registro eliminado");
                    return false;
                }

                _contextoBD.Entry(regExistente).State = EntityState.Detached;

                var propId = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("id");
                if (propId != null && propId.CanWrite)
                {
                    propId.SetValue(regExistente, id);
                }
                else
                {
                    _logger.LogWarning($"La entidad {typeof(T).Name} no tiene una propiedad id");
                }

                _dbSet.Update(regExistente);
                await _contextoBD.SaveChangesAsync();
                return true;

            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error de concurrencia con el registro {typeof(T).Name} con id {id}");
                throw new ApplicationException($"Error al actualizar {typeof(T).Name}", ex);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error inesperado al actualizar registro de tipo {typeof(T).Name} con id {id}");
                throw;
            }
        }

        public async Task<T> CrearAsync(T entidad)
        {
            try
            {
                entidad.aud_estado = 0;
                _dbSet.Add(entidad);
                await _contextoBD.SaveChangesAsync();
                return entidad;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error de base de datos al crear registro de tipo {typeof(T).Name}");
                throw new ApplicationException($"Error de base de datos al crear registro de tipo {typeof(T).Name},", ex);
            }
        }

        public async Task<bool> EliminarAsync(TKey id)
        {
            try
            {
                var regExistente = await _dbSet.FindAsync(id);
                if (regExistente == null || regExistente.aud_estado != 0)
                {
                    _logger.LogWarning($"No se puede eliminar de forma lógica {typeof(T).Name} con id {id}. No encontrado");
                    return false;
                }

                regExistente.aud_estado = 1;//Marca como eliminado de forma logica
                _dbSet.Update(regExistente);
                await _contextoBD.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar logicamente el registro del tipo {typeof(T).Name} con id {id}");
                throw;
            }
        }

        public async Task<T?> EliminarFisicoAsync(TKey id)
        {
            try
            {
                var regExistente = await _dbSet.FindAsync(id);
                if (regExistente == null)
                {
                    _logger.LogWarning($"No se puede eliminar fisicamente la entidad de tipo {typeof(T).Name} con id {id}. No encontrado");
                    return null;
                }

                _dbSet.Remove(regExistente);
                await _contextoBD.SaveChangesAsync();
                _logger.LogInformation($"Entidad de tipo {typeof(T).Name} con id {id} Eliminada fisicamete");
                return regExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar fisicamente registro {typeof(T).Name} con id {id}");
                throw;
            }
        }

        public async Task<T> ObtenerPorIdAsync(TKey id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);

                if (entidad == null)
                {
                    _logger.LogWarning($"Registro de tipo {typeof(T).Name} con id {id} no encontrado");
                    throw null!;
                }

                if (entidad.aud_estado != 0)
                {
                    _logger.LogWarning($"Entidad de tipo {typeof(T).Name} con id {id} con estado no vigente");
                    throw null!;
                }

                return entidad;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener registro de tipo {typeof(T).Name} con id {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            try
            {
                var query = _dbSet.AsQueryable();
                query = query.Where(e => e.aud_estado == 0);
                var entidades = await query.ToListAsync();
                return entidades;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas los registros de tipo {EntityType}.", typeof(T).Name);
                throw;
            }
        }

        public async Task<PaginacionResultado<T>> ObtenerTodosPagAsync(PaginacionParametros parametrosPag)
        {
            try
            {
                if (parametrosPag == null)
                {
                    parametrosPag = new PaginacionParametros();
                }

                IQueryable<T> query = _dbSet.AsQueryable();
                query = query.Where(e => e.aud_estado == 0);
                var totalReg = await query.CountAsync();

                var resultado = await query
                    .Skip(parametrosPag.ElementosAOmitir)
                    .Take(parametrosPag.TamanoPagina)
                    .ToListAsync();

                return new PaginacionResultado<T>
                {
                    Elementos = resultado,
                    TotalRegistros = totalReg,
                    PaginaActual = parametrosPag.PaginaNumero,
                    TotalPaginas = (int)Math.Ceiling((double)totalReg / parametrosPag.TamanoPagina)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listado de registros de tipo {EntityType}", typeof(T).Name);
                throw;
            }
        }

        #endregion Metodos


    }
}
