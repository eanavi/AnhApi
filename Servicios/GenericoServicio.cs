using AnhApi.Datos;
using AnhApi.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Add this using statement
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnhApi.Servicios
{
    public class GenericoServicio<TEntity, TId> : IGenericoServicio<TEntity, TId>
        where TEntity : ModeloBase
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger<GenericoServicio<TEntity, TId>> _logger; // Add logger

        public GenericoServicio(AppDbContext context, ILogger<GenericoServicio<TEntity, TId>> logger) // Inject logger
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Initialize logger
        }

        public async Task<IEnumerable<TEntity>> ObtenerTodosAsync()
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
                _logger.LogError(ex, "Error al obtener todos los registros de {EntityType}.", typeof(TEntity).Name);
                throw; // Rethrow the exception after logging, or handle it as appropriate for your application
            }
        }

        public async Task<TEntity?> ObtenerPorIdAsync(TId id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null || entidad.aud_estado != 0)
                {
                    return null;
                }
                return entidad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                throw; // Rethrow or return null/default based on your error handling strategy
            }
        }

        public async Task<TEntity> CrearAsync(TEntity entidad, string usuario, string ip)
        {
            try
            {
                entidad.aud_usuario = usuario;
                entidad.aud_ip = ip;
                entidad.aud_fecha = DateTime.UtcNow;
                entidad.aud_estado = 0;

                _dbSet.Add(entidad);
                await _context.SaveChangesAsync();
                return entidad;
            }
            catch (DbUpdateException dbEx) // Catch specific EF Core exceptions for more detail
            {
                _logger.LogError(dbEx, "DbUpdateError al crear un registro de {EntityType}.", typeof(TEntity).Name);
                throw new ApplicationException($"Error de base de datos al crear {typeof(TEntity).Name}.", dbEx); // Wrap and re-throw custom exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear un registro de {EntityType}.", typeof(TEntity).Name);
                throw new ApplicationException($"Error al crear {typeof(TEntity).Name}.", ex); // Wrap and re-throw custom exception
            }
        }

        public async Task<bool> ActualizarAsync(TId id, TEntity entidad, string usuario, string ip)
        {
            try
            {
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity == null || existingEntity.aud_estado != 0)
                {
                    return false;
                }

                _context.Entry(existingEntity).CurrentValues.SetValues(entidad);

                existingEntity.aud_usuario = usuario;
                existingEntity.aud_ip = ip;
                existingEntity.aud_fecha = DateTime.UtcNow;

                _dbSet.Update(existingEntity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException concEx) // Handle concurrency conflicts
            {
                _logger.LogWarning(concEx, "Conflicto de concurrencia al actualizar {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                return false; // Or throw a specific concurrency exception
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DbUpdateError al actualizar un registro de {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                throw new ApplicationException($"Error de base de datos al actualizar {typeof(TEntity).Name}.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar un registro de {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                throw new ApplicationException($"Error al actualizar {typeof(TEntity).Name}.", ex);
            }
        }

        public async Task<bool> EliminarAsync(TId id, string usuario, string ip)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null) return false;

                entidad.aud_estado = 1;
                entidad.aud_usuario = usuario;
                entidad.aud_ip = ip;
                entidad.aud_fecha = DateTime.UtcNow;

                _dbSet.Update(entidad);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DbUpdateError al eliminar (lógicamente) un registro de {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                throw new ApplicationException($"Error de base de datos al eliminar {typeof(TEntity).Name}.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar (lógicamente) un registro de {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                throw new ApplicationException($"Error al eliminar {typeof(TEntity).Name}.", ex);
            }
        }

        public async Task<TEntity?> EliminarFisicoAsync(TId id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null) return null;

                _dbSet.Remove(entidad);
                await _context.SaveChangesAsync();
                return entidad;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DbUpdateError al eliminar (físicamente) un registro de {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                throw new ApplicationException($"Error de base de datos al eliminar físicamente {typeof(TEntity).Name}.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar (físicamente) un registro de {EntityType} con ID {Id}.", typeof(TEntity).Name, id);
                throw new ApplicationException($"Error al eliminar físicamente {typeof(TEntity).Name}.", ex);
            }
        }
    }
}