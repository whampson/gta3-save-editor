using System;

namespace GTA3SaveEditor.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNumericType(this object o)
        {
            if (IsIntegerType(o)) return true;

            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsIntegerType(this object o)
        {
            if (o == null) return false;

            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }
    }
}
