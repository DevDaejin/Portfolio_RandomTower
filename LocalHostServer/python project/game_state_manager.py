from proto.net_pb2 import Envelope
from proto.game_pb2 import GameStatePacket

class GameStateManager:
    def __init__(self, room_manager):
        self.room_manager = room_manager

    async def handle_game_state(self, sender, packet: GameStatePacket):
        room = self.room_manager.get_room(sender)
        if not room:
            print(f"[GameState] Invalid room for client {sender.client_id}")
            return
        
        envelope = Envelope()
        envelope.type = "game_state"
        envelope.payload = packet.SerializeToString()

        for client in room.clients:
            if client != sender:
                await client.websocket.send(envelope.SerializeToString())