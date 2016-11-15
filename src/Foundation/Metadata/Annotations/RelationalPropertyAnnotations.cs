using System;
using System.Globalization;
using System.Reflection;
using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    public class RelationalPropertyAnnotations : IRelationalPropertyAnnotations
    {
        public RelationalPropertyAnnotations(Property property)
        {
            Check.NotNull(property, nameof(property));

            Property = property;
        }

        protected virtual Property Property { get; }

        protected virtual Annotable Metadata => Property;

        protected virtual bool ShouldThrowOnConflict => true;

        public virtual string ColumnName
        {
            get { return (string)Metadata[RelationalAnnotationNames.ColumnName] ?? Property.Name; }
            set { Metadata[RelationalAnnotationNames.ColumnName] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        public virtual string ColumnType
        {
            get { return (string)Metadata[RelationalAnnotationNames.ColumnType]; }
            set { Metadata[RelationalAnnotationNames.ColumnType] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        #region DefaultValueSql

        public virtual string DefaultValueSql
        {
            get { return (string)Metadata[RelationalAnnotationNames.DefaultValueSql]; }
            set { SetDefaultValueSql(value); }
        }

        protected virtual bool SetDefaultValueSql(string value)
        {
            if (!CanSetDefaultValueSql(value))
            {
                return false;
            }

            if (!ShouldThrowOnConflict && DefaultValueSql != value && value != null)
            {
                ClearAllServerGeneratedValues();
            }

            Metadata[RelationalAnnotationNames.DefaultValueSql] = Check.NullButNotEmpty(value, nameof(value));

            return true;
        }

        protected virtual bool CanSetDefaultValueSql(string value)
        {
            if (DefaultValueSql == value)
            {
                return true;
            }

            if (ShouldThrowOnConflict)
            {
                if (DefaultValue != null)
                {
                    throw new InvalidOperationException(ResX.ConflictingColumnServerGeneration(nameof(DefaultValueSql), Property.Name, nameof(DefaultValue)));
                }

                if (ComputedColumnSql != null)
                {
                    throw new InvalidOperationException(ResX.ConflictingColumnServerGeneration(nameof(DefaultValueSql), Property.Name, nameof(ComputedColumnSql)));
                }
            }
            else if (value != null && (!CanSetDefaultValue(null) || !CanSetComputedColumnSql(null)))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region ComputedColumnSql

        public virtual string ComputedColumnSql
        {
            get { return (string)Metadata[RelationalAnnotationNames.ComputedColumnSql]; }
            set { SetComputedColumnSql(value); }
        }

        protected virtual bool SetComputedColumnSql(string value)
        {
            if (!CanSetComputedColumnSql(value))
            {
                return false;
            }

            if (!ShouldThrowOnConflict && ComputedColumnSql != value && value != null)
            {
                ClearAllServerGeneratedValues();
            }

            Metadata[RelationalAnnotationNames.ComputedColumnSql] = Check.NullButNotEmpty(value, nameof(value));

            return true;
        }

        protected virtual bool CanSetComputedColumnSql(string value)
        {
            if (ComputedColumnSql == value)
            {
                return true;
            }

            if (ShouldThrowOnConflict)
            {
                if (DefaultValue != null)
                {
                    throw new InvalidOperationException(ResX.ConflictingColumnServerGeneration(nameof(ComputedColumnSql), Property.Name, nameof(DefaultValue)));
                }
                if (DefaultValueSql != null)
                {
                    throw new InvalidOperationException(ResX.ConflictingColumnServerGeneration(nameof(ComputedColumnSql), Property.Name, nameof(DefaultValueSql)));
                }
            }
            else if (value != null && (!CanSetDefaultValue(null) || !CanSetDefaultValueSql(null)))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region DefaultValue

        public virtual object DefaultValue
        {
            get { return (string)Metadata[RelationalAnnotationNames.DefaultValue]; }
            set { SetDefaultValue(value); }
        }

        protected virtual bool SetDefaultValue(object value)
        {
            if (value != null)
            {
                var valueType = value.GetType();
                if (Property.ClrType.UnwrapNullableType() != valueType)
                {
                    try
                    {
                        value = Convert.ChangeType(value, Property.ClrType, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException(ResX.IncorrectDefaultValueType(value, valueType, Property.Name, Property.ClrType, Property.DeclaringEntity.Name));
                    }
                }

                if (valueType.GetTypeInfo().IsEnum)
                {
                    value = Convert.ChangeType(value, valueType.UnwrapEnumType(), CultureInfo.InvariantCulture);
                }
            }

            if (!ShouldThrowOnConflict && DefaultValue != value && value != null)
            {
                ClearAllServerGeneratedValues();
            }

            Metadata[RelationalAnnotationNames.DefaultValue] = value;

            return true;
        }

        protected virtual bool CanSetDefaultValue(object value)
        {
            if (DefaultValue == value)
            {
                return true;
            }

            if (ShouldThrowOnConflict)
            {
                if (DefaultValueSql != null)
                {
                    throw new InvalidOperationException(ResX.ConflictingColumnServerGeneration(nameof(DefaultValue), Property.Name, nameof(DefaultValueSql)));
                }
                if (ComputedColumnSql != null)
                {
                    throw new InvalidOperationException(ResX.ConflictingColumnServerGeneration(nameof(DefaultValue), Property.Name, nameof(ComputedColumnSql)));
                }
            }
            else if (value != null && (!CanSetDefaultValueSql(null) || !CanSetComputedColumnSql(null)))
            {
                return false;
            }

            return true;
        }

        #endregion

        protected virtual void ClearAllServerGeneratedValues()
        {
            SetDefaultValue(null);
            SetDefaultValueSql(null);
            SetComputedColumnSql(null);
        }
    }
}
