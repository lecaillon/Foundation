using System;
using System.Reflection;

namespace Foundation
{
    public static class PropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo property) => (property.GetMethod ?? property.SetMethod).IsStatic;

        public static bool IsCandidateProperty(this PropertyInfo property, bool needsWrite = true)
            => !property.IsStatic()
               && property.GetIndexParameters().Length == 0
               && property.CanRead
               && (!needsWrite || property.CanWrite)
               && property.GetMethod != null
               && property.GetMethod.IsPublic;

        public static Type FindCandidateNavigationPropertyType(this PropertyInfo propertyInfo)
        {
            var targetType = propertyInfo.PropertyType;
            var targetSequenceType = targetType.TryGetSequenceType();
            if (!propertyInfo.IsCandidateProperty(targetSequenceType == null))
            {
                return null;
            }

            targetType = targetSequenceType ?? targetType;
            targetType = targetType.UnwrapNullableType();

            if (targetType.IsPrimitive()
                || targetType.GetTypeInfo().IsInterface
                || targetType.GetTypeInfo().IsValueType
                || targetType == typeof(object))
            {
                return null;
            }

            return targetType;
        }
    }
}
