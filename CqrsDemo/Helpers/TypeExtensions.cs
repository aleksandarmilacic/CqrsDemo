namespace CqrsDemo.Api.Helpers
{
    public static class TypeExtensions
    {
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null || genericType == null)
                return false;

            return givenType == genericType
                   || (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                   || givenType.BaseType?.IsAssignableToGenericType(genericType) == true
                   || givenType.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType);
        }
    }

}
