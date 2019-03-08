namespace APIBlox.NetCore.Contracts
{
    /// <summary>
    ///     Marker Interface
    ///     <para>
    ///         Important characteristics of events is that since an event is something that happened in the past,
    ///         it should not change. Therefore it must be an immutable class.  It's name should also be of the past tense.
    ///     </para>
    /// </summary>
    public interface IDomainEvent
    {
    }
}
