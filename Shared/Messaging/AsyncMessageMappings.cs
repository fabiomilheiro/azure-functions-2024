using Azf.Shared.Types;

namespace Azf.Shared.Messaging;

public static class AsyncMessageMappings
{
    public static readonly Dictionary<string, AsyncMessageMapping> ByMessageTypeName =
        GetAsyncMessageHandlerTypeMappings();

    private static Dictionary<string, AsyncMessageMapping> GetAsyncMessageHandlerTypeMappings()
    {
        return TypeRepository
               .GetConcreteSubTypesOf<IAsyncMessageHandler>()
               .Select(type =>
               {
                   if (type.BaseType!.GetGenericTypeDefinition() != typeof(AsyncMessageHandlerBase<>))
                   {
                       throw new Exception(
                           $"Message handler '{type}' must implement '{typeof(AsyncMessageHandlerBase<>)}'");
                   }

                   return new
                   {
                       MessageType = type.BaseType.GetGenericArguments()[0],
                       HandlerType = type,
                   };
               })
               .ToDictionary(
                   x => x.MessageType.Name,
                   x => new AsyncMessageMapping
                   {
                       MessageType = x.MessageType,
                       HandlerType = x.HandlerType,
                   });
    }
}

public class AsyncMessageMapping
{
    public required Type HandlerType { get; set; }

    public required Type MessageType { get; set; }
}