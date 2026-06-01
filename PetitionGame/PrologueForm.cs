using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// 프롤로그 — 카드 5장. 배경 전환 + 내레이션, 클릭/엔터로 진행.
    /// 마지막 카드 다음 단계에서 Finished 발생 → 책상(튜토리얼)으로.
    /// </summary>
    public class PrologueForm : SceneForm
    {
        private readonly List<(string Bg, string Text)> _cards = GameData.PrologueCards();
        private int _index = -1;

        private Panel _bgHost;
        private AssetPanel _bg;
        private Label _narration;
        private Label _hint;

        public event Action Finished;

        public PrologueForm()
        {
            _bgHost = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Ink };
            Controls.Add(_bgHost);

            // 하단 내레이션 띠 (반투명 느낌의 어두운 박스)
            var band = new Panel
            {
                BackColor = Color.FromArgb(18, 18, 22),
                Size = new Size(1280, 200),
                Location = new Point(0, 520)
            };
            _bgHost.Controls.Add(band);
            band.BringToFront();

            _narration = new Label
            {
                Font = UiTheme.Title(22f, FontStyle.Regular),
                ForeColor = Color.WhiteSmoke,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            band.Controls.Add(_narration);

            _hint = new Label
            {
                Text = "▶ 화면을 클릭하면 다음으로",
                Font = UiTheme.Body(11f),
                ForeColor = Color.FromArgb(170, 170, 178),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Size = new Size(1240, 24),
                Location = new Point(20, 165)
            };
            band.Controls.Add(_hint);
            _hint.BringToFront();

            // 클릭으로 진행 (배경/띠/내레이션 어디를 눌러도 동작)
            _bgHost.Click += (s, e) => Advance();
            band.Click += (s, e) => Advance();
            _narration.Click += (s, e) => Advance();
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                    Advance();
            };

            Advance(); // 첫 카드 표시
        }

        private void Advance()
        {
            _index++;
            if (_index >= _cards.Count)
            {
                Finished?.Invoke();
                return;
            }

            var card = _cards[_index];

            // 배경 교체
            _bg?.Dispose();
            _bg = new AssetPanel(card.Bg) { Dock = DockStyle.Fill };
            _bgHost.Controls.Add(_bg);
            _bg.SendToBack();
            _bg.Click += (s, e) => Advance();

            _narration.Text = card.Text;

            if (_index == _cards.Count - 1)
                _hint.Text = "▶ 클릭하면 첫 청원이 시작됩니다";
        }
    }
}
