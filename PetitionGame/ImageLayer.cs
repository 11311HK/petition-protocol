using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// 고품질 이미지 레이어 — 이미지를 회전 + 반투명 + 고품질 보간으로 직접 그린다.
    /// 배경(부모)이 비치도록 투명 배경으로 그리며, 클릭은 통과시킨다.
    /// · 결정 도장: 회전/반투명으로 사용
    /// · 증명사진: 회전 0·불투명으로 사용 (저품질 스케일링 모아레 방지)
    /// 항상 정사각(min(W,H)) 영역에 맞춰 가운데 정렬한다(= Zoom).
    /// </summary>
    public class ImageLayer : Control
    {
        private readonly Image _img;
        private readonly float _angle;    // 기울기(도)
        private readonly float _opacity;  // 0~1

        public ImageLayer(string file, float angleDeg = 0f, float opacity = 1f)
        {
            _img = AssetLoader.LoadImage(file);
            _angle = angleDeg;
            _opacity = Math.Max(0f, Math.Min(1f, opacity));

            SetStyle(ControlStyles.SupportsTransparentBackColor
                   | ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Enabled = false; // 클릭 통과
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_img == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.TranslateTransform(Width / 2f, Height / 2f);
            if (_angle != 0f) g.RotateTransform(_angle);

            float side = Math.Min(Width, Height);
            var dest = new[]
            {
                new PointF(-side / 2f, -side / 2f), // 좌상
                new PointF( side / 2f, -side / 2f), // 우상
                new PointF(-side / 2f,  side / 2f)  // 좌하
            };

            if (_opacity >= 1f)
            {
                g.DrawImage(_img, dest);
            }
            else
            {
                var cm = new ColorMatrix { Matrix33 = _opacity };
                using var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);
                g.DrawImage(_img, dest,
                    new RectangleF(0, 0, _img.Width, _img.Height),
                    GraphicsUnit.Pixel, ia);
            }
        }
    }
}
