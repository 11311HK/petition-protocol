using System.Drawing;

namespace PetitionGame
{
    /// <summary>
    /// 공통 색/폰트. 제목은 명조(serif), 본문은 고딕(sans).
    /// 지정 폰트가 없으면 제네릭 패밀리로 폴백.
    /// </summary>
    public static class UiTheme
    {
        // 색상 팔레트 — 디스토피아 톤
        public static readonly Color Ink = Color.FromArgb(28, 28, 34);     // 배경 어두운 톤
        public static readonly Color Paper = Color.FromArgb(232, 226, 210); // 종이색
        public static readonly Color PaperInk = Color.FromArgb(40, 36, 30);
        public static readonly Color Accent = Color.FromArgb(176, 48, 48);  // 제국 적색
        public static readonly Color Conscience = Color.FromArgb(110, 170, 220);
        public static readonly Color Sin = Color.FromArgb(176, 48, 48);
        public static readonly Color Suspicion = Color.FromArgb(214, 178, 80);

        private static readonly string SerifName = PickFamily("Batang", "바탕", "Nanum Myeongjo", "Noto Serif CJK KR");
        private static readonly string GothicName = PickFamily("Malgun Gothic", "맑은 고딕", "Nanum Gothic", "Noto Sans CJK KR");

        public static Font Title(float size, FontStyle style = FontStyle.Bold)
            => new Font(SerifName, size, style);

        public static Font Body(float size, FontStyle style = FontStyle.Regular)
            => new Font(GothicName, size, style);

        private static string PickFamily(params string[] names)
        {
            foreach (var n in names)
            {
                try
                {
                    using var ff = new FontFamily(n);
                    return ff.Name; // 설치되어 있으면 사용
                }
                catch
                {
                    // 미설치 → 다음 후보
                }
            }
            return FontFamily.GenericSansSerif.Name;
        }
    }
}
