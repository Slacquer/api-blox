using System;

namespace APIBlox.NetCore.Models
{
    /// <summary>
    ///     Class EventStreamModel.
    /// </summary>
    public class EventStreamModel
    {
        /// <summary>
        ///     Gets or sets the events.
        /// </summary>
        /// <value>The events.</value>
        public EventModel[] Events { get; set; }

        /// <summary>
        ///     Gets or sets the snapshot.
        /// </summary>
        /// <value>The snapshot.</value>
        public SnapshotModel Snapshot { get; set; }

        /// <summary>
        ///     Gets or sets the stream identifier.
        /// </summary>
        /// <value>The stream identifier.</value>
        public string StreamId { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public long Version { get; set; }

        /// <summary>
        ///     Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTimeOffset TimeStamp { get; set; }
    }
}
