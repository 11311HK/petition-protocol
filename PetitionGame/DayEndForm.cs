using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>1일차 종료 — 정산(양심·죄악·의심) + 체험판 마무리.</summary>
    public class DayEndForm : SceneForm
    {
        public event Action ExitRequested;

        public DayEndForm(Stats stats)
        {
            var bg = new AssetPanel("bg_desk.png") { Dock = DockStyle.Fill };
            Controls.Add(bg);

            // 일반 Panel은 알파를 지원하지 않으므로 어두운 단색으로 화면을 덮는다.
            var veil = new Panel
            {
                BackColor = Color.FromArgb(16, 16, 20),
                Dock = DockStyle.Fill
            };
            bg.Controls.Add(veil);

            var title = new Label
            {
                Text = "1일차 종료",
                Font = UiTheme.Title(40f, FontStyle.Bold),
                ForeColor = Color.WhiteSmoke,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(1280, 80),
                Location = new Point(0, 110)
            };
            veil.Controls.Add(title);

            var tally = new Label
            {
                Text = $"양심  {stats.Conscience}\n죄악  {stats.Sin}\n의심  {stats.Suspicion}",
                Font = UiTheme.Body(24f, FontStyle.Bold),
                ForeColor = Color.Gainsboro,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(600, 200),
                Location = new Point((1280 - 600) / 2, 230)
            };
            veil.Controls.Add(tally);

            var closing = new Label
            {
                Text = "발표용 체험판 — 여기까지.",
                Font = UiTheme.Title(20f),
                ForeColor = Color.WhiteSmoke,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(900, 40),
                Location = new Point((1280 - 900) / 2, 450)
            };
            veil.Controls.Add(closing);

            var teaser = new Label
            {
                Text = "내일, 낯익은 얼굴이 청원서에 오를지도 모른다.",
                Font = UiTheme.Body(14f, FontStyle.Italic),
                ForeColor = Color.FromArgb(180, 180, 188),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(900, 30),
                Location = new Point((1280 - 900) / 2, 500)
            };
            veil.Controls.Add(teaser);

            var exit = new Button
            {
                Text = "종료",
                Font = UiTheme.Body(16f, FontStyle.Bold),
                Size = new Size(180, 54),
                Location = new Point((1280 - 180) / 2, 580),
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.Accent,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            exit.FlatAppearance.BorderColor = Color.WhiteSmoke;
            exit.Click += (s, e) => ExitRequested?.Invoke();
            veil.Controls.Add(exit);
        }
    }
}
