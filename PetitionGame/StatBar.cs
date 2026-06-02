using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// 스탯 한 줄: [아이콘] 이름 [채움바] 값.
    /// 값 변경 시 잠깐 깜빡여 변화를 알린다.
    /// </summary>
    public class StatBar : Panel
    {
        private readonly Label _name;
        private readonly Label _valueLabel;
        private readonly Panel _barBg;
        private readonly Panel _barFill;
        private readonly Color _color;
        private int _value;

        public string StatName { get; }

        public StatBar(string statName, string iconFile, Color color)
        {
            StatName = statName;
            _color = color;
            Size = new Size(270, 36);
            BackColor = Color.FromArgb(26, 26, 32);

            var icon = new AssetPanel(iconFile)
            {
                Location = new Point(6, 6),
                Size = new Size(24, 24)
            };
            Controls.Add(icon);

            _name = new Label
            {
                Text = statName,
                Location = new Point(36, 0),
                Size = new Size(56, 36),
                ForeColor = Color.Gainsboro,
                Font = UiTheme.Body(11f),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            Controls.Add(_name);

            _barBg = new Panel
            {
                Location = new Point(96, 11),
                Size = new Size(130, 14),
                BackColor = Color.FromArgb(58, 58, 68)
            };
            _barFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(0, 14),
                BackColor = color
            };
            _barBg.Controls.Add(_barFill);
            Controls.Add(_barBg);

            _valueLabel = new Label
            {
                Text = "0",
                Location = new Point(232, 0),
                Size = new Size(34, 36),
                ForeColor = Color.White,
                Font = UiTheme.Body(12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            Controls.Add(_valueLabel);
        }

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                _valueLabel.Text = _value.ToString();
                _barFill.Width = Math.Min(_barBg.Width, Math.Max(0, _value) * 26);
            }
        }

        /// <summary>값을 올리고 깜빡이는 연출.</summary>
        public void Bump(int delta)
        {
            Value += delta;
            Flash();
        }

        private void Flash()
        {
            int blinks = 0;
            var baseColor = BackColor;
            var timer = new Timer { Interval = 120 };
            timer.Tick += (s, e) =>
            {
                BackColor = (blinks % 2 == 0) ? _color : baseColor;
                blinks++;
                if (blinks >= 6)
                {
                    BackColor = baseColor;
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }
    }
}
