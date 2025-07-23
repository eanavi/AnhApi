using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;

using AnhApi.Datos;
using AnhApi.Modelos;
using AnhApi.Esquemas;


namespace AnhApi.Servicios
{

    /// <summary>
    /// Require un servicio especializado para manejar las operaciones CRUD de la entidad Parametro.
    /// debido a que la entidad Parametro NO hereda de ModeloBase por lo que no tiene los campos de
    /// aud_usuario, aud_ip, aud_fecha, solo cuenta con aud_estado para la eliminacion logica 
    /// 
    /// Se debe consultar las opciones de loggin para ver que informacion es relevante 
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

        /// <summary>
        /// Listado general de todos los archivos
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Parametro>> ObtenerTodosAsync()
        {
            try
            {
                var query = _context.Set<Parametro>().AsQueryable();
                query = query.Where(e => e.aud_estado == 0);
                var paramExistentes = await query.ToListAsync();
                return paramExistentes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de Parametro.");
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Obtener un registro en particular para lo cual se debe conocer el nombre
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Parametro?> ObtenerPorIdAsync(int id)
        {
            try
            {
                var paramExistente = await _context.Set<Parametro>().FindAsync(id);
                if (paramExistente == null || paramExistente.aud_estado != 0)
                {
                    return null;
                }
                return paramExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el registro de Parametro con ID {Id}.", id);
                throw; // Rethrow or handle the exception as appropriate
            }
        }

        /// <summary>
        /// Creacion de un registro de parametro
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        public async Task<Parametro> CrearAsync(Parametro entidad)
        {
            try
            {
                _context.Set<Parametro>().Add(entidad);
                //_logger.LogInformation($"Prametro creado con Id: {entidad.id_parametro}");
                await _context.SaveChangesAsync();
                return entidad;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el registro de Parametro.");
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Actualizacion de un registro de parametro
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entidad"></param>
        /// <returns></returns>
        public async Task<bool> ActualizarAsync(int id, Parametro entidad)
        {
            try
            {
                var paramExistente = await _context.Parametros.FirstOrDefaultAsync(
                    p => p.id_parametro == id && p.aud_estado == 0);
                if (paramExistente == null)
                {
                    _logger.LogWarning("No se encontró el registro de Parametro con ID {Id} o está eliminado.", id);
                    return false; // No se encontró la entidad o está eliminada
                }
                // Actualizar los campos necesarios
                paramExistente.codigo = entidad.codigo;
                paramExistente.descripcion = entidad.descripcion;
                paramExistente.sigla = entidad.sigla;
                paramExistente.grupo = entidad.grupo;
                paramExistente.aud_estado = entidad.aud_estado;

                _context.Set<Parametro>().Update(paramExistente);
                await _context.SaveChangesAsync();
                //_logger.LogInformation($"Parametro actualizado con Id: {id}"); 
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el registro de Parametro con ID {Id}.", id);
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Eliminacion logica de un registro de parametro
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var paramExistente = await _context.Parametros.FirstOrDefaultAsync(
                    p => p.id_parametro == id && p.aud_estado == 0);
                if (paramExistente == null)
                {
                    _logger.LogWarning("No se encontró el registro de Parametro con ID {Id} o está eliminado.", id);
                    return false; // No se encontró la entidad o ya está eliminada
                }
                paramExistente.aud_estado = 1; // Marcar como eliminado
                _context.Set<Parametro>().Update(paramExistente);
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

        /// <summary>
        /// Eliminacion fisica de un registro debe realizarse con cuidado para 
        /// no dejar registros huerfanos en la base de datos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> EliminarFisico(int id)
        {
            try
            {
                var paramExistente = await _context.Parametros.FindAsync(id);
                if (paramExistente == null)
                {
                    _logger.LogWarning("No se encontró el registro de Parametro con ID {Id}.", id);
                    return false; // No se encontró la entidad
                }
                _context.Set<Parametro>().Remove(paramExistente);
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


        /// <summary>
        /// Despliega en una lista los miembros de un grupo
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Parametro>> ObtenerGrupo(string grupo)
        {
            try
            {
                var query = _context.Set<Parametro>().AsQueryable();
                query = query.Where(e => e.grupo == grupo && e.aud_estado == 0);
                var paramExistentes = await query.ToListAsync();
                return paramExistentes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar el grupo {grupo},", grupo);
                throw;
            }
        }
    }
}
