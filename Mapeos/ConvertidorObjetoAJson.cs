using System.Text.Json;
using AutoMapper;

namespace AnhApi.Mapeos
{
    public class ConvertidorObjetoAJson : ITypeConverter<object, JsonDocument?>
    {
        public JsonDocument? Convert(object source, JsonDocument? destination, ResolutionContext context)
        {
            // Si el objeto de origen es nulo o un string "null", devuelve null
            if (source == null || source.ToString() == "null")
            {
                return null;
            }

            // Intenta serializar el objeto y luego analizarlo como JSON
            try
            {
                // Primero, serializa el objeto de origen a un string JSON.
                // Esto es necesario porque el object podría ser un JsonElement, JToken, etc.
                string jsonString = JsonSerializer.Serialize(source);

                // Luego, analiza el string JSON en un JsonDocument
                return JsonDocument.Parse(jsonString);
            }
            catch (JsonException)
            {
                // Si la serialización o el análisis fallan, devuelve null.
                // Podrías considerar registrar este error para depurar.
                return null;
            }
        }
    }
}