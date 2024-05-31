import DisconnectReason from '../../protocol/disconnect-reason';

export default interface S5PlayerConnectPacket {
  playerId: string;
  reason: DisconnectReason;
}
