using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Contracts
{
    [ProtoContract]
    class SqlWorkItem
    {
        [ProtoMember(1)]
        public string SqlQuery { get; set; }
        [ProtoMember(2)]
        public string SqlConnection { get; set; }
        [ProtoMember(3)]
        public int Interval { get; set; }
        [ProtoMember(4)]
        public string EndPoint { get; set; }
        [ProtoMember(5)]
        public long LastRun { get; set; }
        [ProtoMember(6)]
        public SpParameter[] SpParameters { get; set; }
    }
}
