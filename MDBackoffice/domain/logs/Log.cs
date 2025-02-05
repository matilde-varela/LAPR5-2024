using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Logs
{
    public class Log : Entity<LogId>, IAggregateRoot
    {
        public Change Change { get; set; }
        public string ObjectReference { get; set; }
        public string ObjectClass { get; set; }
        public Date DateOfChange { get; set; }

        private Log() { }
        public Log(string seqNumber, string objectClass, string objectReference, int typeOfChange, string changeDescription)
        {
            Id = new LogId(seqNumber, true);
            ObjectReference = objectReference;
            ObjectClass = objectClass;
            Change = new Change((ChangeType)typeOfChange, changeDescription);
            DateOfChange = new Date(DateTime.Now.ToString());
        }

        
    }
}