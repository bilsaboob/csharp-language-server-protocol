using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using Xunit;

namespace Lsp.Tests.Matchers
{
    public class TextDocumentMatcherTests
    {
        private readonly ILogger _logger;

        public TextDocumentMatcherTests()
        {
            _logger = Substitute.For<ILogger>();
        }

        [Fact]
        public void Should_Not_Return_Null()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = new TextDocumentMatcher(_logger);

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().NotBeNull();
        }

        [Fact]
        public void Should_Return_Empty_Descriptor()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();

            var handlerMatcher = new TextDocumentMatcher(_logger);

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_Did_Open_Text_Document_Handler_Descriptor()
        {
            // Given
            var handlerMatcher = new TextDocumentMatcher(_logger);
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            // When
            var result = handlerMatcher.FindHandler(new DidOpenTextDocumentParams() {
                TextDocument = new TextDocumentItem {
                    Uri = new Uri("file:///abc/123/d.cs")
                }
            },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor("textDocument/didOpen",
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == "textDocument/didOpen");
        }

        [Fact]
        public void Should_Return_Did_Change_Text_Document_Descriptor()
        {
            // Given
            var handlerMatcher = new TextDocumentMatcher(_logger);
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            // When
            var result = handlerMatcher.FindHandler(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor("textDocument/didChange",
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == "textDocument/didChange");
        }

        [Fact]
        public void Should_Return_Did_Save_Text_Document_Descriptor()
        {
            // Given
            var handlerMatcher = new TextDocumentMatcher(_logger);
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            // When
            var result = handlerMatcher.FindHandler(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor("textDocument/didSave",
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == "textDocument/didSave");
        }

        [Fact]
        public void Should_Return_Did_Close_Text_Document_Descriptor()
        {
            // Given
            var handlerMatcher = new TextDocumentMatcher(_logger);
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            // When
            var result = handlerMatcher.FindHandler(new DidCloseTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor("textDocument/didClose",
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == "textDocument/didClose");
        }
    }
}
