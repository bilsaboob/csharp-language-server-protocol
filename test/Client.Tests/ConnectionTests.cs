using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Client.Dispatcher;
using OmniSharp.Extensions.LanguageServer.Client.Protocol;
using Xunit;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests
{
    /// <summary>
    ///     Tests for <see cref="LspConnection"/>.
    /// </summary>
    public class ConnectionTests
        : PipeServerTestBase
    {
        /// <summary>
        ///     Create a new <see cref="LspConnection"/> test suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        public ConnectionTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }

        /// <summary>
        ///     Verify that a server <see cref="LspConnection"/> can handle an empty notification from a client <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Server connection can handle empty notification from client")]
        public async Task Client_HandleEmptyNotification_Success()
        {
            var testCompletion = new TaskCompletionSource<object>();

            LspConnection clientConnection = await CreateClientConnection();
            LspConnection serverConnection = await CreateServerConnection();

            var serverDispatcher = new LspDispatcher();
            serverDispatcher.HandleEmptyNotification("test", () =>
            {
                Log.LogInformation("Got notification.");

                testCompletion.SetResult(null);
            });
            serverConnection.Connect(serverDispatcher);

            clientConnection.Connect(new LspDispatcher());
            clientConnection.SendEmptyNotification("test");

            await testCompletion.Task;

            clientConnection.Disconnect(flushOutgoing: true);
            serverConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }

        /// <summary>
        ///     Verify that a client <see cref="LspConnection"/> can handle an empty notification from a server <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Client connection can handle empty notification from server")]
        public async Task Server_HandleEmptyNotification_Success()
        {
            var testCompletion = new TaskCompletionSource<object>();

            LspConnection clientConnection = await CreateClientConnection();
            LspConnection serverConnection = await CreateServerConnection();

            var clientDispatcher = new LspDispatcher();
            clientDispatcher.HandleEmptyNotification("test", () =>
            {
                Log.LogInformation("Got notification.");

                testCompletion.SetResult(null);
            });
            clientConnection.Connect(clientDispatcher);

            serverConnection.Connect(new LspDispatcher());
            serverConnection.SendEmptyNotification("test");

            await testCompletion.Task;

            serverConnection.Disconnect(flushOutgoing: true);
            clientConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }

        /// <summary>
        ///     Verify that a client <see cref="LspConnection"/> can handle a request from a server <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Client connection can handle request from server")]
        public async Task Server_HandleRequest_Success()
        {
            LspConnection clientConnection = await CreateClientConnection();
            LspConnection serverConnection = await CreateServerConnection();

            var clientDispatcher = new LspDispatcher();
            clientDispatcher.HandleRequest<TestRequest, TestResponse>("test", (request, cancellationToken) =>
            {
                Log.LogInformation("Got request: {@Request}", request);

                return Task.FromResult(new TestResponse
                {
                    Value = request.Value.ToString()
                });
            });
            clientConnection.Connect(clientDispatcher);

            serverConnection.Connect(new LspDispatcher());
            TestResponse response = await serverConnection.SendRequest<TestResponse>("test", new TestRequest
            {
                Value = 1234
            });

            Assert.Equal("1234", response.Value);

            Log.LogInformation("Got response: {@Response}", response);

            serverConnection.Disconnect(flushOutgoing: true);
            clientConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }

        /// <summary>
        ///     Verify that a server <see cref="LspConnection"/> can handle a request from a client <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Server connection can handle request from client")]
        public async Task Client_HandleRequest_Success()
        {
            LspConnection clientConnection = await CreateClientConnection();
            LspConnection serverConnection = await CreateServerConnection();

            var serverDispatcher = new LspDispatcher();
            serverDispatcher.HandleRequest<TestRequest, TestResponse>("test", (request, cancellationToken) =>
            {
                Log.LogInformation("Got request: {@Request}", request);

                return Task.FromResult(new TestResponse
                {
                    Value = request.Value.ToString()
                });
            });
            serverConnection.Connect(serverDispatcher);

            clientConnection.Connect(new LspDispatcher());
            TestResponse response = await clientConnection.SendRequest<TestResponse>("test", new TestRequest
            {
                Value = 1234
            });

            Assert.Equal("1234", response.Value);

            Log.LogInformation("Got response: {@Response}", response);

            clientConnection.Disconnect(flushOutgoing: true);
            serverConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }

        /// <summary>
        ///     Verify that a client <see cref="LspConnection"/> can handle a command-style request (i.e. no response body) from a server <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Client connection can handle command request from server")]
        public async Task Server_HandleCommandRequest_Success()
        {
            LspConnection clientConnection = await CreateClientConnection();
            LspConnection serverConnection = await CreateServerConnection();

            var clientDispatcher = new LspDispatcher();
            clientDispatcher.HandleRequest<TestRequest>("test", (request, cancellationToken) =>
            {
                Log.LogInformation("Got request: {@Request}", request);

                Assert.Equal(1234, request.Value);

                return Task.CompletedTask;
            });
            clientConnection.Connect(clientDispatcher);

            serverConnection.Connect(new LspDispatcher());
            await serverConnection.SendRequest("test", new TestRequest
            {
                Value = 1234
            });

            serverConnection.Disconnect(flushOutgoing: true);
            clientConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }

        /// <summary>
        ///     Verify that a server <see cref="LspConnection"/> can handle a command-style request (i.e. no response body) from a client <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Server connection can handle command request from client")]
        public async Task Client_HandleCommandRequest_Success()
        {
            LspConnection clientConnection = await CreateClientConnection();
            LspConnection serverConnection = await CreateServerConnection();

            var serverDispatcher = new LspDispatcher();
            serverDispatcher.HandleRequest<TestRequest>("test", (request, cancellationToken) =>
            {
                Log.LogInformation("Got request: {@Request}", request);

                Assert.Equal(1234, request.Value);

                return Task.CompletedTask;
            });
            serverConnection.Connect(serverDispatcher);

            clientConnection.Connect(new LspDispatcher());
            await clientConnection.SendRequest("test", new TestRequest
            {
                Value = 1234
            });

            clientConnection.Disconnect(flushOutgoing: true);
            serverConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }
    }

    /// <summary>
    ///     A test request.
    /// </summary>
    class TestRequest
    {
        /// <summary>
        ///     A test value for the request.
        /// </summary>
        public int Value { get; set; }
    }

    /// <summary>
    ///     A test response.
    /// </summary>
    class TestResponse
    {
        /// <summary>
        ///     A test value for the response.
        /// </summary>
        public string Value { get; set; }
    }
}
