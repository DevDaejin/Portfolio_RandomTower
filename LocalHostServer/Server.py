import asyncio
import websockets

connected_clients = set()
addr = "0.0.0.0"
port = 8765

async def handler(websocket):
    connected_clients.add(websocket)
    try:
        async for message in websocket:
            if isinstance(message, bytes):
                print(f"📦 바이너리 메시지 수신 ({len(message)} bytes)")
                await broadcast(message)
            else:
                print(f"⚠️ 텍스트 메시지 무시: {message}")
    except websockets.exceptions.ConnectionClosed:
        print("연결 끊김")
    finally:
        connected_clients.remove(websocket)

async def broadcast(message):
    if connected_clients:
        await asyncio.gather(*(client.send(message) for client in connected_clients))

async def main():
    print(f"🔌 웹소켓 서버 실행 중 (포트: {port})")
    async with websockets.serve(handler, addr, port):
        await asyncio.Future()  # 서버 유지

if __name__ == "__main__":
    asyncio.run(main())