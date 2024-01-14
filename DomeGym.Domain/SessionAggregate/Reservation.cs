namespace DomeGym.Domain.SessionAggregate;

public sealed class Reservation : Entity
{
    public Guid ParticipantId { get; }

    public Reservation(Guid participantId, Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        ParticipantId = participantId;
    }
}