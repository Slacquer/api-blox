﻿namespace APIBlox.NetCore.Documents
{
    public class EventDocument : DocumentBase
    {
        public override string Id => GenerateId(StreamId, Version);

        public override DocumentType DocumentType => DocumentType.Event;

        public string EventType { get; set; }

        public object EventData { get; set; }

        public static string GenerateId(string streamId, ulong version)
        {
            return $"{streamId}{Separator}{version}";
        }
    }
}