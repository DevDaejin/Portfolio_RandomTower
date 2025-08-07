# Portfolio_RandomTower
1인 개발로 진행하였고, 재직 중이라 다소 시간이 오래 걸렸습니다, 초회 릴리즈까지 약 3개월 소모 하였습니다.
개발 중 난항은 개발 보단 기획, 리소스 수급이였습니다. 현재도 관련으로 아쉬움이 많고 꾸준히 업데이트 진행해 보겠습니다.

## 개요
본 프로젝트는 랜덤 타워 디펜스 장르를 구현하면서, 게임 로직 외에 네트워크 구조 설계까지 함께 경험해보기 위해 진행한 포트폴리오입니다.

단순 싱글 플레이와 실시간 멀티 플레이 기능을 직접 구축한 소켓 서버를 통해 Unity에서 자주 사용 되는 포톤 네트워크와 유사한 방식의 매치메이킹 시스템을 설계, 구현하는 것을 목표로 하였습니다.

## 플레이 화면
[영상 보기](https://www.youtube.com/watch?v=Wm8T2JbfT-E&ab_channel=DevDaejin)

## 실행 파일
[Release](https://github.com/DevDaejin/Portfolio_RandomTower/releases/tag/Publish)

## 실행 방법

### 클라이언트
1. Release에서 "Portfolio_RTD_XXXXXX_XXXX.zip"파일을 다운로드 및 압축 해제
2. 압축 해제 한 경로 내 "RandomTower.exe"을 실행

### 서버
1. Release에서 "Portfolio_RTD_Server.exe" 파일을 다운로드
2. 서버를 구축할 윈도우 환경 컴퓨터에서 "Portfolio_RTD_Server.exe"을 실행
3. 접속을 시킬 IP를 입력, 하기 옵션 중 택일
  - 공인 IP : [IPv4 확인](https://whatismyipaddress.com/)
  - 로컬 IP : 127.0.0.1
    - 같은 공유기 환경 내에서 동작 시 유효
  - 모든 접근 허용 : 0.0.0.0
4. 접속 Port 입력, 하기 두 조건 충족하여야 함
  - 모뎀이나 공유기 환경에 따라서 원한는 포트를 개방하여야 합니다. [포트포워딩](https://www.google.com/search?q=%ED%8F%AC%ED%8A%B8%ED%8F%AC%EC%9B%8C%EB%94%A9&oq=%ED%8F%AC%ED%8A%B8%ED%8F%AC%EC%9B%8C%EB%94%A9&gs_lcrp=EgZjaHJvbWUyBggAEEUYOdIBCDI0NjVqMGoxqAIAsAIA&sourceid=chrome&ie=UTF-8)
  - PC 환경에 따라 방화벽에서 원하는 포트를 개방하여야 합니다. [방화벽 해제](https://www.google.com/search?q=%EB%B0%A9%ED%99%94%EB%B2%BD+%ED%95%B4%EC%A0%9C&oq=%EB%B0%A9%ED%99%94%EB%B2%BD+%ED%95%B4%EC%A0%9C&gs_lcrp=EgZjaHJvbWUyBggAEEUYOdIBCDE5MjFqMGo5qAIAsAIA&sourceid=chrome&ie=UTF-8)
5. 정상 실행 시 화면

    <img width="402" height="255" alt="image" src="https://github.com/user-attachments/assets/bc623a01-b066-49f9-ab22-9131b3674e31" />

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
  - [Robot Sphere](https://assetstore.unity.com/packages/3d/characters/robots/robot-sphere-136226)
- Enviroment
  - [RPG Poly Pack - Lite](https://assetstore.unity.com/packages/3d/environments/landscapes/rpg-poly-pack-lite-148410)
- UI
  - [Dark Theme UI](https://assetstore.unity.com/packages/2d/gui/dark-theme-ui-199010#content)
- Sound
  - [Shooting Sound](https://assetstore.unity.com/packages/audio/sound-fx/shooting-sound-177096)
  - [Action Arcade Music Pack: Neon Pulse](https://assetstore.unity.com/packages/audio/music/electronic/action-arcade-music-pack-neon-pulse-312286)
- VFX
  - [Slash Effects FREE](https://assetstore.unity.com/packages/vfx/particles/spells/slash-effects-free-295209)
