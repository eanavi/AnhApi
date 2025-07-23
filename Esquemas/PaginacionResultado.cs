// Archivo: AnhApi.Esquemas/PaginacionResultado.cs
using System.Collections.Generic;

namespace AnhApi.Esquemas
{
    /// <summary>
    /// Representa un resultado paginado de una colección de elementos.
    /// </summary>
    /// <typeparam name="T">El tipo de los elementos en la colección.</typeparam>
    public class PaginacionResultado<T>
    {
        public IEnumerable<T> Elementos { get; set; } = new List<T>();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalPaginas { get; set; }
    }
}