using AutoMapper;
using System.Text.Json;

namespace AnhApi.Mapeos
{
    // Cambia ITypeConverter por IValueConverter
    public class ObjectToJsonDocumentConverter : IValueConverter<object, JsonDocument?>
    {
        public JsonDocument? Convert(object sourceMember, ResolutionContext context)
        {
            if (sourceMember == null || sourceMember.ToString() == "null")
            {
                return null;
            }

            try
            {
                string jsonString = JsonSerializer.Serialize(sourceMember);
                return JsonDocument.Parse(jsonString);
            }
            catch (JsonException)
            {
                // Maneja la excepción si el JSON no es válido
                return null;
            }
        }
    }
}