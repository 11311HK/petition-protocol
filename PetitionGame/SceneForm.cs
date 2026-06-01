using System.Drawing;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// 모든 씬의 공통 베이스. 16:9 고정 창 + 페이드 인 연출(§Phase 4).
    /// </summary>
    public class SceneForm : Form
    {
        private Timer _fadeTimer;

        public SceneForm()
        {
            Text = "오늘의 청원을 처리하시겠습니까?";
            ClientSize = new Size(1280, 720);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = UiTheme.Ink;
            DoubleBuffered = true;
            KeyPreview = true;
            Opacity = 0d;
        }

        protected override void OnShown(System.EventArgs e)
        {
            base.OnShown(e);
            StartFadeIn();
        }

        private void StartFadeIn()
        {
            _fadeTimer = new Timer { Interval = 20 };
            _fadeTimer.Tick += (s, ev) =>
            {
                Opacity += 0.08d;
                if (Opacity >= 1d)
                {
                    Opacity = 1d;
                    _fadeTimer.Stop();
                    _fadeTimer.Dispose();
                    _fadeTimer = null;
                }
            };
            _fadeTimer.Start();
        }
    }
}
