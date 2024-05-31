public class C3LocomotionUpdatePacket
{
    public EntityAnimator.LocomotionState previous;
    public EntityAnimator.LocomotionState current;

    public C3LocomotionUpdatePacket(EntityAnimator.LocomotionState prev, EntityAnimator.LocomotionState curr)
    {
        previous = prev;
        current = curr;
    }
}