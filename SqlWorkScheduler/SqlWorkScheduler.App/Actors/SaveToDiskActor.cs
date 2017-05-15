using Akka.Actor;
using ProtoBuf;
using SqlWorkScheduler.App.Contracts;
using SqlWorkScheduler.App.Messeges;
using System;
using System.Collections.Generic;
using System.IO;

namespace SqlWorkScheduler.App.Actors
{
    class SaveToDiskActor : ReceiveActor
    {
        private Dictionary<string, SqlWorkItem> _workItems;
        private string _filePath = "./savedworkItems.bin";

        protected override void PreStart()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    using (var file = File.Open(_filePath, FileMode.Open, FileAccess.Read))
                    {
                        _workItems = Serializer.Deserialize<Dictionary<string, SqlWorkItem>>(file);
                        file.Close();
                    }

                    foreach (var item in _workItems)
                    {
                        StaticActors.SchedulerActor
                            .Tell(new ScheduleWorkCmd(item.Key, item.Value.SqlQuery, item.Value.SqlConnection, item.Value.Interval, item.Value.EndPoint, item.Value.SpParameters, item.Value.LastRun, false));
                    }
                }
                else
                {
                    using (var file = File.Create(_filePath))
                        file.Close();

                    _workItems = new Dictionary<string, SqlWorkItem>();
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
                var contract = new SqlWorkItem()
                {
                    SqlQuery = cmd.ScheduleMessage.SqlQuery,
                    SqlConnection = cmd.ScheduleMessage.SqlConnection,
                    Interval = cmd.ScheduleMessage.Interval,
                    LastRun = cmd.ScheduleMessage.LastRun,
                    EndPoint = cmd.ScheduleMessage.EndPoint,
                    SpParameters = cmd.ScheduleMessage.SpParameters
                };
                _workItems.Add(cmd.ScheduleMessage.Id, contract);

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
    }
}
