using AnhApi.Datos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnhApi.Servicios
{
    public class GenericoServicio<TEntity, TDto, TId> : IGenericoServicio<TEntity, TDto, TId>
        where TEntity : class
        where TDto : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IMapper _mapper;

        public GenericoServicio(AppDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TDto>> ObtenerTodosAsync()
        {
            try
            {
                var query = _dbSet.AsQueryable();
                var prop = typeof(TEntity).GetProperty("aud_estado"); // Corregido: solo aud_estado

                if (prop != null)
                {
                    query = query.Where(e => EF.Property<int?>(e, "aud_estado") == 0);
                }

                var entidades = await query.ToListAsync();
                return _mapper.Map<IEnumerable<TDto>>(entidades);
            }
            catch (Exception ex)
            {
                // Opcional: Registrar el error en un logger
                throw new InvalidOperationException("Error al obtener los registros", ex);
            }
        }

        public async Task<TDto?> ObtenerPorIdAsync(TId id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null) return null;

                var prop = entidad.GetType().GetProperty("aud_estado");
                if(prop != null)
                {
                    var estado_registro = prop.GetValue(entidad) as int?;
                    if (estado_registro != 0) return null;
                }

                return _mapper.Map<TDto>(entidad);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener el registro con ID {id}", ex);
            }
        }

        public async Task<TDto> CrearAsync(TDto dto)
        {
            try
            {
                var entidad = _mapper.Map<TEntity>(dto);
                _dbSet.Add(entidad);
                await _context.SaveChangesAsync();
                return _mapper.Map<TDto>(entidad);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al crear el registro", ex);
            }
        }

        public async Task<bool> ActualizarAsync(TDto dto)
        {
            try
            {
                var entidad = _mapper.Map<TEntity>(dto);
                _dbSet.Update(entidad);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al actualizar el registro", ex);
            }
        }

        public async Task<bool> EliminarAsync(TId id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null) return false;

                var prop = entidad.GetType().GetProperty("aud_estado");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(entidad, 1); // Eliminación lógica
                    _dbSet.Update(entidad);
                    return await _context.SaveChangesAsync() > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al realizar eliminación lógica del registro con ID {id}", ex);
            }
        }

        public async Task<TDto?> EliminarFisicoAsync(TId id)
        {
            try
            {
                var entidad = await _dbSet.FindAsync(id);
                if (entidad == null) return null;

                _dbSet.Remove(entidad);
                await _context.SaveChangesAsync();
                return _mapper.Map<TDto>(entidad);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al realizar eliminación física del registro con ID {id}", ex);
            }
        }
    }
}