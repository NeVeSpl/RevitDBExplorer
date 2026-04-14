using System;
using CommunityToolkit.Mvvm.Messaging;

namespace RevitDBExplorer.Utils
{

    internal interface IHaveSender
    {
        object Sender { get; set; }
    }

    internal sealed class MessengerWithSender : IMessenger
    {
        private readonly IMessenger _inner;
        private readonly object _sender;

        public MessengerWithSender(object sender, IMessenger inner)
        {
            _sender = sender;
            _inner = inner;
        }

        public void Cleanup()
        {
            _inner.Cleanup();
        }

        public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            return _inner.IsRegistered<TMessage, TToken>(recipient, token);
        }

        public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
            where TRecipient : class
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            _inner.Register(recipient, token, handler);
        }

        public void Reset()
        {
            _inner.Reset();
        }

        public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            if (message is IHaveSender messageWithSender)
            {
                messageWithSender.Sender = _sender;
            }
            return _inner.Send<TMessage, TToken>(message, token);
        }

        public void Unregister<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            _inner.Unregister<TMessage, TToken>(recipient, token);
        }

        public void UnregisterAll(object recipient)
        {
            _inner.UnregisterAll(recipient);
        }

        public void UnregisterAll<TToken>(object recipient, TToken token) where TToken : IEquatable<TToken>
        {
            _inner.UnregisterAll<TToken>(recipient, token);
        }
    }
}
