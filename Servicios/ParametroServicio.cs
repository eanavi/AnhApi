using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;

using AnhApi.Datos;
using AnhApi.Modelos;


namespace AnhApi.Servicios
{

    /// <summary>
    /// Require un servicio especializado para manejar las operaciones CRUD de la entidad Parametro.
    /// debido a que la entidad Parametro NO hereda de ModeloBase
    /// </summary>
    public class ParametroServicio
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ParametroServicio> _logger;


        //Constructor de la clase
        public ParametroServicio(AppDbContext context, ILogger<ParametroServicio> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Parametro>> ObtenerTodosAsync()
        {
            try
            {
                var query = _context.Set<Parametro>().AsQueryable();
                query = query.Where(e => e.aud_estado == 0);
                var entidades = await query.ToListAsync();
                return entidades;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de Parametro.");
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<Parametro?> ObtenerPorIdAsync(int id)
        {
            try
            {
                var entidad = await _context.Set<Parametro>().FindAsync(id);
                if (entidad == null || entidad.aud_estado != 0)
                {
                    return null;
                }
                return entidad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de Parametro con ID {Id}.", id);
                throw; // Rethrow or handle the exception as appropriate
            }
        }

        public async Task<Parametro> CrearAsync(Parametro entidad, string usuario, string ip)
        {
            try
            {
                entidad.aud_estado = entidad.aud_estado?? 0; // Estado activo
                _context.Set<Parametro>().Add(entidad);
                _logger.LogInformation($"Prametro creado con Id: {entidad.id_parametro}");
                await _context.SaveChangesAsync();
                return entidad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el registro de Parametro.");
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<bool> ActualizarAsync(int id, Parametro entidad, string usuario, string ip)
        {
            try
            {
                var existingEntity = await _context.Parametros.FirstOrDefaultAsync(p => p.id_parametro == id && p.aud_estado == 0);
                if (existingEntity == null)
                {
                    _logger.LogWarning("No se encontró el registro de Parametro con ID {Id} o está eliminado.", id);
                    return false; // No se encontró la entidad o está eliminada
                }
                // Actualizar los campos necesarios
                existingEntity.codigo = entidad.codigo;
                existingEntity.descripcion = entidad.descripcion;
                existingEntity.sigla = entidad.sigla;
                existingEntity.grupo = entidad.grupo;
                existingEntity.aud_estado = entidad.aud_estado;

                _context.Set<Parametro>().Update(existingEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Parametro actualizado con Id: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el registro de Parametro con ID {Id}.", id);
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<bool> EliminarAsync(int id, string usuario, string ip)
        {
            try
            {
                var entidad = await _context.Parametros.FirstOrDefaultAsync(p => p.id_parametro == id && p.aud_estado == 0);
                if (entidad == null)
                {
                    _logger.LogWarning("No se encontró el registro de Parametro con ID {Id} o está eliminado.", id);
                    return false; // No se encontró la entidad o ya está eliminada
                }
                entidad.aud_estado = 1; // Marcar como eliminado
                _context.Set<Parametro>().Update(entidad);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Parametro eliminado con Id: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el registro de Parametro con ID {Id}.", id);
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<bool> EliminarFisico(int id)
        {
            try
            {
                var entidad = await _context.Parametros.FindAsync(id);
                if (entidad == null)
                {
                    _logger.LogWarning("No se encontró el registro de Parametro con ID {Id}.", id);
                    return false; // No se encontró la entidad
                }
                _context.Set<Parametro>().Remove(entidad);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Parametro eliminado físicamente con Id: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar físicamente el registro de Parametro con ID {Id}.", id);
                throw; // Rethrow the exception after logging
            }
        }


    }
}
