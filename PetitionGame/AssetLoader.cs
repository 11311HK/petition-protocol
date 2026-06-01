using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// §1 에셋 로더. 우선순위: ① exe 임베디드 리소스 → ② 실행 파일 옆 assets/ 파일 → ③ 폴백(null).
    /// 파일이 전혀 없으면 AssetPanel 이 회색 박스 + 파일명으로 폴백한다.
    /// </summary>
    public static class AssetLoader
    {
        public static string AssetsDir { get; } =
            Path.Combine(AppContext.BaseDirectory, "assets");

        public static Image LoadImage(string fileName)
        {
            // ① 임베디드 리소스 (LogicalName = 파일명)
            try
            {
                var asm = typeof(AssetLoader).Assembly;
                using var s = asm.GetManifestResourceStream(fileName);
                if (s != null)
                {
                    using var tmp = Image.FromStream(s);
                    return new Bitmap(tmp);
                }
            }
            catch
            {
                // 다음 단계로
            }

            // ② 실행 파일 옆 assets/ 파일 (개발 편의)
            try
            {
                var path = Path.Combine(AssetsDir, fileName);
                if (File.Exists(path))
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    using var tmp = Image.FromStream(fs);
                    return new Bitmap(tmp);
                }
            }
            catch
            {
                // 폴백
            }

            return null;
        }
    }

    /// <summary>
    /// 이미지를 배경으로 채우는 패널. 파일이 없으면 회색 + 파일명 텍스트로 폴백.
    /// 폴백 텍스트는 자식 컨트롤이 아니라 패널이 직접 그리므로(z-order 안전),
    /// 폼이 위에 올린 오버레이(제목·버튼 등)를 가리지 않는다.
    /// </summary>
    public class AssetPanel : Panel
    {
        public string FileName { get; }
        public bool HasImage { get; }

        public AssetPanel(string fileName, ImageLayout layout = ImageLayout.Stretch)
        {
            FileName = fileName;
            DoubleBuffered = true;

            var img = AssetLoader.LoadImage(fileName);
            if (img != null)
            {
                HasImage = true;
                BackgroundImage = img;
                BackgroundImageLayout = layout;
            }
            else
            {
                HasImage = false;
                BackColor = Color.FromArgb(72, 72, 80);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (HasImage) return;

            var s = "[" + FileName + "]";
            using var font = new Font(FontFamily.GenericSansSerif, 8f);
            var size = e.Graphics.MeasureString(s, font);
            e.Graphics.DrawString(
                s, font, Brushes.Gainsboro,
                (Width - size.Width) / 2f, (Height - size.Height) / 2f);
        }
    }
}
