import uuid
import json

class Room:
    def __init__(self, room_id, name):
        self.room_id = room_id
        self.name = name
        self.clients = set()

class RoomManager:
    def __init__(self):
        self.rooms = {}

    async def create_room(self, client, data):
        name = data.get("name", "Unnamed Room")
        room_id = str(uuid.uuid4())
        room = Room(room_id, name)
        self.rooms[room_id] = room
        print(f"[Room] Created: {room_id} ({name})")
        await self.join_room(client, {"room_id": room_id})
        await self._send(client, {
            "type": "room_created",
            "room_id": room_id,
            "name": name
        })

    async def join_room(self, client, data):
        room_id = data.get("room_id")
        room = self.rooms.get(room_id)
        if not room:
            await self._send(client, {"type": "error", "message": "Room not found"})
            return
        if client.room_id:
            await self.leave_room(client)
        room.clients.add(client)
        client.room_id = room_id
        print(f"[Room] {client.client_id} joined {room_id}")
        await self._send(client, {
            "type": "room_joined",
            "room_id": room_id,
            "name": room.name
        })

    async def leave_room(self, client):
        room_id = client.room_id
        if not room_id or room_id not in self.rooms:
            return
        room = self.rooms[room_id]
        room.clients.discard(client)
        if not room.clients:
            del self.rooms[room_id]
        print(f"[Room] {client.client_id} left {room_id}")
        client.room_id = None
        await self._send(client, {
            "type": "room_left",
            "room_id": room_id
        })

    async def list_rooms(self, client):
        print(f"[RoomManager] list_rooms called by {client.client_id}")

        rooms_info = [
            {
                "room_id": room.room_id,
                "name": room.name,
                "client_count": len(room.clients)
            }
            for room in self.rooms.values()
        ]

        print(f"[RoomManager] returning {len(rooms_info)} rooms")

        payload = {
            "type": "room_list",
            "rooms": rooms_info
        }

        print("[Server] Sending room_list response:\n" + json.dumps(payload, indent=2))
        await self._send(client, payload)  # ✅ 딱 한 번만 전송


        
    async def relay_sync(self, sender, data):
        room_id = sender.room_id
        room = self.rooms.get(room_id)
        if not room:
            return
        for client in room.clients:
            if client != sender:
                await self._send(client, data)

    async def spawn_object(self, client, data):
        room_id = client.room_id
        if not room_id or room_id not in self.rooms:
            await self._send(client, {
                "type": "error",
                "message": "Cannot spawn: not in a valid room"
            })
            return

        prefab_name = data.get("prefab_name", "Unknown")
        object_id = str(uuid.uuid4())

        spawn_packet = {
            "type": "spawn",
            "object_id": object_id,
            "prefab_name": prefab_name,
            "room_id": room_id,
            "owner_id": client.client_id
        }

        for target in self.rooms[room_id].clients:
            await self._send(target, spawn_packet)

    async def _send(self, client, message_dict):
        try:
            await client.websocket.send(json.dumps(message_dict))
        except Exception as e:
            print(f"[Send Error] {e}")