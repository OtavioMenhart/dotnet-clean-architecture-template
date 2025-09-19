using System.Collections.Concurrent;

namespace CleanArchTemplate.Application.Handlers;

public interface IRequestDispatcher
{
    Task<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public class RequestDispatcher : IRequestDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ConcurrentDictionary<Type, Type> HandlerCache = new();

    public RequestDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = HandlerCache.GetOrAdd(requestType, rt =>
            typeof(IHandler<,>).MakeGenericType(rt, typeof(TResponse))
        );

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"Handler not found for {requestType.Name}");

        return await ((dynamic)handler).Handle((dynamic)request, cancellationToken);
    }
}
