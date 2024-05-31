import { Server as SocketIOServer } from 'socket.io';
import Client from './client';
import { ServerPacket } from './packets';
import C1AuthPacket from './packets/client/C1AuthPacket';
import { generateRandomNumber, randomString } from './utils/random-utils';

export default class Server {
  private io: SocketIOServer;
  private players: Map<string, Client>;

  constructor(io: SocketIOServer) {
    this.io = io;
    this.players = new Map();
  }

  broadcastPacket(
    channel: string,
    packet: ServerPacket,
    filter?: (client: Client) => boolean,
  ) {
    for (const player of this.getPlayers()) {
      if (filter && !filter(player)) {
        continue;
      }

      player.sendPacket(channel, packet);
    }
  }

  init() {
    this.io.on('connect', (clientSocket) => {
      clientSocket.on('C1AuthPacket', (packet: C1AuthPacket) => {
        try {
          const token = packet.token;

          const id = randomString(32);
          const username = `${token}#${generateRandomNumber(1, 9999)}`;
          const client = new Client(this, clientSocket, id, username);
          client.init();
          this.players.set(client.playerId, client);

          clientSocket.on('disconnect', () => {
            this.players.delete(client.playerId);
            console.log(`Player ${client.playerUsername} disconnected`);
          });
        } catch (error) {
          console.error('Error during client authentication:', error);
          clientSocket.disconnect();
        }
      });
    });
  }

  getPlayers() {
    return this.players.values();
  }

  removePlayer(playerId: string) {
    this.players.delete(playerId);
  }
}
