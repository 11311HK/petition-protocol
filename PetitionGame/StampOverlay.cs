using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// 결정 도장 오버레이 — 청원서 본문 위에 회전 + 반투명으로 도장을 찍는 별도 레이어.
    /// 배경(청원서 양식)이 비치도록 투명 배경으로 그린다.
    /// </summary>
    public class StampOverlay : Control
    {
        private readonly Image _img;
        private readonly float _angle;    // 기울기(도)
        private readonly float _opacity;  // 0~1

        public StampOverlay(string file, float angleDeg, float opacity)
        {
            _img = AssetLoader.LoadImage(file);
            _angle = angleDeg;
            _opacity = Math.Max(0f, Math.Min(1f, opacity));

            SetStyle(ControlStyles.SupportsTransparentBackColor
                   | ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Enabled = false; // 클릭 통과(아래 컨트롤에 영향 없음)
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_img == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.TranslateTransform(Width / 2f, Height / 2f);
            g.RotateTransform(_angle);

            float side = Math.Min(Width, Height);
            var dest = new[]
            {
                new PointF(-side / 2f, -side / 2f), // 좌상
                new PointF( side / 2f, -side / 2f), // 우상
                new PointF(-side / 2f,  side / 2f)  // 좌하
            };

            var cm = new ColorMatrix { Matrix33 = _opacity };
            using var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            g.DrawImage(_img, dest,
                new RectangleF(0, 0, _img.Width, _img.Height),
                GraphicsUnit.Pixel, ia);
        }
    }
}
