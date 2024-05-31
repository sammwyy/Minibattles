import EntityType from '../../protocol/entity-type';

export default interface S3EntitySpawnPacket {
  label: string;
  entityId: number;
  x: number;
  y: number;
  z: number;
  rot: number;
  withFocus: boolean;
  withTakeControl: boolean;
  entityType: EntityType;
}
