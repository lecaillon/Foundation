﻿using System.Linq;
using System.Text;
using Foundation.Metadata;
using Foundation.Utilities;

namespace Foundation
{
    public static class MetadataExtensions
    {
        public static string ToDebugString(this Model model, string indent = "")
        {
            var builder = new StringBuilder();

            builder.Append(indent).Append("Model: ");

            foreach (var entity in model.GetEntities())
            {
                builder.AppendLine().Append(entity.ToDebugString(false, indent + "  "));
            }

            return builder.ToString();
        }

        #region Entity

        public static string ToDebugString(this Entity entity, bool singleLine = true, string indent = "")
        {
            var builder = new StringBuilder();

            builder.Append(indent).Append("Entity: ").Append(entity.Name);

            if (entity.BaseType != null)
            {
                builder.Append(" Base: ").Append(entity.BaseType.Name);
            }

            if (entity.IsAbstract)
            {
                builder.Append(" Abstract");
            }

            //if (!singleLine)
            //{
            //    var properties = entity.GetDeclaredProperties().ToList();
            //    if (properties.Count != 0)
            //    {
            //        builder.AppendLine().Append(indent).Append("  Properties: ");
            //        foreach (var property in properties)
            //        {
            //            builder.AppendLine().Append(property.ToDebugString(false, indent + "    "));
            //        }
            //    }

            //    var navigations = entity.GetDeclaredNavigations().ToList();
            //    if (navigations.Count != 0)
            //    {
            //        builder.AppendLine().Append(indent).Append("  Navigations: ");
            //        foreach (var navigation in navigations)
            //        {
            //            builder.AppendLine().Append(navigation.ToDebugString(false, indent + "    "));
            //        }
            //    }

            //    var keys = entity.GetDeclaredKeys().ToList();
            //    if (keys.Count != 0)
            //    {
            //        builder.AppendLine().Append(indent).Append("  Keys: ");
            //        foreach (var key in keys)
            //        {
            //            builder.AppendLine().Append(key.ToDebugString(false, indent + "    "));
            //        }
            //    }

            //    var fks = entity.GetDeclaredForeignKeys().ToList();
            //    if (fks.Count != 0)
            //    {
            //        builder.AppendLine().Append(indent).Append("  Foreign keys: ");
            //        foreach (var fk in fks)
            //        {
            //            builder.AppendLine().Append(fk.ToDebugString(false, indent + "    "));
            //        }
            //    }

            //    builder.Append(entity.AnnotationsToDebugString(indent + "  "));
            //}

            return builder.ToString();
        }

        /// <summary>
        ///     Determines if an entity derives from (or is the same as) a given entity.
        /// </summary>
        /// <param name="entity"> The base entity type. </param>
        /// <param name="derived"> The entity to check if it derives from <paramref name="entity" />. </param>
        /// <returns>
        ///     True if <paramref name="derived" /> derives from (or is the same as) <paramref name="entity" />, otherwise false.
        /// </returns>
        public static bool IsAssignableFrom(this Entity entity, Entity derived)
        {
            Check.NotNull(entity, nameof(entity));
            Check.NotNull(derived, nameof(derived));

            var baseType = derived;
            while (baseType != null)
            {
                if (baseType == entity)
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }

        #endregion

        public static string ToDebugString(this Property property, bool singleLine = true, string indent = "")
        {
            var builder = new StringBuilder();

            builder.Append(indent);

            if (singleLine)
            {
                builder.Append("Property: ").Append(property.DeclaringEntity.Name).Append(".");
            }

            builder.Append(property.Name).Append(" (");

            //var field = property.GetField();
            //if (field == null)
            //{
            //    builder.Append("no field, ");
            //}
            //else if (!field.EndsWith(">k__BackingField"))
            //{
            //    builder.Append(field).Append(", ");
            //}

            builder.Append(property.ClrType.ShortDisplayName()).Append(")");

            if (property.IsShadowProperty)
            {
                builder.Append(" Shadow");
            }

            if (!property.IsNullable)
            {
                builder.Append(" Required");
            }

            if (property.PrimaryKey != null)
            {
                builder.Append(" PK");
            }

            if (property.ForeignKeys != null)
            {
                builder.Append(" FK");
            }

            if (property.Keys != null && property.PrimaryKey == null)
            {
                builder.Append(" AlternateKey");
            }

            if (property.Indexes != null)
            {
                builder.Append(" Index");
            }

            //if (property.IsConcurrencyToken)
            //{
            //    builder.Append(" Concurrency");
            //}

            //if (property.IsReadOnlyAfterSave)
            //{
            //    builder.Append(" ReadOnlyAfterSave");
            //}

            //if (property.IsReadOnlyBeforeSave)
            //{
            //    builder.Append(" ReadOnlyBeforeSave");
            //}

            //if (property.RequiresValueGenerator)
            //{
            //    builder.Append(" RequiresValueGenerator");
            //}

            //if (property.ValueGenerated != ValueGenerated.Never)
            //{
            //    builder.Append(" ValueGenerated.").Append(property.ValueGenerated);
            //}

            //if (property.IsStoreGeneratedAlways)
            //{
            //    builder.Append(" StoreGeneratedAlways");
            //}

            //if (property.GetMaxLength() != null)
            //{
            //    builder.Append(" MaxLength").Append(property.GetMaxLength());
            //}

            //if (property.IsUnicode() == false)
            //{
            //    builder.Append(" Ansi");
            //}

            //if (property.GetPropertyAccessMode() != null)
            //{
            //    builder.Append(" PropertyAccessMode.").Append(property.GetPropertyAccessMode());
            //}

            //var indexes = property.GetPropertyIndexes();
            //builder.Append(" ").Append(indexes.Index);
            //builder.Append(" ").Append(indexes.OriginalValueIndex);
            //builder.Append(" ").Append(indexes.RelationshipIndex);
            //builder.Append(" ").Append(indexes.ShadowIndex);
            //builder.Append(" ").Append(indexes.StoreGenerationIndex);

            //if (!singleLine)
            //{
            //    builder.Append(property.AnnotationsToDebugString(indent + "  "));
            //}

            return builder.ToString();
        }

        public static string ToDebugString(this Key key, bool singleLine = true, string indent = "")
        {
            var builder = new StringBuilder();

            builder.Append(indent);

            if (singleLine)
            {
                builder.Append("Key: ");
            }

            builder.Append(string.Join(", ", key.Properties.Select(p => singleLine ? p.DeclaringEntity.Name + "." + p.Name: p.Name)));

            if (key.IsPrimaryKey)
            {
                builder.Append(" PK");
            }

            //if (!singleLine)
            //{
            //    builder.Append(key.AnnotationsToDebugString(indent + "  "));
            //}

            return builder.ToString();
        }
    }
}
