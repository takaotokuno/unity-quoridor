using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Quoridor
{
    public static class LogFormatter
    {
        public static string Format(string label, object value)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                label = "Log";
            }

            if (value == null)
            {
                return $"[{label}] null";
            }

            Type type = value.GetType();

            PropertyInfo[] properties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetIndexParameters().Length == 0)
                .ToArray();

            if (properties.Length == 0)
            {
                return $"[{label}] {type.Name}";
            }

            var builder = new StringBuilder();
            builder.Append('[');
            builder.Append(label);
            builder.Append("] ");
            builder.Append(type.Name);
            builder.Append(": ");

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                object propertyValue;

                try
                {
                    propertyValue = property.GetValue(value);
                }
                catch (Exception ex)
                {
                    propertyValue = $"<Failed to read: {ex.GetType().Name}>";
                }

                builder.Append(property.Name);
                builder.Append('=');
                builder.Append(FormatValue(propertyValue));

                if (i < properties.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string s)
            {
                return $"\"{s}\"";
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var values = enumerable
                    .Cast<object>()
                    .Select(FormatValue);

                return "[" + string.Join(", ", values) + "]";
            }

            return value.ToString();
        }
    }
}