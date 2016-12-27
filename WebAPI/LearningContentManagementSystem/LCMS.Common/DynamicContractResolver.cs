using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LCMS.Common
{
    class DynamicContractResolver : DefaultContractResolver
    {
        private readonly List<string> _startingWithChar;
 
        public DynamicContractResolver(List<string> startingWithChar)
        {
            _startingWithChar = startingWithChar;
        }
 
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            // only serializer properties that start with the specified character
            //properties = properties.Where(_startingWithChar.Any(p=>p == )).ToList();

            return properties;
        }
    }
}
