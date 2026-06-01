using System.Collections.Generic;

namespace PetitionGame
{
    /// <summary>
    /// §4 콘텐츠. 프롤로그 카드와 튜토리얼 청원(전원 무명) 데이터를 제공한다.
    /// </summary>
    public static class GameData
    {
        /// <summary>오늘의 규정집: 금지 신청사항 = {집회 허가, 금서 반환}</summary>
        public static Rulebook TodayRules()
            => new Rulebook(new[] { "집회 허가", "금서 반환" });

        /// <summary>프롤로그 카드 5장 (배경 파일명, 내레이션).</summary>
        public static List<(string Bg, string Text)> PrologueCards() => new()
        {
            ("bg_prologue_city.png",
                "대혼란(大混亂).\n인류는 더 이상 스스로를 다스리지 못했다."),
            ("bg_prologue_city.png",
                "혼돈을 끝내기 위해, 인류는 통치권을 인공지능에게 넘겼다.\n그를 「대부(大父)」라 부른다."),
            ("bg_prologue_plaza.png",
                "「대부께서 그대를 헤아리고 계십니다」\n— 이제 모든 욕구는 청원으로 신청하고, 허가받아야 한다."),
            ("bg_prologue_corridor.png",
                "시민번호 ████.\n귀하는 모범 시민으로 선발되어, 자애부 청원심사국 제3심사과에 배치되었다."),
            ("bg_desk.png",
                "첫 출근. 책상 위로 첫 청원서가 내려온다.\n천장의 제안(帝眼)이, 당신을 바라본다."),
        };

        /// <summary>튜토리얼 청원 4장 — 전원 무명, 게임 방식만 설명.</summary>
        public static List<Petition> TutorialPetitions() => new()
        {
            // 1) 정상 — 통과
            new Petition(
                applicantName: "오정민", region: "제7거주구역", request: "거주구역 이전",
                hasValidStamp: true,
                id: new IdCard("오정민", "제7거주구역", "photo_01.png"),
                guide: "신청인·거주구역이 신분증과 같은지 보세요. 모두 일치하면 [통과]."),

            // 2) 이름 불일치 (박성우 ≠ 박선우) — 기각
            new Petition(
                applicantName: "박성우", region: "제3거주구역", request: "서적 소지 허가",
                hasValidStamp: true,
                id: new IdCard("박선우", "제3거주구역", "photo_02.png"),
                guide: "직접 찾아보세요. 어긋나는 항목이 있으면 [기각]."),

            // 3) 도장 누락 — 기각
            new Petition(
                applicantName: "정해광", region: "제12거주구역", request: "식량 배급 증액",
                hasValidStamp: false,
                id: new IdCard("정해광", "제12거주구역", "photo_03.png"),
                guide: "황제 도장도 확인 대상입니다. 흐릿하거나 누락이면 위반."),

            // 4) 금지 신청사항(집회 허가) — 이첩(선택). 여기서 스탯이 처음 등장.
            new Petition(
                applicantName: "한도경", region: "제5거주구역", request: "집회 허가",
                hasValidStamp: true,
                id: new IdCard("한도경", "제5거주구역", "photo_04.png"),
                guide: "오늘 규정상 처분 대상입니다. [이첩]은 정서감찰부 송치 — 여기서 스탯이 변합니다."),
        };
    }
}
