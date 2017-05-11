using Akka.Actor;
using ProtoBuf;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlWorkScheduler.App.Actors
{
    [ProtoContract]
    class WorkItem
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
    }

    class SaveToDiskActor : ReceiveActor
    {
        private Dictionary<string, WorkItem> _workItems;
        private string _filePath = "./savedworkItems.bin";

        protected override void PreStart()
        {
            using (var file = File.Open(_filePath, FileMode.Open, FileAccess.Read))
            {
                _workItems = Serializer.Deserialize<Dictionary<string, WorkItem>>(file);
            }

            foreach (var item in _workItems)
            {
                StaticActors.SchedulerActor
                    .Tell(new ScheduleWorkCmd(item.Key, item.Value.SqlQuery, item.Value.SqlConnection, item.Value.Interval, item.Value.EndPoint, false));
            }

            base.PreStart();
        }

        public SaveToDiskActor()
        {
            Receive<SaveWorkItemToDiskCmd>(cmd => ReceiveSaveWorkItemToDiskCmd(cmd));
            Receive<RemoveWorkItemFromDiskCmd>(cmd => ReceiveRemoveWorkItemFromDiskCmd(cmd));
            Receive<UpdateLastRunTickCmd>(cmd => ReceiveUpdateLastRunTickCmd(cmd));
        }

        private void ReceiveUpdateLastRunTickCmd(UpdateLastRunTickCmd cmd)
        {
            _workItems[cmd.Id].LastRun = cmd.NewValue;

            using (var file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Serializer.Serialize(file, _workItems);
            }
        }

        private void ReceiveRemoveWorkItemFromDiskCmd(RemoveWorkItemFromDiskCmd cmd)
        {
            var workItem = _workItems[cmd.Id];

            if (workItem != null)
            {
                _workItems.Remove(cmd.Id);

                using (var file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    Serializer.Serialize(file, _workItems);
                }
            }
        }

        private void ReceiveSaveWorkItemToDiskCmd(SaveWorkItemToDiskCmd cmd)
        {
            var contract = new WorkItem()
            {
                SqlQuery = cmd.SqlQuery,
                SqlConnection = cmd.SqlConnection,
                Interval = cmd.Interval,
                LastRun = new DateTime().AddYears(1970).Ticks
            };
            _workItems.Add(cmd.Id, contract);

            using (var file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Serializer.Serialize(file, _workItems);
            }
        }
    }
}
