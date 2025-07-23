using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnhApi.Datos;
using AnhApi.Interfaces;
using AnhApi.Modelos;
namespace AnhApi.Servicios
{
    public class GenericoServicio<T, TKey> : IGenericoServicio<T, TKey> where T:class, IAuditable   
    {
        private readonly AppDbContext _contexto;
        private readonly ILogger<GenericoServicio<T, TKey>> _logger;
        private readonly DbSet<T> _dbSet;

        public GenericoServicio(AppDbContext contexto, ILogger<GenericoServicio<T, TKey>> logger)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = _contexto.Set<T>();
        }

        public async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            try
            {
                IQueryable<T> query = _dbSet;
                query = query.Where(e => e.aud_estado == 0);
                var resultado = await query.ToListAsync();

                return resultado;
        
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar entidades de tipo {EntityType}.", typeof(T).Name);
                throw; // Puedes lanzar excepción personalizada si lo deseas
            }
        }

        public async Task<T> ObtenerPorIdAsync(TKey id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);

                if (entidad == null)
                {
                    _logger.LogWarning($"Entidad de tipo {typeof(T).Name} con ID {id} no encontrada.");
                    throw null!; // Devuelve null para indicar que no se encontró (o podrías lanzar NotFoundException)
                }

                // Si es auditable, solo devuelve si el estado es 0 (activo)
                if (entidad.aud_estado != 0)
                {
                    _logger.LogWarning($"Entidad de tipo {typeof(T).Name} con ID {id} encontrada pero con estado {entidad.aud_estado} (no activo).");
                    throw null!; // Devuelve null si no está activo
                }

                return entidad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener entidad de tipo {typeof(T).Name} con ID {id}.");
                throw;
            }
        }

        public async Task<T> CrearAsync(T entidad, string usuario, string ip)
        {
            try
            {
                // Establece los campos de auditoría al crear la entidad
                entidad.aud_estado = 0; // Por defecto, activo
                entidad.aud_usuario = usuario;
                entidad.aud_ip = ip;
                entidad.aud_fecha = DateTime.UtcNow;

                _dbSet.Add(entidad);
                await _contexto.SaveChangesAsync();
                return entidad;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error de base de datos al crear entidad de tipo {typeof(T).Name}.");
                throw new ApplicationException($"Error de base de datos al crear entidad de tipo {typeof(T).Name}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al crear entidad de tipo {typeof(T).Name}.");
                throw;
            }
        }

        public async Task<bool> ActualizarAsync(TKey id, T entidad, string usuario, string ip)
        {
            try
            {
                var entidadExistente = await _dbSet.FindAsync(id);
                if (entidadExistente == null || entidadExistente.aud_estado != 0)
                {
                    _logger.LogWarning($"No se puede actualizar la entidad de tipo {typeof(T).Name} con ID {id}. No encontrada o no activa.");
                    return false;
                }

                // Detach la entidad existente para evitar problemas de seguimiento si se pasa la misma entidad de DTO/modelo.
                _contexto.Entry(entidadExistente).State = EntityState.Detached;


                // Una forma genérica de asignar el ID si tu modelo tiene una propiedad llamada "Id" o "id"
                var idProperty = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("id");
                if (idProperty != null && idProperty.CanWrite)
                {
                    idProperty.SetValue(entidad, id);
                }
                else
                {
                    _logger.LogWarning($"La entidad {typeof(T).Name} no tiene una propiedad 'Id' o 'id' para establecer el ID de actualización. Podría haber problemas con el seguimiento de EF.");
                    // Si tus claves primarias no son nombradas consistentemente, esto es un punto de dolor para genéricos.
                    // Para evitar el error "Cannot attach an entity that already exists" es mejor usar Update.
                }

                // Actualiza los campos de auditoría al modificar
                entidad.aud_usuario = usuario;
                entidad.aud_ip = ip;
                entidad.aud_fecha = DateTime.UtcNow; // Fecha de la última modificación

                _dbSet.Update(entidad); // Update instruye a EF para que marque la entidad como modificada
                await _contexto.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error de concurrencia al actualizar entidad de tipo {typeof(T).Name} con ID {id}.");
                // Podrías intentar recargar y reintentar si es apropiado, o manejar la colisión
                throw new ApplicationException($"Error de concurrencia al actualizar entidad {typeof(T).Name}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inesperado al actualizar entidad de tipo {typeof(T).Name} con ID {id}.");
                throw;
            }
        }

        // --- 5. EliminarAsync (Eliminado Lógico) ---
        public async Task<bool> EliminarAsync(TKey id, string usuario, string ip)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null || entidad.aud_estado != 0)
                {
                    _logger.LogWarning($"No se puede eliminar lógicamente la entidad de tipo {typeof(T).Name} con ID {id}. No encontrada o ya inactiva.");
                    return false;
                }

                entidad.aud_estado = 1; // Marca como eliminado lógicamente
                entidad.aud_usuario = usuario;
                entidad.aud_ip = ip;
                entidad.aud_fecha = DateTime.UtcNow; // Fecha de la eliminación lógica

                _dbSet.Update(entidad); // Marca la entidad como modificada
                await _contexto.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar lógicamente entidad de tipo {typeof(T).Name} con ID {id}.");
                throw;
            }
        }


        public async Task<T?> EliminarFisicoAsync(TKey id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null)
                {
                    _logger.LogWarning($"No se puede eliminar físicamente la entidad de tipo {typeof(T).Name} con ID {id}. No encontrada.");
                    return null;
                }

                _dbSet.Remove(entidad); // Elimina físicamente
                await _contexto.SaveChangesAsync();
                _logger.LogInformation($"Entidad de tipo {typeof(T).Name} con ID {id} eliminada físicamente.");
                return entidad; // Retorna la entidad eliminada
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar físicamente entidad de tipo {typeof(T).Name} con ID {id}.");
                throw;
            }
        }

    }
}
