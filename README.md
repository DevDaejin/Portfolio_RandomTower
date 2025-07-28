# Portfolio_RandomTower
## 개요
본 프로젝트는 랜덤 타워 디펜스 장르를 구현하면서, 게임 로직 외에 네트워크 구조 설계까지 함께 경험해보기 위해 진행한 포트폴리오입니다.

단순 싱글 플레이와 실시간 멀티 플레이 기능을 직접 구축한 소켓 서버를 통해 Unity에서 자주 사용 되는 포톤 네트워크와 유사한 방식의 매치메이킹 시스템을 설계, 구현하는 것을 목표로 하였습니다.

## 플레이 화면

## 실행 파일
- Client
- Server

## 기술 스택

### 클라이언트
  - Unity 6000.0.38f1
  - C#
  - Native WebSocket
  - ProtoBuf
    
### 서버
  - Python 3.13
  - async.io(WebSocket 기반 비동기 통신)
  - ProtoBuf
    
## 외부 라이브러리
### 네트워킹
  - [Native Websockets](https://github.com/endel/NativeWebSocket)
  - [Protobuf 3](https://protobuf.dev/)
### 리소스
- 3D Model
  - [Kawaii Slimes](https://assetstore.unity.com/packages/3d/characters/creatures/kawaii-slimes-221172)
  - [Dog Knight PBR Polyart](https://assetstore.unity.com/packages/3d/characters/animals/dog-knight-pbr-polyart-135227)
  - [RPG Tiny Hero Duo PBR Polyart](https://assetstore.unity.com/packages/3d/characters/humanoids/rpg-tiny-hero-duo-pbr-polyart-225148)
  - [Battle Wizard Poly Art](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/battle-wizard-poly-art-128097)
  - [Low Poly Soldiers Demo](https://assetstore.unity.com/packages/3d/characters/low-poly-soldiers-demo-73611)
  - [Robot Hero : PBR HP Polyart](https://assetstore.unity.com/packages/3d/characters/robots/robot-hero-pbr-hp-polyart-106154)
  - [Giant Monster Model - Golem](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/giant-monster-model-golem-278960)
- Enviroment
  - [RPG Poly Pack - Lite](https://assetstore.unity.com/packages/3d/environments/landscapes/rpg-poly-pack-lite-148410)
- UI
  - [Dark Theme UI](https://assetstore.unity.com/packages/2d/gui/dark-theme-ui-199010#content)
- Sound
  - [Shooting Sound](https://assetstore.unity.com/packages/audio/sound-fx/shooting-sound-177096)
  - [Action Arcade Music Pack: Neon Pulse](https://assetstore.unity.com/packages/audio/music/electronic/action-arcade-music-pack-neon-pulse-312286)
- VFX
  - [Slash Effects FREE](https://assetstore.unity.com/packages/vfx/particles/spells/slash-effects-free-295209)
