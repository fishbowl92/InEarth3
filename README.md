# InEarth3

Unity와 C#으로 제작한 카드·덱 기반 전투 프로젝트의 스크립트 보존 저장소입니다.
2022년에 정리한 코드 스냅숏을 바탕으로 하며, 전체 Unity 프로젝트와 게임 리소스는 포함하지 않습니다.
2026-09-07 검토에서 버프 해제 오류를 수정했습니다. [당시 원본](https://github.com/fishbowl92/InEarth3/tree/818936be937d9bea53e8eced8ac9991640bbc963)과 [후속 수정·검증 기록](./docs/BUFF_STATE_FIX.md)을 구분해 보존합니다.

## 개인 기여 바로 보기

[개인 기여 설명과 함수별 코드 확인 순서](./docs/PERSONAL_CONTRIBUTIONS.md)

| 대표 기능 | 개인 기여 범위 | 코드 바로 보기 |
| --- | --- | --- |
| 버프 상태 관리 | 비트 플래그 구조 제안 및 구현 | [`비트 상태 함수`](https://github.com/fishbowl92/InEarth3/blob/f5b16cf0a3c9c018ad3c72a753989034e89c7f4f/MapManager.cs#L1739-L1781) · [`추가·중첩 처리`](https://github.com/fishbowl92/InEarth3/blob/f5b16cf0a3c9c018ad3c72a753989034e89c7f4f/MapManager.cs#L1783-L1874) |

팀 전체 코드와 개인 기여를 구분했습니다. 아래 프로젝트 코드 개요는 맥락 설명이며, 파일 전체를 단독 작성했다는 의미는 아닙니다.

## 프로젝트 개요

| 구분 | 내용 |
| --- | --- |
| 개발 형태 | 팀 프로젝트 |
| 사용 기술 | Unity, C# |
| 주요 영역 | 카드·덱, 플레이어·몬스터 스킬, 버프와 전투 상태 |
| 저장소 상태 | 당시 작성한 스크립트를 보존한 코드 자료 |

## 담당 및 기여 범위

- 버프 종류별 상태 배열을 확장하는 방식 대신 비트 플래그 기반 존재 확인 구조 제안 및 구현
- 버프 존재 여부와 실제 수치·UI(User Interface, 사용자 인터페이스) 정보를 분리하여 관리

카드·덱·스킬의 다른 코드는 프로젝트 맥락으로 안내하며, 전체를 단독 구현한 것으로 표시하지 않습니다.

## 프로젝트 코드 개요

### 1. 비트 플래그 기반 버프 상태 관리

플레이어·골렘·몬스터의 버프 존재 여부를 대상별 정수값에 기록합니다. 비트 연산으로 버프를 활성화·확인·해제하고, 실제 수치와 UI 참조는 별도 목록에서 관리합니다.

```mermaid
flowchart LR
    A[버프 요청] --> B{대상에게 같은 버프가 있는가?}
    B -- 예 --> C[기존 수치 합산]
    B -- 아니요 --> D[버프 정보와 UI 추가]
    C --> E[능력치와 UI 갱신]
    D --> E
```

관련 코드:

- [MapManager.cs](./MapManager.cs): `setBuffStateGab`, `removeBuffStateGab`, `getBuffStateGab`, `getBuffNewVersion`

### 2. 스킬 데이터 구성

스킬의 이름, 등급, 대상, 비용·피해·반복 횟수, 효과 위치, 아이콘과 사운드를 `SkillBlock` 자산으로 구성했습니다. 스킬마다 달라지는 값을 Unity 에디터에서 설정할 수 있습니다.

관련 코드:

- [SkillBlock.cs](./scriptable/SkillBlock.cs)
- [EffectData.cs](./scriptable/EffectData.cs)
- [MonsterSkill.cs](./scriptable/MonsterSkill.cs)

### 3. 카드·덱 전투 흐름

보유 덱에서 전투용 카드를 구성하고, 카드 비용과 스킬 이벤트를 연결하며, 사용 전·사용 후 카드 목록을 관리합니다.

관련 코드:

- [MapManager.cs](./MapManager.cs): 카드 획득·사용 및 전투 상태
- [GameManager.cs](./GameManager.cs): 덱 데이터와 진행 상태

## 코드 안내 문서

- [핵심 코드 흐름](./docs/CODE_WALKTHROUGH.md)
- [현재 관점의 회고와 개선 방향](./docs/RETROSPECTIVE.md)
- [2026-09-07 버프 오류 수정과 회귀 테스트](./docs/BUFF_STATE_FIX.md)

## 저장소 제한사항

- 전체 Unity 프로젝트가 아니므로 이 저장소만으로 게임을 실행할 수 없습니다.
- 원본 씬, 프리팹, 사운드와 이미지 등 일부 리소스가 포함되어 있지 않습니다.
- 메모리나 처리 속도의 정량적인 개선 결과는 측정하지 않았으므로 성능 수치를 주장하지 않습니다.
- 당시 학습·개발 과정의 코드를 보존한 자료이므로 현재의 코드 작성 기준과 차이가 있습니다.
- 별도 라이선스를 제공하지 않으며, 공개 저장소는 오픈소스 사용 허가를 의미하지 않습니다.
