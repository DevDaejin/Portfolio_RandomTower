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
            "name": room.name,
            "client": client.client_id
        })

    async def leave_room(self, client):
        room_id = client.room_id
        if not room_id:
            print(f"[LeaveRoom] {client.client_id} is not in any room.")
            return

        room = self.rooms.get(room_id)
        if not room:
            print(f"[LeaveRoom] Room {room_id} does not exist.")
            client.room_id = None
            return

        if client not in room.clients:
            print(f"[LeaveRoom] {client.client_id} not found in room {room_id}.")
        else:
            room.clients.discard(client)
            print(f"[Room] {client.client_id} left {room_id}")

        if not room.clients:
            print(f"[Room] {room_id} is empty. Deleting room.")
            del self.rooms[room_id]

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
        await self._send(client, payload) 


        
    async def relay_sync(self, sender, data):
        room_id = sender.room_id
        room = self.rooms.get(room_id)
        if not room:
            return
        for client in room.clients:
            if client != sender:
                await self._send(client, data)

    async def spawn_enemy(self, client, data):
        room_id = client.room_id
        if not room_id or room_id not in self.rooms:
            await self._send(client, {
                "type": "error",
                "message": "Cannot spawn: not in a valid room"
            })
            return

        enemy_id = data.get("enemy_id")
        object_id = data.get("object_id")

        spawn_packet = {
            "type": "spawn_enemy",
            "enemy_id" : enemy_id,
            "object_id": object_id,
            "room_id": room_id,
            "owner_id": client.client_id
        }

        for target in self.rooms[room_id].clients:
            if target != client:
                await self._send(target, spawn_packet)


    async def spawn_tower(self, client, data):
        room_id = client.room_id
        print(f"[spawn_tower] sender: {client.client_id} in room {room_id}")

        if not room_id or room_id not in self.rooms:
            print(f"[spawn_tower] invalid room_id")
            return

        tower_id = data.get("tower_id")
        object_id = data.get("object_id")

        spawn_packet = {
            "type": "spawn_tower",
            "tower_id": tower_id,
            "object_id": object_id,
            "room_id": room_id,
            "owner_id": client.client_id
        }

        for target in self.rooms[room_id].clients:
            print(f" → target {target.client_id}")
            if target != client:
                await self._send(target, spawn_packet)

    async def _send(self, client, message_dict):
        try:
            await client.websocket.send(json.dumps(message_dict))
        except Exception as e:
            print(f"[Send Error] {e}")