# 오늘의 청원을 처리하시겠습니까? — 발표용 데모

타이틀 → 프롤로그 → 1일차 튜토리얼을 순수 C#(**WinForms**)로 만든 실행 가능한 발표 데모입니다.
게임 엔진 미사용. 도메인 모델(`IdCard`/`Petition`/`Rulebook`/`Stats`)과 UI를 분리했습니다.

> 참고: 첨부 플랜은 기존 `Program.cs`의 모델을 재사용하라고 했으나, 저장소에 해당 파일이 없어
> 플랜 §4 명세대로 동일한 형태의 모델을 `Models.cs`에 재구성했습니다. 실제 `Program.cs`가 있으면
> `Models.cs`의 클래스 정의를 그쪽 정의로 교체하면 됩니다.

## 빌드 & 실행 (Windows 전용)

WinForms는 Windows에서만 빌드/실행됩니다.

```bash
cd PetitionGame
dotnet run
```

또는 Visual Studio에서 `PetitionGame.csproj`를 열어 실행.

- 대상 프레임워크: `net8.0-windows`
- .NET 8 SDK 필요

## 씬 흐름

```
TitleForm ─시작─▶ PrologueForm (카드 5장, 배경 전환, 클릭/엔터 진행)
   └▶ DeskForm (튜토리얼 청원 4장) ─▶ DayEndForm (정산) ─▶ 종료
```

## 에셋

`PetitionGame/assets/`에 이미지를 넣으면 로드되고, 없으면 회색 박스 + 파일명으로 폴백합니다.
필요한 파일 목록은 [`PetitionGame/assets/README.md`](PetitionGame/assets/README.md) 참고.

## 조작

- **타이틀**: [심사 시작] 클릭
- **프롤로그**: 화면 클릭 또는 Enter/Space로 다음 카드
- **책상**: 청원서·신분증을 비교 → [통과]/[기각]/([이첩]) → 결과 확인 → [다음 청원]
- 4번째 청원에서 [이첩] 버튼과 스탯(양심·죄악·의심)이 처음 등장합니다.

## 튜토리얼 규정 (1일차)

- 신청인 = 신분증 이름 일치
- 거주구역 = 신분증 거주구역 일치
- 황제 도장 유효
- 금지 신청사항 = { 집회 허가, 금서 반환 }

## 파일 구성

| 파일 | 역할 |
|------|------|
| `Models.cs` | 도메인 모델 (IdCard/Petition/Rulebook/Stats) — 재사용 대상 |
| `GameData.cs` | 프롤로그 카드·튜토리얼 청원 데이터 (§4) |
| `AssetLoader.cs` | 이미지 로드 + 회색 박스 폴백 (`AssetPanel`) |
| `UiTheme.cs` | 색/폰트 (제목 명조·본문 고딕, 폴백 포함) |
| `StatBar.cs` | 스탯 표시 바 (+ 변화 깜빡임 연출) |
| `SceneForm.cs` | 씬 공통 베이스 (16:9 고정 + 페이드 인) |
| `Program.cs` | 진입점 + 씬 흐름 제어(`GameContext`) |
| `TitleForm.cs` / `PrologueForm.cs` / `DeskForm.cs` / `DayEndForm.cs` | 각 씬 |
