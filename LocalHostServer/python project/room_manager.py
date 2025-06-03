import uuid
from proto.room_pb2 import RoomCreated, RoomJoined, RoomList, RoomLeft, RoomPacket, RoomInfo, JoinRoomRequest
from proto.net_pb2 import Envelope

class Room:
    def __init__(self, room_id, name):
        self.room_id = room_id
        self.name = name
        self.clients = set()

class RoomManager:
    def __init__(self):
        self.rooms = {}

    async def create_room(self, client, req):
        name = req.name or "Unnamed Room"
        room_id = str(uuid.uuid4())
        room = Room(room_id, name)
        self.rooms[room_id] = room
        print(f"[Room] Created room {room_id} with name '{name}'")
        await self.join_room(client, JoinRoomRequest(room_id=room_id))

        created = RoomCreated(room_id=room_id, name=name, client_id=client.client_id)
        await self._send(client, created)

    async def join_room(self, client, req):
        room = self.rooms.get(req.room_id)
        if not room:
            print(f"[JoinRoom] Room not found: {req.room_id}")
            return

        if client.room_id:
            await self.leave_room(client)

        room.clients.add(client)
        client.room_id = req.room_id
        print(f"[Room] Client {client.client_id} joined room {req.room_id}")
        joined = RoomJoined(room_id=room.room_id, name=room.name, client_id=client.client_id)
        await self._send(client, joined)

    async def leave_room(self, client):
        room_id = client.room_id
        if not room_id:
            return

        room = self.rooms.get(room_id)
        if room and client in room.clients:
            room.clients.discard(client)
            print(f"[Room] Client {client.client_id} left room {room_id}")
            if not room.clients:
                print(f"[Room] Room {room_id} is now empty and will be deleted")
                del self.rooms[room_id]
        client.room_id = None
        left = RoomLeft(room_id=room_id)
        await self._send(client, left)

    async def list_rooms(self, client):
        print(f"[Room] Sending room list to client {client.client_id}")
        rooms_info = [RoomInfo(room_id=r.room_id, name=r.name, client_count=len(r.clients)) for r in self.rooms.values()]
        room_list = RoomList(rooms=rooms_info)
        await self._send(client, room_list)
    
    def get_room(self, client):
        if not client.room_id:
            return None
        return self.rooms.get(client.room_id)

    async def _send(self, client, room_message):
        packet = RoomPacket()
        
        if isinstance(room_message, RoomCreated):
            packet.room_created.CopyFrom(room_message)
        elif isinstance(room_message, RoomJoined):
            packet.room_joined.CopyFrom(room_message)
        elif isinstance(room_message, RoomList):
            packet.room_list.CopyFrom(room_message)
        elif isinstance(room_message, RoomLeft):
            packet.room_left.CopyFrom(room_message)

        envelope = Envelope()
        envelope.type = "room"
        envelope.payload = packet.SerializeToString()

        await client.websocket.send(envelope.SerializeToString())
