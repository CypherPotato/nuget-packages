using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Json
{
    /// <summary>
    /// Provides functionality to serialize and deserialize JSON datagrams to anonymous or typed objects.
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// Gets a shared, thread-safe <see cref="JsonUtil"/> object for JSON encoding.
        /// </summary>
        public static JsonUtil Shared { get; } = new JsonUtil();

        /// <summary>
        /// Gets or sets the JSON options to be used with this JSON serializer.
        /// </summary>
        public JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions();

        /// <summary>
        /// Serializes an object into an JSON string with given options.
        /// </summary>
        /// <param name="obj">The object which will be serialized into an JSON string.</param>
        /// <param name="options">Parameters to using in the serialization.</param>
        /// <returns>The JSON string representation of the object.</returns>
        public string Serialize(object? obj, JsonSerializerOptions options) => JsonSerializer.Serialize(obj, options);

        /// <summary>
        /// Serializes an object into an JSON string.
        /// </summary>
        /// <param name="obj">The object which will be serialized into an JSON string.</param>
        /// <returns>The JSON string representation of the object.</returns>
        public string Serialize(object? obj) => Serialize(obj, this.Options);

        /// <summary>
        /// Deserializes an JSON string into an anonymous object with given decoding options.
        /// </summary>
        /// <param name="json">The input JSON string.</param>
        /// <param name="options">Parameters to using in the decoding.</param>
        /// <returns>An dynamic object with JSON contents.</returns>
        public object? Deserialize(string json, JsonSerializerOptions options)
        {
            JsonElement? rootObject = JsonSerializer.Deserialize<JsonElement>(json, options);
            if (rootObject == null) return null;
            return DecodeInternal(rootObject.Value);
        }

        /// <summary>
        /// Deserializes an JSON string into an anonymous object.
        /// </summary>
        /// <param name="json">The input JSON string.</param>
        /// <returns>An dynamic object with JSON contents.</returns>
        public object? Deserialize(string json) => Deserialize(json, this.Options);

        /// <summary>
        /// Parses the text representing a single JSON value into a <typeparamref name="T"/> with given JSON options.
        /// </summary>
        /// <returns>A <typeparamref name="T"/> representation of the JSON value.</returns>
        /// <param name="json">JSON text to parse.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        public T? Deserialize<T>(string json, JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(json, options);

        /// <summary>
        /// Parses the text representing a single JSON value into a <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A <typeparamref name="T"/> representation of the JSON value.</returns>
        /// <param name="json">JSON text to parse.</param>
        public T? Deserialize<T>(string json) => Deserialize<T>(json, this.Options);

        private object? DecodeInternal(JsonElement node)
        {
            switch (node.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        var exn = new ExpandoObject();
                        var exp = node.Deserialize<ExpandoObject>()!;
                        foreach (var expNode in exp)
                        {
                            if (expNode.Value == null)
                            {
                                exn.TryAdd(expNode.Key, null);
                            }
                            else
                            {
                                exn.TryAdd(expNode.Key, DecodeInternal((JsonElement)expNode.Value));
                            }
                        }
                        return exn;
                    }
                case JsonValueKind.Array:
                    {
                        List<object?> exarr = new List<object?>();
                        var exp = node.Deserialize<JsonElement[]>()!;
                        foreach (var expNode in exp)
                        {
                            exarr.Add(DecodeInternal(expNode));
                        }
                        return exarr.ToArray();
                    }
                case JsonValueKind.String:
                    return node.GetString();
                case JsonValueKind.Number:
                    return node.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default: // JsonValueKind.Null:
                    return null;
            }
        }
    }
}
