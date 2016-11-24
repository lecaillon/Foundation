using System;
using System.Data;
using System.Globalization;
using System.Text;

namespace Foundation.Storage
{
    /// <summary>
    ///     Logging information about the parameters of a <see cref="DbCommand" /> that is being executed.
    /// </summary>
    public class DbParameterLogData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DbParameterLogData" /> class.
        /// </summary>
        /// <param name="name"> The name of the parameter. </param>
        /// <param name="value"> The value of the parameter. </param>
        /// <param name="hasValue"> A value indicating whether the parameter has a value (or is assigned null). </param>
        /// <param name="direction"> The direction of the parameter. </param>
        /// <param name="dbType"> The type of the parameter. </param>
        /// <param name="nullable"> A value indicating whether the parameter type is nullable. </param>
        /// <param name="size"> The size of the type of the parameter. </param>
        /// <param name="precision"> The precision of the type of the parameter. </param>
        /// <param name="scale"> The scale of the type of the parameter. </param>
        public DbParameterLogData(string name, object value, bool hasValue, ParameterDirection direction, DbType dbType, bool nullable, int size, byte precision, byte scale)
        {
            Name = name;
            Value = value;
            HasValue = hasValue;
            Direction = direction;
            DbType = dbType;
            IsNullable = nullable;
            Size = size;
            Precision = precision;
            Scale = scale;
        }

        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        ///     Gets the value of the parameter.
        /// </summary>
        public virtual object Value { get; }

        /// <summary>
        ///     Gets a value indicating whether the parameter has a value (or is assigned null).
        /// </summary>
        public virtual bool HasValue { get; set; }

        /// <summary>
        ///     Gets the direction of the parameter.
        /// </summary>
        public virtual ParameterDirection Direction { get; set; }

        /// <summary>
        ///     Gets the type of the parameter.
        /// </summary>
        public virtual DbType DbType { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the parameter type is nullable.
        /// </summary>
        public virtual bool IsNullable { get; }

        /// <summary>
        ///     Gets the size of the type of the parameter.
        /// </summary>
        public virtual int Size { get; }

        /// <summary>
        ///     Gets the precision of the type of the parameter.
        /// </summary>
        public virtual byte Precision { get; }

        /// <summary>
        ///     Gets the scale of the type of the parameter.
        /// </summary>
        public virtual byte Scale { get; }

        public string FormatParameter(bool quoteValues = true)
        {
            var builder = new StringBuilder();

            var value = Value;
            var clrType = value?.GetType();

            FormatParameterValue(builder, value, quoteValues);

            if (IsNullable && value != null && !clrType.IsNullableType())
            {
                builder.Append(" (Nullable = true)");
            }
            else
            {
                if (!IsNullable && HasValue && (value == null || clrType.IsNullableType()))
                {
                    builder.Append(" (Nullable = false)");
                }
            }

            if (Size != 0)
            {
                builder.Append(" (Size = ")
                       .Append(Size.ToString(CultureInfo.InvariantCulture))
                       .Append(')');
            }

            if (Precision != 0)
            {
                builder.Append(" (Precision = ")
                       .Append(Precision.ToString(CultureInfo.InvariantCulture))
                       .Append(')');
            }

            if (Scale != 0)
            {
                builder.Append(" (Scale = ")
                       .Append(Scale.ToString(CultureInfo.InvariantCulture))
                       .Append(')');
            }

            if (Direction != ParameterDirection.Input)
            {
                builder.Append(" (Direction = ")
                       .Append(Direction)
                       .Append(')');
            }

            if (HasValue && !IsNormalDbType(DbType, clrType))
            {
                builder.Append(" (DbType = ")
                       .Append(DbType)
                       .Append(')');
            }

            return builder.ToString();
        }

        private static void FormatParameterValue(StringBuilder builder, object parameterValue, bool quoteValues)
        {
            if (quoteValues)
            {
                builder.Append('\'');
            }

            if (parameterValue?.GetType() != typeof(byte[]))
            {
                builder.Append(Convert.ToString(parameterValue, CultureInfo.InvariantCulture));
            }
            else
            {
                var buffer = (byte[])parameterValue;
                builder.Append("0x");

                for (var i = 0; i < buffer.Length; i++)
                {
                    if (i > 31)
                    {
                        builder.Append("...");
                        break;
                    }
                    builder.Append(buffer[i].ToString("X2", CultureInfo.InvariantCulture));
                }
            }

            if (quoteValues)
            {
                builder.Append('\'');
            }
        }

        private static bool IsNormalDbType(DbType dbType, Type clrType)
        {
            if (clrType == null)
            {
                return false;
            }

            clrType = clrType.UnwrapNullableType().UnwrapEnumType();

            switch (dbType)
            {
                case DbType.AnsiString: // Zero
                    return clrType != typeof(string);
                case DbType.Binary:
                    return clrType == typeof(byte[]);
                case DbType.Byte:
                    return clrType == typeof(byte);
                case DbType.Boolean:
                    return clrType == typeof(bool);
                case DbType.Decimal:
                    return clrType == typeof(decimal);
                case DbType.Double:
                    return clrType == typeof(double);
                case DbType.Guid:
                    return clrType == typeof(Guid);
                case DbType.Int16:
                    return clrType == typeof(short);
                case DbType.Int32:
                    return clrType == typeof(int);
                case DbType.Int64:
                    return clrType == typeof(long);
                case DbType.Object:
                    return clrType == typeof(object);
                case DbType.SByte:
                    return clrType == typeof(sbyte);
                case DbType.Single:
                    return clrType == typeof(float);
                case DbType.String:
                    return clrType == typeof(string);
                case DbType.Time:
                    return clrType == typeof(TimeSpan);
                case DbType.UInt16:
                    return clrType == typeof(ushort);
                case DbType.UInt32:
                    return clrType == typeof(uint);
                case DbType.UInt64:
                    return clrType == typeof(ulong);
                case DbType.DateTime2:
                    return clrType == typeof(DateTime);
                case DbType.DateTimeOffset:
                    return clrType == typeof(DateTimeOffset);
                //case DbType.VarNumeric:
                //case DbType.AnsiStringFixedLength:
                //case DbType.StringFixedLength:
                //case DbType.Xml:
                //case DbType.Currency:
                //case DbType.Date:
                //case DbType.DateTime:
                default:
                    return false;
            }
        }
    }
}
