import { Socket } from 'socket.io';
import { ServerPacket } from './packets';
import C2MovementPacket from './packets/client/C2MovementPacket';
import C3LocomotionUpdatePacket from './packets/client/C3LocomotionUpdatePacket';
import S2EntityMovementPacket from './packets/server/S2EntityMovementPacket';
import S3EntitySpawnPacket from './packets/server/S3EntitySpawnPacket';
import S7EntityLocomotionUpdatePacket from './packets/server/S7EntityLocomotionUpdatePacket';
import S8EntityRemovePacket from './packets/server/S8EntityRemovePacket';
import EntityType from './protocol/entity-type';
import Server from './server';
import { generateRandomNumber } from './utils/random-utils';

export default class Client {
  private readonly server: Server;
  private readonly socket: Socket;
  public readonly playerId: string;
  public readonly playerUsername: string;

  private entityId: number;
  private x: number;
  private y: number;
  private z: number;
  private rotation: number;
  // private locomotion: LocomotionState;

  constructor(
    server: Server,
    socket: Socket,
    playerId: string,
    username: string,
  ) {
    this.server = server;
    this.socket = socket;
    this.playerId = playerId;
    this.playerUsername = username;
    // this.locomotion = LocomotionState.IDLE;

    this.entityId = -1;
    this.x = 0;
    this.y = 0;
    this.z = 0;
    this.rotation = 0;
  }

  createPositionPacket(): S2EntityMovementPacket {
    return {
      entityId: this.entityId,
      rot: this.rotation,
      x: this.x,
      y: this.y,
      z: this.z,
    };
  }

  createSpawnPacket(local: boolean): S3EntitySpawnPacket {
    return {
      entityId: this.entityId,
      entityType: EntityType.PLAYER,
      label: this.playerUsername,
      x: this.x,
      y: this.y,
      z: this.z,
      rot: this.rotation,
      withFocus: local,
      withTakeControl: local,
    };
  }

  sendPacket(channel: string, packet: ServerPacket) {
    this.socket.emit(channel, packet);
    console.log(`Send packet ${channel} for ${this.entityId}`);
  }

  sendPacketExcept(channel: string, packet: ServerPacket) {
    this.server.broadcastPacket(channel, packet, (another) => {
      return this.socket != another.socket;
    });
  }

  spawn(x: number, y: number, z: number, rot: number) {
    this.entityId = generateRandomNumber(1, 99999);
    this.x = x;
    this.y = y;
    this.z = z;
    this.rotation = rot;

    this.sendPacket('S3EntitySpawnPacket', this.createSpawnPacket(true));
    this.sendPacketExcept('S3EntitySpawnPacket', this.createSpawnPacket(false));
  }

  init() {
    this.sendPacket('S1AuthSuccessPacket', {
      playerId: this.playerId,
      playerUsername: this.playerUsername,
    });

    this.spawn(0, 10, 0, 0);

    for (const client of this.server.getPlayers()) {
      const packet = client.createSpawnPacket(false);
      this.sendPacket('S3EntitySpawnPacket', packet);
      console.log(
        `Sending new entity spawn (${client.entityId}) for ${this.entityId}`,
      );
    }

    this.socket.on('C2MovementPacket', (packet: C2MovementPacket) => {
      if (this.validateMovementPacket(packet)) {
        this.x = packet.x;
        this.y = packet.y;
        this.z = packet.z;
        this.rotation = packet.rot;

        const out = this.createPositionPacket();
        this.sendPacketExcept('S2EntityMovementPacket', out);
      }
    });

    this.socket.on(
      'C3LocomotionUpdatePacket',
      (packet: C3LocomotionUpdatePacket) => {
        let locomotion = packet.current;
        // this.locomotion = locomotion;

        const out: S7EntityLocomotionUpdatePacket = {
          entityId: this.entityId,
          locomotion,
        };
        this.sendPacketExcept('S7EntityLocomotionUpdatePacket', out);
      },
    );

    this.socket.on('disconnect', () => {
      this.server.removePlayer(this.playerId);
      const out: S8EntityRemovePacket = { entityId: this.entityId };
      this.sendPacketExcept('S8EntityRemovePacket', out);
      console.log(`Player ${this.playerUsername} disconnected`);
    });
  }

  private validateMovementPacket(packet: C2MovementPacket): boolean {
    // Add validation logic, e.g., checking if values are within acceptable ranges
    return (
      typeof packet.x === 'number' &&
      typeof packet.y === 'number' &&
      typeof packet.z === 'number' &&
      typeof packet.rot === 'number'
    );
  }
}
