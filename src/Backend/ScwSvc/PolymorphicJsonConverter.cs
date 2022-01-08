using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScwSvc
{
    internal class PolymorphicJsonConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => JsonSerializer.Serialize<object>(writer, value, options);
    }
}
