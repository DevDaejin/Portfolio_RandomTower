class ClientContext:
    def __init__(self, client_id, websocket):
        self.client_id = client_id
        self.websocket = websocket
        self.room_id = None