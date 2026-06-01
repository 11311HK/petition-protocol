using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// §1 에셋 매니페스트 로더.
    /// 파일이 있으면 이미지를, 없으면 회색 박스 + 파일명 라벨을 돌려준다.
    /// (일러스트 후조립 — 아트가 없어도 개발/발표가 가능하도록.)
    /// </summary>
    public static class AssetLoader
    {
        public static string AssetsDir { get; } =
            Path.Combine(AppContext.BaseDirectory, "assets");

        /// <summary>이미지를 로드한다. 없으면 null.</summary>
        public static Image LoadImage(string fileName)
        {
            try
            {
                var path = Path.Combine(AssetsDir, fileName);
                if (File.Exists(path))
                {
                    // 파일 잠금을 피하기 위해 스트림 복사로 로드
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    using var tmp = Image.FromStream(fs);
                    return new Bitmap(tmp);
                }
            }
            catch
            {
                // 손상된 파일 등 → 폴백
            }
            return null;
        }
    }

    /// <summary>
    /// 이미지를 배경으로 채우는 패널. 파일이 없으면 회색 + 파일명 라벨로 폴백.
    /// 배경/UI 부품/사진/아이콘 모두 이것으로 표현한다.
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
                var label = new Label
                {
                    Text = "[" + fileName + "]",
                    ForeColor = Color.FromArgb(190, 190, 200),
                    Font = new Font(FontFamily.GenericSansSerif, 8f),
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                Controls.Add(label);
            }
        }
    }
}
