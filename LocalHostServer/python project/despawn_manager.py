from proto.net_pb2 import Envelope
from proto.despawn_pb2 import DespawnPacketData, DespawnEnemyPacket, DespawnTowerPacket, DespawnProjectilePacket

class DespawnManager:
    def __init__(self, room_manager):
        self.room_manager = room_manager

    async def handle_despawn(self, sender, despawn_packet: DespawnPacketData):
        room = self.room_manager.get_room(sender)
        if not room:
            print(f"[Despawn] Invalid room for client {sender.client_id}")
            return
        
        despawn_type = despawn_packet.despawn_type.lower()

        if despawn_type == "enemy":
            packet = DespawnEnemyPacket()
        elif despawn_type == "tower":
            packet = DespawnTowerPacket()
        elif despawn_type == "projectile":
            packet = DespawnProjectilePacket()
        else:
            print(f"[Despawn] Unknown despawn_type: {despawn_type}")
            return

        try:
            packet.ParseFromString(despawn_packet.payload)
        except Exception as e:
            print(f"[DespawnError] Failed to parse payload: {e}")
            return

        envelope = Envelope()
        envelope.type = "despawn"
        envelope.payload = despawn_packet.SerializeToString()

        for client in room.clients:
            if client != sender:
                await client.websocket.send(envelope.SerializeToString())