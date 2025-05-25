import json
import uuid

class Room:
    def __init__(self, name):
        self.id = str(uuid.uuid4())
        self.name = name
        self.clients = []

    def info(self):
        return{
            "id": self.id,
            "name": self.name,
            "client_count": len(self.clients)
        }
    
class RoomManager:
    def __init__(self):
        self.rooms = {}

    async def create_room(self, ws, data):
        name = data.get("name", "DefaultRoom")
        room = Room(name)
        self.rooms[room.id] = room
        print(f"[Room Created] {room.id} - {room.name}")
        await ws.send(json.dumps({
            "type": "room_created",
            "room_id": room.id,
            "name": room.name
        }))


    async def join_room(self, ws, data):
        room_id = data.get("room_id")
        room = self.rooms.get(room_id)
        if room:
            room.clients.append(ws)
            print(f"[Room Joined] {room_id} - {room.name} | Clients: {len(room.clients)}")
            await ws.send(json.dumps({
                "type": "room_joined",
                "room_id": room.id,
                "name": room.name
            }))
        else:
            print(f"[Join Failed] Room not found: {room_id}")
            await ws.send(json.dumps({"type": "error", "message": "room not found"}))
            
    async def list_rooms(self, ws):
        room_list = [room.info() for room in self.rooms.values()]
        await ws.send(json.dumps({
            "type": "room_list",
            "rooms": room_list
        }))
        print(f"[Room List Sent] {len(room_list)} rooms")

    async def get_room_info(self, ws, data):
        room_id = data.get("room_id")
        room = self.rooms.get(room_id)
        if room:
            await ws.send(json.dumps({
                "type": "room_info",
                "room_id": room.id,
                "client_count": len(room.clients)
            }))
        else:
            await ws.send(json.dumps({"type": "error", "message": "room not found"}))