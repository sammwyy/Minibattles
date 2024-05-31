import S1AuthSuccessPacket from './server/S1AuthSuccessPacket';
import S2EntityMovementPacket from './server/S2EntityMovementPacket';
import S3EntitySpawnPacket from './server/S3EntitySpawnPacket';
import S4EntityFocusPacket from './server/S4EntityFocusPacket';
import S5PlayerConnectPacket from './server/S5PlayerConnectPacket';
import S7EntityLocomotionUpdatePacket from './server/S7EntityLocomotionUpdatePacket';
import S8EntityRemovePacket from './server/S8EntityRemovePacket';

export type ServerPacket =
  | S1AuthSuccessPacket
  | S2EntityMovementPacket
  | S3EntitySpawnPacket
  | S4EntityFocusPacket
  | S5PlayerConnectPacket
  | S7EntityLocomotionUpdatePacket
  | S8EntityRemovePacket;
