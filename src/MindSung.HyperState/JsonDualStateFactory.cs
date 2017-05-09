using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Text;

namespace MindSung.HyperState
{
    public class JsonDualStateFactory : DualStateFactoryBase
    {
        public JsonDualStateFactory(ISerializationProvider<string> jsonSerializer)
            : base(jsonSerializer)
        {
        }

        public JsonDualStateFactory(JsonSerializerSettings settings = null)
            : this(new JsonSerializationProvider(settings))
        {
        }

        public JsonDualStateFactory(Action<JsonSerializerSettings> setupAction)
            : this(new JsonSerializationProvider(setupAction))
        {
        }

        protected override IDualState<TObject, string> CreateDualState<TObject>(ISerializationProvider<string> serializer)
        {
            return new JsonDualState<TObject>(Serializer);
        }

        public new JsonDualState<TObject> FromObject<TObject>(TObject obj)
        {
            return (JsonDualState<TObject>)base.FromObject<TObject>(obj);
        }

        public new JsonDualState<TObject> FromSerialized<TObject>(string serialized)
        {
            return (JsonDualState<TObject>)base.FromSerialized<TObject>(serialized);
        }

        public override Type GetDualStateObjectType<TObject>()
        {
            return typeof(JsonDualState<TObject>);
        }

        public override IReadOnlyList<IDualState<TObject, string>> FromSerializedCollection<TObject>(string serializedCollection)
        {
            if (string.IsNullOrWhiteSpace(serializedCollection))
            {
                return new List<JsonDualState<TObject>>();
            }
            serializedCollection = serializedCollection.Trim();
            if (!serializedCollection.StartsWith("[") || !serializedCollection.EndsWith("]"))
            {
                // Assume a single object or value.
                return new List<JsonDualState<TObject>>() { FromSerialized<TObject>(serializedCollection) };
            }
            var list = new List<JsonDualState<TObject>>();
            var serializedObject = new StringBuilder();
            var depth = 0;
            bool inQuote = false;
            char quoteChar = '\"';
            Action tryAddObject = () =>
            {
                var serialized = serializedObject.ToString();
                if (!string.IsNullOrWhiteSpace(serialized))
                {
                    list.Add(FromSerialized<TObject>(serialized));
                }
                serializedObject.Clear();
            };
            for (var i = 1; i < serializedCollection.Length - 1; i++)
            {
                var c = serializedCollection[i];
                if (depth == 0 && c == ',')
                {
                    tryAddObject();
                }
                else
                {
                    serializedObject.Append(c);
                    if (!inQuote && c == '{')
                    {
                        depth++;
                    }
                    else if (!inQuote && c == '}')
                    {
                        depth--;
                    }
                    else
                    {
                        if (!inQuote && c == '\"' || c == '\'')
                        {
                            quoteChar = c;
                            inQuote = true;
                        }
                        else if (inQuote && c == quoteChar)
                        {
                            inQuote = false;
                        }
                    }
                }
            }
            tryAddObject();
            return list;
        }

        public override string ToSerializedCollection<TObject>(IEnumerable<IDualState<TObject, string>> collection)
        {
            if (collection == null)
            {
                return "[]";
            }
            // This implementation is faster than prettier solutions, such as string interpolation and LINQ.
            // Don't re-implement to something more appealing without verifying performance.
            var serializedCollection = new StringBuilder();
            var first = true;
            serializedCollection.Append('[');
            foreach (var proxy in collection)
            {
                if (!first) { serializedCollection.Append(','); }
                else { first = false; }
                serializedCollection.Append(proxy.Serialized);
            }
            serializedCollection.Append(']');
            return serializedCollection.ToString();
        }
    }
}
