namespace Azf.Shared.Types;

public static class TypeExtensions
{
    public static bool IsConcreteSubTypeOf<TBase>(this Type type)
    {
        return type.IsConcreteSubTypeOf(typeof(TBase));
    }

    public static bool IsConcreteSubTypeOf(this Type type, Type baseType)
    {
        return !type.IsAbstract && !type.IsInterface && baseType.IsAssignableFrom(type);
    }

    public static IEnumerable<Type> GetConcreteSubTypes(this Type type)
    {
        return TypeRepository.GetTypes()
            .Where(t => t.IsConcreteSubTypeOf(type))
            .Where(t => t != type);
    }
}