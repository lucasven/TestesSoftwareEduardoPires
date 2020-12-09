using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Core.Messages
{
    public abstract class Message : IRequest<bool>
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }

        public Message()
        {
            MessageType = GetType().Name;
        }
    }

    public abstract class Event: Message, INotification
    {
        public DateTime TimeStamp { get; set; }

        public Event()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
