using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using VZaps.Models;

namespace VZaps.SDK.Tests;

public sealed class ClientTests
{
    [Fact]
    public async Task GetAccessTokenAsync_CachesToken()
    {
        using var fixture = new ClientFixture();

        var first = await fixture.Client.Auth.GetAccessTokenAsync();
        var second = await fixture.Client.Auth.GetAccessTokenAsync();

        first.Should().Be("jwt-token");
        second.Should().Be("jwt-token");
        fixture.Handler.Requests.Count(request => request.RequestUri!.AbsolutePath == "/token").Should().Be(1);
    }

    [Fact]
    public async Task SendTextAsync_SendsExpectedHeadersPathAndBody()
    {
        using var fixture = new ClientFixture();

        await fixture.Client.Messages.SendTextAsync<JsonElement>(new SendTextMessageRequest
        {
            InstanceId = "VZ123",
            InstanceToken = "instance-token",
            Phone = "5511999999999",
            Message = "Hello from VZaps",
        });

        var request = fixture.Handler.Requests.Last();
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri!.AbsolutePath.Should().Be("/instances/VZ123/chat/send/text");
        request.Headers.Authorization!.Scheme.Should().Be("Bearer");
        request.Headers.Authorization.Parameter.Should().Be("jwt-token");
        request.Headers.GetValues("X-Client-Token").Single().Should().Be("client-token");
        request.Headers.GetValues("X-Instance-Token").Single().Should().Be("instance-token");
        request.Headers.UserAgent.ToString().Should().Contain("VZaps.SDK/");

        var body = JsonDocument.Parse(await fixture.Handler.Bodies.Last().ReadAsStringAsync()).RootElement;
        body.TryGetProperty("instance_id", out _).Should().BeFalse();
        body.TryGetProperty("instance_token", out _).Should().BeFalse();
        body.GetProperty("phone").GetString().Should().Be("5511999999999");
        body.GetProperty("message").GetString().Should().Be("Hello from VZaps");
    }

    [Fact]
    public async Task GetInstanceAsync_SendsExpectedPathAndBody()
    {
        using var fixture = new ClientFixture();

        await fixture.Client.Instances.GetAsync<JsonElement>("VZKB8AU4S4CWY1SLXX4I5WJGRZQMDDFTV6");

        var request = fixture.Handler.Requests.Last();
        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri!.AbsolutePath.Should().Be("/instances/get");

        var body = JsonDocument.Parse(await fixture.Handler.Bodies.Last().ReadAsStringAsync()).RootElement;
        body.GetProperty("id").GetString().Should().Be("VZKB8AU4S4CWY1SLXX4I5WJGRZQMDDFTV6");
    }

    [Fact]
    public async Task ListInstancesAsync_NormalizesSearchAndPageSize()
    {
        using var fixture = new ClientFixture();

        await fixture.Client.Instances.ListAsync<JsonElement>(new InstanceListRequest
        {
            Page = 2,
            PageSize = 50,
            Search = "  sales  ",
        });

        var body = JsonDocument.Parse(await fixture.Handler.Bodies.Last().ReadAsStringAsync()).RootElement;
        body.GetProperty("page").GetInt32().Should().Be(2);
        body.GetProperty("size").GetInt32().Should().Be(50);
        body.GetProperty("filter").GetProperty("query").GetString().Should().Be("sales");
    }

    [Theory]
    [InlineData(401, typeof(VZapsAuthenticationException))]
    [InlineData(403, typeof(VZapsAuthenticationException))]
    [InlineData(429, typeof(VZapsRateLimitException))]
    [InlineData(500, typeof(VZapsApiException))]
    public async Task RequestAsync_MapsApiErrors(int statusCode, Type exceptionType)
    {
        using var fixture = new ClientFixture();
        fixture.Handler.ErrorStatusCode = (HttpStatusCode)statusCode;

        var action = () => fixture.Client.Instances.GetAsync<JsonElement>("VZ123");

        var exception = await action.Should().ThrowAsync<VZapsException>();
        exception.Which.Should().BeOfType(exceptionType);
        ((VZapsApiException)exception.Which).ResponseBody.Should().Contain("Nope");
    }

