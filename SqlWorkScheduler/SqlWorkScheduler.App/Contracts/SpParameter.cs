using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Contracts
{
    [ProtoContract]
    public class SpParameter
    {
        [ProtoMember(1)]
        public string ParameterName { get; set; }
        [ProtoMember(2)]
        public string ParameterValue { get; set; }
    }
}
