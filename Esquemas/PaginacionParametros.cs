using System.ComponentModel.DataAnnotations;

namespace AnhApi.Esquemas
{
    /// <summary>
    /// DTO para parámetros de paginación de la API.
    /// </summary>
    public class PaginacionParametros
    {
        private const int MaximoTamanoPagina = 50; // Límite máximo para el tamaño de la página

        // Propiedad para el número de página actual. Inicia en 1.
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser al menos 1.")]
        public int PaginaNumero { get; set; } = 1;

        // Propiedad para el tamaño de la página (número de elementos por página).
        // Se asegura de que no exceda el máximo permitido.
        private int _tamanoPagina = 10; // Valor predeterminado
        [Range(1, MaximoTamanoPagina, ErrorMessage = "El tamaño de la página debe estar entre 1 y {MaximoTamanoPagina}.")]
        public int TamanoPagina
        {
            get => _tamanoPagina;
            set => _tamanoPagina = (value > MaximoTamanoPagina) ? MaximoTamanoPagina : value;
        }

        /// <summary>
        /// Calcula el número de elementos a omitir (skip).
        /// </summary>
        public int ElementosAOmitir => (PaginaNumero - 1) * TamanoPagina;
    }
}