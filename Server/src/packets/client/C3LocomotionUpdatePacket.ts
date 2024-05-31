import LocomotionState from '../../protocol/locomotion-state';

export default interface C3LocomotionUpdatePacket {
  previous: LocomotionState;
  current: LocomotionState;
}
