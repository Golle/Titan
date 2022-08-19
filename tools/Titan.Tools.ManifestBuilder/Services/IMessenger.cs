using System;
using System.Threading.Tasks;

namespace Titan.Tools.ManifestBuilder.Services;


public delegate Task MessageCallbackDelegate<in T>(T message);

public interface IMessenger
{
    void Subscribe<T>(MessageCallbackDelegate<T> callback);
    Task SendAsync<T>(T message);

    //NOTE(Jens): We don't add a unsubscribe method until we need it. All views will be singletons and only created once anyway, so no need for it right now.
}

internal class MulticastDelegateMessenger : IMessenger
{
    public void Subscribe<T>(MessageCallbackDelegate<T> callback)
    {
        MessengerInternal<T>.Callbacks += callback;
    }

    public async Task SendAsync<T>(T message)
    {
        foreach (var del in MessengerInternal<T>.Callbacks?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            if (del is not MessageCallbackDelegate<T> method)
            {
                continue;
            }
            try
            {
                await method(message);
            }
            catch
            {
                // ignored
            }
        }
    }

    private static class MessengerInternal<T>
    {
        public static MessageCallbackDelegate<T>? Callbacks;
    }
}
