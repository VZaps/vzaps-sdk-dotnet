using VZaps.Http;
using VZaps.Resources;

namespace VZaps;

public sealed class VZapsClient : IDisposable
{
    private readonly bool _disposeHttpClient;

    public VZapsClient(VZapsClientOptions options)
        : this(options, new HttpClient(), disposeHttpClient: true)
    {
    }

    public VZapsClient(VZapsClientOptions options, HttpClient httpClient)
        : this(options, httpClient, disposeHttpClient: false)
    {
    }

    internal VZapsClient(VZapsClientOptions options, HttpClient httpClient, bool disposeHttpClient)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (httpClient is null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

        options.Validate();
        _disposeHttpClient = disposeHttpClient;

        Http = new VZapsHttpClient(options, httpClient);
        Auth = new AuthResource(Http);
        Instances = new InstancesResource(Http);
        Sessions = new SessionsResource(Http);
        Messages = new MessagesResource(Http);
        Webhooks = new WebhooksResource(Http);
        Contacts = new ContactsResource(Http);
        Groups = new GroupsResource(Http);
        Users = new UsersResource(Http);
        Queues = new QueuesResource(Http);
        TypeBots = new TypeBotsResource(Http);
        Chatwoot = new ChatwootResource(Http);
        Chats = new ChatsResource(Http);
        Events = new EventsResource(Http, options);
    }

    internal VZapsHttpClient Http { get; }

    public AuthResource Auth { get; }

    public InstancesResource Instances { get; }

    public SessionsResource Sessions { get; }

    public MessagesResource Messages { get; }

    public WebhooksResource Webhooks { get; }

    public ContactsResource Contacts { get; }

    public GroupsResource Groups { get; }

    public UsersResource Users { get; }

    public QueuesResource Queues { get; }

    public TypeBotsResource TypeBots { get; }

    public ChatwootResource Chatwoot { get; }

    public ChatsResource Chats { get; }

    public EventsResource Events { get; }

    public Task<TResponse?> RequestAsync<TResponse>(
        HttpMethod method,
        string path,
        VZapsRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return Http.RequestAsync<TResponse>(method, path, options, cancellationToken);
    }

    public void Dispose()
    {
        Http.Dispose();
        if (_disposeHttpClient)
        {
            Http.InnerClient.Dispose();
        }
    }
}
