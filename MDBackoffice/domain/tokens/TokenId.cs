using System;
using MDBackoffice.Domain.Shared;
using Newtonsoft.Json;

namespace MDBackoffice.Domain.Tokens
{
    public class TokenId : EntityId
    {

        [JsonConstructor]
        public TokenId(Guid value) : base(value)
        {
        }

        public TokenId(String value) : base(value)
        {
        }

        override
        protected  Object createFromString(String text){
            return new Guid(text);
        }

        override
        public String AsString(){
            Guid obj = (Guid) base.ObjValue;
            return obj.ToString();
        }
        
       
        public Guid AsGuid(){
            return (Guid) base.ObjValue;
        }
    }
}