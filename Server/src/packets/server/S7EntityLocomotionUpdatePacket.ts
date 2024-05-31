import LocomotionState from '../../protocol/locomotion-state';

export default interface S7EntityLocomotionUpdatePacket {
  entityId: number;
  locomotion: LocomotionState;
}
