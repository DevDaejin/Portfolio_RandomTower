import json
from room_manager import RoomManager

room_manager = RoomManager()

async def handle_connection(websocket):
    print(f"[Connect] {websocket.remote_address}")
    try:
        async for msg in websocket:
            print(f"[Recv] {msg}")
            data = json.loads(msg)
            msg_type = data.get("type")

            if msg_type == "create_room":
                await room_manager.create_room(websocket, data)

            elif msg_type == "list_rooms":
                await room_manager.list_rooms(websocket)

            elif msg_type == "join_room":
                await room_manager.join_room(websocket, data)

            elif msg_type == "get_room_info":
                await room_manager.get_room_info(websocket, data)

            elif msg_type == "sync":
                await room_manager.sync_object(websocket, data)

            elif msg_type == "spawn":
                await room_manager.sync_object(websocket, data)

            elif msg_type == "leave_room":
                await room_manager.leave_room(websocket, data)

    except Exception as e:
        print(f"[Error] {e}")
    finally:
        print(f"[Disconnect] {websocket.remote_address}")
        room_manager.remove_client(websocket)
