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
            try
            {
                if (File.Exists(_filePath))
                {
                    using (var file = File.Open(_filePath, FileMode.Open, FileAccess.Read))
                    {
                        _workItems = Serializer.Deserialize<Dictionary<string, WorkItem>>(file);
                        file.Close();
                    }

                    foreach (var item in _workItems)
                    {
                        StaticActors.SchedulerActor
                            .Tell(new ScheduleWorkCmd(item.Key, item.Value.SqlQuery, item.Value.SqlConnection, item.Value.Interval, item.Value.EndPoint, item.Value.LastRun, false));
                    }
                }
                else
                {
                    using (var file = File.Create(_filePath))
                        file.Close();

                    _workItems = new Dictionary<string, WorkItem>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
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
            try
            {
                _workItems[cmd.Id].LastRun = cmd.NewValue;

                using (var file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    Serializer.Serialize(file, _workItems);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        private void ReceiveRemoveWorkItemFromDiskCmd(RemoveWorkItemFromDiskCmd cmd)
        {
            try
            {
                var workItem = _workItems[cmd.Id];

                if (workItem != null)
                {
                    _workItems.Remove(cmd.Id);

                    using (var file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        Serializer.Serialize(file, _workItems);
                        file.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        private void ReceiveSaveWorkItemToDiskCmd(SaveWorkItemToDiskCmd cmd)
        {
            try
            {
                var contract = new WorkItem()
                {
                    SqlQuery = cmd.SqlQuery,
                    SqlConnection = cmd.SqlConnection,
                    Interval = cmd.Interval,
                    LastRun = new DateTime().AddYears(1970).Ticks,
                    EndPoint = cmd.EndPoint
                };
                _workItems.Add(cmd.Id, contract);

                using (var file = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    Serializer.Serialize(file, _workItems);
                    file.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
