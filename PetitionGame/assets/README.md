# 에셋 폴더 (§1 매니페스트)

이 폴더에 아래 파일들을 넣으면 자동으로 로드됩니다.
**파일이 없으면** 게임은 회색 박스 + 파일명 라벨로 폴백하므로,
아트가 없어도 개발/발표가 가능합니다. (일러스트 후조립)

## 배경 (16:9 권장, 1280×720)
- `bg_title.png` — 타이틀 화면
- `bg_prologue_city.png` — 프롤로그: 도시 전경
- `bg_prologue_plaza.png` — 프롤로그: 광장/대부
- `bg_prologue_corridor.png` — 프롤로그: 사무실 도착
- `bg_desk.png` — 책상 시점 (게임플레이 메인 배경, 중앙은 비어 있음)

## UI 부품 (글자 없음 — 텍스트는 코드로 오버레이)
- `petition_blank.png` · `id_blank.png`
- `stamp_approve.png` · `stamp_reject.png` · `stamp_refer.png`
- `photo_01.png` ~ `photo_04.png` — 무명 청원자 증명사진
- `icon_conscience.png` · `icon_sin.png` · `icon_suspicion.png`

> 빌드 시 이 폴더의 파일은 출력 폴더(`bin/.../assets`)로 복사됩니다.
