
from proto.net_pb2 import Envelope, SyncPacketData
from proto.sync_pb2 import SyncTransformData, SyncHPData, SyncProjectileData

class SyncManager:
    def __init__(self, room_manager):
        self.room_manager = room_manager

    async def handle_sync(self, sender, sync_packet: SyncPacketData):
        room = self.room_manager.get_room(sender)
        if not room:
            print(f"[Sync] Invalid room for client {sender.client_id}")
            return

        print(f"[Sync] Relaying {sync_packet.sync_type} from {sender.client_id} for object {sync_packet.object_id}")

        if sync_packet.sync_type == "transform":
            msg = SyncTransformData()
        elif sync_packet.sync_type == "hp":
            msg = SyncHPData()
        elif sync_packet.sync_type == "projectile":
            msg = SyncProjectileData()
        else:
            print(f"[Sync] Unknown sync type: {sync_packet.sync_type}")
            return

        try:
            msg.ParseFromString(sync_packet.payload)
        except Exception as e:
            print(f"[SyncError] Failed to parse {sync_packet.sync_type}: {e}")
            return

        envelope = Envelope()
        envelope.type = "sync"
        envelope.payload = sync_packet.SerializeToString()

        for client in room.clients:
            if client != sender:
                await client.websocket.send(envelope.SerializeToString())