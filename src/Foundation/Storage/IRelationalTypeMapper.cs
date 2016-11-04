﻿using System;
using Foundation.Metadata;

namespace Foundation.Storage
{
    /// <summary>
    ///     <para>
    ///         Maps .NET types to their corresponding relational database types.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public interface IRelationalTypeMapper
    {
        /// <summary>
        ///     Gets the relational database type for the given property.
        ///     Returns null if no mapping is found.
        /// </summary>
        /// <param name="property"> The property to get the mapping for. </param>
        /// <returns> The type mapping to be used. </returns>
        RelationalTypeMapping FindMapping(Property property);

        /// <summary>
        ///     Gets the relational database type for a given .NET type.
        ///     Returns null if no mapping is found.
        /// </summary>
        /// <param name="clrType"> The type to get the mapping for. </param>
        /// <returns> The type mapping to be used. </returns>
        RelationalTypeMapping FindMapping(Type clrType);

        /// <summary>
        ///     Gets the mapping that represents the given database type.
        ///     Returns null if no mapping is found.
        /// </summary>
        /// <param name="storeType"> The type to get the mapping for. </param>
        /// <returns> The type mapping to be used. </returns>
        RelationalTypeMapping FindMapping(string storeType);

        /// <summary>
        ///     Ensures that the given type name is a valid type for the relational database.
        ///     An exception is thrown if it is not a valid type.
        /// </summary>
        /// <param name="storeType"> The type to be validated. </param>
        void ValidateTypeName(string storeType);

        /// <summary>
        ///     Gets the mapper to be used for byte array properties.
        /// </summary>
        IByteArrayRelationalTypeMapper ByteArrayMapper { get; }

        /// <summary>
        ///     Gets the mapper to be used for string properties.
        /// </summary>
        IStringRelationalTypeMapper StringMapper { get; }
    }
}