    [Theory]
    [MemberData(nameof(MessageEndpointCases))]
    public async Task MessagesResource_UsesExpectedEndpoints(Func<VZapsClient, Task> call, string method, string path)
    {
        using var fixture = new ClientFixture();

        await call(fixture.Client);

        var request = fixture.Handler.Requests.Last();
        request.Method.Method.Should().Be(method);
        request.RequestUri!.AbsolutePath.Should().Be(path);
    }

    public static IEnumerable<object[]> MessageEndpointCases()
    {
        var instance = "VZ123";
        var token = "instance-token";
        yield return Case(client => client.Messages.SendImageAsync<JsonElement>(new SendImageMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Image = "https://example.com/a.png" }), "POST", "/instances/VZ123/chat/send/image");
        yield return Case(client => client.Messages.SendAudioAsync<JsonElement>(new SendAudioMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Audio = "https://example.com/a.mp3" }), "POST", "/instances/VZ123/chat/send/audio");
        yield return Case(client => client.Messages.SendDocumentAsync<JsonElement>(new SendDocumentMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Document = "https://example.com/a.pdf" }), "POST", "/instances/VZ123/chat/send/document");
        yield return Case(client => client.Messages.SendVideoAsync<JsonElement>(new SendVideoMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Video = "https://example.com/a.mp4" }), "POST", "/instances/VZ123/chat/send/video");
        yield return Case(client => client.Messages.SendStickerAsync<JsonElement>(new SendStickerMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Sticker = "data:image/webp;base64,AA==" }), "POST", "/instances/VZ123/chat/send/sticker");
        yield return Case(client => client.Messages.SendGifAsync<JsonElement>(new SendGifMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Gif = "https://example.com/a.gif" }), "POST", "/instances/VZ123/chat/send/gif");
        yield return Case(client => client.Messages.SendLocationAsync<JsonElement>(new SendLocationMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Latitude = -23.5, Longitude = -46.6 }), "POST", "/instances/VZ123/chat/send/location");
        yield return Case(client => client.Messages.SendContactAsync<JsonElement>(new SendContactMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55" }), "POST", "/instances/VZ123/chat/send/contact");
        yield return Case(client => client.Messages.SendButtonsAsync<JsonElement>(new SendButtonsMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Message = "Choose", Buttons = new[] { new MessageButton { Id = "a", Text = "A" } } }), "POST", "/instances/VZ123/chat/send/buttons");
        yield return Case(client => client.Messages.SendListAsync<JsonElement>(new SendListMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Title = "Menu", Description = "Pick", ButtonText = "Open", Sections = Array.Empty<MessageListSection>() }), "POST", "/instances/VZ123/chat/send/list");
        yield return Case(client => client.Messages.SendLinkAsync<JsonElement>(new SendLinkMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Message = "Link", LinkUrl = "https://vzaps.com", Title = "VZaps", LinkDescription = "Docs" }), "POST", "/instances/VZ123/chat/send/link");
        yield return Case(client => client.Messages.SendPollAsync<JsonElement>(new SendPollMessageRequest { InstanceId = instance, InstanceToken = token, Phone = "55", Name = "Poll", Options = new[] { "A", "B" } }), "POST", "/instances/VZ123/chat/send/poll");
        yield return Case(client => client.Messages.PollVoteAsync<JsonElement>(new MessagePollVoteRequest { InstanceId = instance, InstanceToken = token, Phone = "55", MessageId = "msg", Vote = "A" }), "POST", "/instances/VZ123/chat/poll/vote");
        yield return Case(client => client.Messages.ReactAsync<JsonElement>(new MessageReactRequest { InstanceId = instance, InstanceToken = token, Phone = "55", MessageId = "msg", Reaction = "ok" }), "POST", "/instances/VZ123/chat/react");
        yield return Case(client => client.Messages.RemoveReactionAsync<JsonElement>(new MessageReactRemoveRequest { InstanceId = instance, InstanceToken = token, Phone = "55", MessageId = "msg" }), "DELETE", "/instances/VZ123/chat/react");
        yield return Case(client => client.Messages.PresenceAsync<JsonElement>(new MessagePresenceRequest { InstanceId = instance, InstanceToken = token, Phone = "55", State = "composing" }), "POST", "/instances/VZ123/chat/presence");
        yield return Case(client => client.Messages.MarkReadAsync<JsonElement>(new MessageMarkReadRequest { InstanceId = instance, InstanceToken = token, Chat = "55", Id = new[] { "msg" } }), "POST", "/instances/VZ123/chat/markread");
        yield return Case(client => client.Messages.DownloadImageAsync<JsonElement>(new MessageDownloadRequest { InstanceId = instance, InstanceToken = token }), "POST", "/instances/VZ123/chat/downloadimage");
        yield return Case(client => client.Messages.DownloadVideoAsync<JsonElement>(new MessageDownloadRequest { InstanceId = instance, InstanceToken = token }), "POST", "/instances/VZ123/chat/downloadvideo");
        yield return Case(client => client.Messages.DownloadAudioAsync<JsonElement>(new MessageDownloadRequest { InstanceId = instance, InstanceToken = token }), "POST", "/instances/VZ123/chat/downloadaudio");
        yield return Case(client => client.Messages.DownloadDocumentAsync<JsonElement>(new MessageDownloadRequest { InstanceId = instance, InstanceToken = token }), "POST", "/instances/VZ123/chat/downloaddocument");
        yield return Case(client => client.Messages.EditAsync<JsonElement>(new MessageEditRequest { InstanceId = instance, InstanceToken = token, MessageId = "msg", Message = "Edited" }), "PATCH", "/instances/VZ123/chat/messages/msg");
        yield return Case(client => client.Messages.DeleteAsync<JsonElement>(new MessageDeleteRequest { InstanceId = instance, InstanceToken = token, MessageId = "msg" }), "DELETE", "/instances/VZ123/chat/messages/msg");
    }

    private static object[] Case(Func<VZapsClient, Task> call, string method, string path) => new object[] { call, method, path };

    private sealed class ClientFixture : IDisposable
    {
        public ClientFixture()
        {
            Handler = new FakeHandler();
            Client = new VZapsClient(
                new VZapsClientOptions
                {
                    ClientToken = "client-token",
                    ClientSecret = "client-secret",
                    BaseUrl = new Uri("https://api.test.local"),
                    Timeout = TimeSpan.FromSeconds(5),
                },
                new HttpClient(Handler));
        }

        public FakeHandler Handler { get; }

        public VZapsClient Client { get; }

        public void Dispose() => Client.Dispose();
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly ConcurrentQueue<HttpRequestMessage> _requests = new();
        private readonly ConcurrentQueue<StringContent> _bodies = new();

        public IReadOnlyCollection<HttpRequestMessage> Requests => _requests.ToArray();

        public IReadOnlyCollection<StringContent> Bodies => _bodies.ToArray();

        public HttpStatusCode? ErrorStatusCode { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var clone = await CloneAsync(request).ConfigureAwait(false);
            _requests.Enqueue(clone.Request);
            if (clone.Body is not null)
            {
                _bodies.Enqueue(clone.Body);
            }

            if (request.RequestUri!.AbsolutePath == "/token")
            {
                return Json("""{"accessToken":"jwt-token","expiresIn":3600}""");
            }

            if (ErrorStatusCode is not null)
            {
                return new HttpResponseMessage(ErrorStatusCode.Value)
                {
                    Content = new StringContent("""{"message":"Nope","code":"bad_request","details":"Details"}"""),
                };
            }

            return Json("""{"ok":true}""");
        }

        private static HttpResponseMessage Json(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
            };
        }

        private static async Task<(HttpRequestMessage Request, StringContent? Body)> CloneAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            StringContent? body = null;
            if (request.Content is not null)
            {
                var text = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                body = new StringContent(text);
                clone.Content = body;
            }

            return (clone, body);
        }
    }
}
