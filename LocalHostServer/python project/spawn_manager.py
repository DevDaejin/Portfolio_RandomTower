from proto.net_pb2 import Envelope
from proto.spawn_pb2 import SpawnEnemyPacket, SpawnTowerPacket, SpawnProjectilePacket

class SpawnManager:
    def __init__(self, room_manager):
        self.room_manager = room_manager

    async def handle_spawn_enemy(self, sender, payload_bytes):
        packet = SpawnEnemyPacket()
        packet.ParseFromString(payload_bytes)

        room = self.room_manager.get_room(sender)
        if not room:
            print(f"[Spawn] Invalid room for client {sender.client_id}")
            return

        print(f"[Spawn] Relaying enemy spawn from {sender.client_id} to room {room.room_id}")

        envelope = Envelope()
        envelope.type = "spawn_enemy"
        envelope.payload = packet.SerializeToString()

        for client in room.clients:
            if client != sender:
                await client.websocket.send(envelope.SerializeToString())

    async def handle_spawn_tower(self, sender, payload_bytes):
        packet = SpawnTowerPacket()
        packet.ParseFromString(payload_bytes)
        await self._relay(sender, packet, "spawn_tower")

    async def handle_spawn_projectile(self, sender, payload_bytes):
        packet = SpawnProjectilePacket()
        packet.ParseFromString(payload_bytes)
        await self._relay(sender, packet, "spawn_projectile")

    async def _relay(self, sender, packet, packet_type):
        room = self.room_manager.get_room(sender)
        if not room:
            print(f"[Spawn] Invalid room for client {sender.client_id}")
            return

        print(f"[Spawn] Relaying {packet_type} from {sender.client_id} to room {room.room_id}")

        envelope = Envelope()
        envelope.type = packet_type
        envelope.payload = packet.SerializeToString()

        for client in room.clients:
            if client != sender:
                await client.websocket.send(envelope.SerializeToString())


