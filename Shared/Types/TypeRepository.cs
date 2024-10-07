namespace Azf.Shared.Types;

public static class TypeRepository
{
    private static readonly Type[] AllTypes =
        AppDomain
            .CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.FullName!.StartsWith("Azf."))
            .SelectMany(a => a.GetTypes())
            .ToArray();

    public static IEnumerable<Type> GetTypes()
    {
        return AllTypes;
    }

    public static IEnumerable<Type> GetConcreteSubTypesOf<TBase>()
    {
        return GetTypes().Where(t => t.IsConcreteSubTypeOf<TBase>());
    }
}