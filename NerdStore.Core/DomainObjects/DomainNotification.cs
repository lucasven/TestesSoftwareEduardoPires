using MediatR;
using NerdStore.Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Core.DomainObjects
{
    public class DomainNotification : Message, INotification
    {
        public DateTime TimeStamp { get; set; }
        public Guid DomainNotificationId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int Version { get; set; }

        public DomainNotification(string key, string value)
        {
            TimeStamp = DateTime.Now;
            DomainNotificationId = Guid.NewGuid();
            Version = 1;
            Key = key;
            Value = value;
        }
    }
}
