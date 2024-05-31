import cors from 'cors';
import express from 'express';
import helmet from 'helmet';
import http from 'http';
import { Server as SocketIO } from 'socket.io';
import Server from './server';

const app = express();
app.use(helmet());
app.use(cors());

const httpServer = http.createServer(app);
const io = new SocketIO(httpServer);

const server = new Server(io);
server.init();

const PORT = 18412;
httpServer.listen(PORT, () => {
  console.log(`Server listening on port ${PORT}`);
});
