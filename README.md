# TimingBridge

> 버튼을 누르는 타이밍으로 다리를 만들어 발판을 건너는 하이퍼캐주얼 게임

<p align="center">
  <img src="docs/screenshots/screenshot_1.png" width="200"/>
  <img src="docs/screenshots/screenshot_2.png" width="200"/>
  <img src="docs/screenshots/screenshot_3.png" width="200"/>
</p>

## 게임 소개

버튼을 누르고 있으면 다리가 자라고, 손을 떼면 다리가 쓰러집니다.
다리가 발판 위에 딱 닿도록 타이밍을 맞추세요.
너무 짧으면 추락, 너무 길면 실패 — 완벽한 타이밍이 핵심입니다.

## 플레이 영상 / 다운로드

<!-- 출시 후 링크 추가 -->
🔗 Google Play Store *(심사 중)*

## 기술 스택

- **Engine** : Unity 2022 (URP)
- **Language** : C#
- **Platform** : Android
- **Ad SDK** : Google AdMob
- **Architecture** : GameState FSM + Manager 패턴 + Object Pool

## 아키텍처

```
GameManager (MonoSingleton)
├── ObjectPoolManager   — 오브젝트 풀 생성/관리
├── PlayerManager       — 플레이어 이동 제어
├── BridgeManager       — 다리 결과 처리 및 스폰 트리거
├── GameUIManager       — UI 요소 관리
└── SoundManager        — BGM / SFX 재생
```

**GameState FSM**
```
WaitLoading → GameReady → GamePlay → GameOver
                                   → GameStop (일시정지)
                                   → GameClear
```

## 핵심 게임플레이 구현

| 클래스 | 역할 |
|--------|------|
| `CubeRoad` | 다리 성장/쓰러짐/판정 로직 |
| `BridgeSpawner` | 다리·발판 스폰 & 오브젝트 풀 관리 |
| `BridgeManager` | 성공/실패 분기 처리 |
| `PlayerMove` | 이동 코루틴 |

## 개발자

**박민재** — 1인 개발
📧 wer9384@naver.com
🔗 [개인정보처리방침](https://pmj9384.github.io/TimingBridge/)
