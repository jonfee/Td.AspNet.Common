using Newtonsoft.Json;
using System;

namespace Td.AspNet.Utils.Json
{
    /// <summary>
    /// 长整型转换
    /// </summary>
    public class Int64Convert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return Type.GetTypeFromHandle(objectType.TypeHandle) == typeof(Int64);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullableType(objectType);

            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!IsNullableType(objectType))
                {
                    throw new Exception(string.Format("不能转换null value to {0}.", objectType));
                }

                return null;
            }

            try
            {
                if (reader.TokenType == JsonToken.String && reader.Value is Int64)
                {
                    return Int64.Parse(reader.Value.ToString());
                }
            }
            catch
            {
                throw new Exception(string.Format("Error converting value {0} to type '{1}'", reader.Value, objectType));
            }
            throw new Exception(string.Format("Unexpected token {0} when parsing enum", reader.TokenType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is Int64)
            {
                writer.WriteValue(value.ToString());
            }
        }

        public bool IsNullableType(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            return (t.FullName == "System.ValueType" && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
