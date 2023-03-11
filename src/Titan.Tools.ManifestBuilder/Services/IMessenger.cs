namespace Titan.Tools.ManifestBuilder.Services;

public delegate Task MessageTaskCallbackDelegate<in T>(T message);
public delegate void MessageCallbackDelegate<in T>(T message);
public interface IMessenger
{
    void Subscribe<T>(object owner, MessageTaskCallbackDelegate<T> callback);
    void Subscribe<T>(object owner, MessageCallbackDelegate<T> callback);
    Task SendAsync<T>(T message);

}

internal class WeakReferenceMessenger : IMessenger
{
    public void Subscribe<T>(object owner, MessageCallbackDelegate<T> callback)
        => Internal<T>.Add(owner, message =>
        {
            callback(message);
            return Task.CompletedTask;
        });

    public void Subscribe<T>(object owner, MessageTaskCallbackDelegate<T> callback)
        => Internal<T>.Add(owner, callback);

    public Task SendAsync<T>(T message)
        => Internal<T>.Send(message);

    private static class Internal<T>
    {
        private static readonly List<MessageCallback> _subscribers = new();
        public static void Add(object owner, MessageTaskCallbackDelegate<T> callback)
            => _subscribers.Add(new MessageCallback(new WeakReference(owner), callback));

        public static async Task Send(T message)
        {
            var hasDeadReferences = false;
            foreach (var subscriber in _subscribers)
            {
                if (!subscriber.Owner.IsAlive)
                {
                    hasDeadReferences = true;
                    continue;
                }
                await subscriber.Callback(message);
            }

            if (hasDeadReferences)
            {
                //NOTE(Jens): possible exception if something else is sending a message. Leave it for now, fix it if needed (lock or semaphore)
                _subscribers.RemoveAll(s => !s.Owner.IsAlive);
            }
        }

        private record MessageCallback(WeakReference Owner, MessageTaskCallbackDelegate<T> Callback);
    }

}

internal class MulticastDelegateMessenger : IMessenger
{
    
    public void Subscribe<T>(object owner, MessageTaskCallbackDelegate<T> callback) => MessengerInternal<T>.Callbacks += callback;
    public void Subscribe<T>(object owner, MessageCallbackDelegate<T> callback) => throw new NotImplementedException();
    public async Task SendAsync<T>(T message)
    {
        foreach (var del in MessengerInternal<T>.Callbacks?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            if (del is not MessageTaskCallbackDelegate<T> method)
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
        public static MessageTaskCallbackDelegate<T>? Callbacks;
    }
}
