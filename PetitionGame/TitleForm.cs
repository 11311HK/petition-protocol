using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>타이틀 화면 — bg_title + 제목 + [시작].</summary>
    public class TitleForm : SceneForm
    {
        public event Action StartRequested;

        public TitleForm()
        {
            var bg = new AssetPanel("bg_title.png") { Dock = DockStyle.Fill };
            Controls.Add(bg);

            var title = new Label
            {
                Text = "오늘의 청원을\n처리하시겠습니까?",
                Font = UiTheme.Title(46f, FontStyle.Bold),
                ForeColor = Color.WhiteSmoke,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(1000, 220),
                Location = new Point((1280 - 1000) / 2, 150)
            };
            bg.Controls.Add(title);

            var subtitle = new Label
            {
                Text = "제3심사과 · 자애부 청원심사국",
                Font = UiTheme.Body(16f),
                ForeColor = Color.Gainsboro,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(800, 40),
                Location = new Point((1280 - 800) / 2, 392)
            };
            bg.Controls.Add(subtitle);

            var start = new Button
            {
                Text = "심사 시작",
                Font = UiTheme.Body(18f, FontStyle.Bold),
                Size = new Size(220, 60),
                Location = new Point((1280 - 220) / 2, 480),
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.Accent,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            start.FlatAppearance.BorderColor = Color.WhiteSmoke;
            start.FlatAppearance.BorderSize = 1;
            start.Click += (s, e) => StartRequested?.Invoke();
            bg.Controls.Add(start);

            var hint = new Label
            {
                Text = "발표용 체험판 — 프롤로그 + 1일차 튜토리얼",
                Font = UiTheme.Body(11f),
                ForeColor = Color.FromArgb(180, 180, 188),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(700, 30),
                Location = new Point((1280 - 700) / 2, 660)
            };
            bg.Controls.Add(hint);
        }
    }
}
