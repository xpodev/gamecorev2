using System;


namespace GameCore
{
    internal static class EnumTools
    {
        public static T GetEnumValueAs<T, EnumT>(EnumT value) where EnumT : struct, Enum where T : struct
        {
            Type enumUnderlyingType = Enum.GetUnderlyingType(typeof(EnumT));
            object result = Convert.ChangeType(value, enumUnderlyingType);
            return (T)result;
        }
    }
}
