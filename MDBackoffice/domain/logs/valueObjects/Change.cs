using System;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.Users;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Org.BouncyCastle.Tls;

namespace MDBackoffice.Domain.Logs
{
    public class Change : IValueObject
    {
        public ChangeType Type { get; set; }
        public string ChangeDescription { get; set; }

        private Change() { }
        public Change(ChangeType type, string changeDescription)
        {
            Type = type;
            ChangeDescription = changeDescription;
        }

        
    }
}