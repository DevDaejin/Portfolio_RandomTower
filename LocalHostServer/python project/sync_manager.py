
from proto.net_pb2 import Envelope
from proto.sync_pb2 import SyncTransformData, SyncHPData, SyncProjectileData

class SyncManager:
    def __init__(self, room_manager):
        self.room_manager = room_manager

    async def handle_sync(self, sender, sync_type: str, payload_bytes: bytes):
        room = self.room_manager.get_room(sender)
        if not room:
            print(f"[Sync] Invalid room for client {sender.client_id}")
            return

        if sync_type == "transform":
            msg = SyncTransformData()
        elif sync_type == "hp":
            msg = SyncHPData()
        elif sync_type == "projectile":
            msg = SyncProjectileData()
        else:
            print(f"[Sync] Unknown sync type: {sync_type}")
            return

        msg.ParseFromString(payload_bytes)
        print(f"[Sync] Relaying {sync_type} from {sender.client_id}")

        envelope = Envelope()
        envelope.type = "sync"
        envelope.payload = payload_bytes

        for client in room.clients:
            if client != sender:
                await client.websocket.send(envelope.SerializeToString())
