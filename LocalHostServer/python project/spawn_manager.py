from proto.net_pb2 import Envelope
from proto.spawn_pb2 import SpawnPacketData, SpawnEnemyPacket, SpawnTowerPacket, SpawnProjectilePacket

class SpawnManager:
    def __init__(self, room_manager):
        self.room_manager = room_manager

    async def handle_spawn(self, sender, spawn_packet: SpawnPacketData):
        room = self.room_manager.get_room(sender)
        
        if not room:
            print(f"[Spawn] Invalid room for client {sender.client_id}")
            return
        
        spawn_type = spawn_packet.spawn_type.lower()

        if spawn_type == "enemy":
            packet = SpawnEnemyPacket()
        elif spawn_type == "tower":
            packet = SpawnTowerPacket()
        elif spawn_type == "projectile":
            packet = SpawnProjectilePacket()
        else:
            print(f"[Spawn] Unknown spawn_type: {spawn_type}")
            return

        try:
            packet.ParseFromString(spawn_packet.payload)
        except Exception as e:
            print(f"[SpawnError] Failed to parse payload: {e}")
            return
        
        envelope = Envelope()
        envelope.type = "spawn"
        envelope.payload = spawn_packet.SerializeToString()

        for client in room.clients:
            if client != sender:
                await client.websocket.send(envelope.SerializeToString())


