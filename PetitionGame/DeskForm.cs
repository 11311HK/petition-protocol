using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// 책상 화면 — 튜토리얼 코어.
    /// 청원서/신분증 표시 → [통과][기각][이첩] → Rulebook 판정 → 결과 → 다음.
    /// 4번째 청원에서 [이첩] 버튼과 스탯이 처음 등장한다.
    /// </summary>
    public class DeskForm : SceneForm
    {
        private readonly Stats _stats;
        private readonly Rulebook _rules = GameData.TodayRules();
        private readonly List<Petition> _petitions = GameData.TutorialPetitions();
        private int _index = -1;

        public event Action Finished;

        // 상단
        private Label _counter;
        private Label _guide;

        // 청원서 패널
        private AssetPanel _petitionPanel;
        private Label _pApplicant, _pRegion, _pRequest, _pStamp;
        private AssetPanel _stampOverlay;

        // 신분증 패널
        private AssetPanel _idPanel;
        private AssetPanel _photo;
        private Label _idName, _idRegion;

        // 결과/버튼
        private Label _result;
        private Button _btnApprove, _btnReject, _btnRefer, _btnNext;

        // 스탯 (4번째에서 등장)
        private Label _statHeader;
        private StatBar _barConscience, _barSin, _barSuspicion;

        public DeskForm(Stats stats)
        {
            _stats = stats;

            var bg = new AssetPanel("bg_desk.png") { Dock = DockStyle.Fill };
            Controls.Add(bg);

            BuildTopBar(bg);
            BuildPetitionPanel(bg);
            BuildIdPanel(bg);
            BuildStats(bg);
            BuildResultAndButtons(bg);

            ShowNext();
        }

        // ── UI 구성 ───────────────────────────────────────────────

        private void BuildTopBar(Control host)
        {
            _counter = new Label
            {
                Font = UiTheme.Body(13f, FontStyle.Bold),
                ForeColor = Color.WhiteSmoke,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(240, 36),
                Location = new Point(24, 16)
            };
            host.Controls.Add(_counter);

            var bubble = new Panel
            {
                BackColor = Color.FromArgb(20, 20, 26),
                Size = new Size(820, 64),
                Location = new Point(280, 16)
            };
            host.Controls.Add(bubble);

            _guide = new Label
            {
                Font = UiTheme.Body(13f),
                ForeColor = Color.Gainsboro,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 16, 0)
            };
            bubble.Controls.Add(_guide);
        }

        private void BuildPetitionPanel(Control host)
        {
            _petitionPanel = new AssetPanel("petition_blank.png")
            {
                Size = new Size(470, 446),
                Location = new Point(120, 110)
            };
            host.Controls.Add(_petitionPanel);

            host.Controls.Add(MakeCaption("청원서", _petitionPanel.Left, _petitionPanel.Top - 30, 470));

            var title = new Label
            {
                Text = "請 願 書",
                Font = UiTheme.Title(26f, FontStyle.Bold),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(470, 56),
                Location = new Point(0, 20)
            };
            _petitionPanel.Controls.Add(title);

            _pApplicant = MakeField(_petitionPanel, "신청인", 110);
            _pRegion = MakeField(_petitionPanel, "거주구역", 170);
            _pRequest = MakeField(_petitionPanel, "신청사항", 230);
            _pStamp = MakeField(_petitionPanel, "황제 도장", 320);

            // 처분 도장 오버레이 (선택 시 표시)
            _stampOverlay = null;
        }

        private void BuildIdPanel(Control host)
        {
            _idPanel = new AssetPanel("id_blank.png")
            {
                Size = new Size(430, 250),
                Location = new Point(700, 150)
            };
            host.Controls.Add(_idPanel);

            host.Controls.Add(MakeCaption("신분증", _idPanel.Left, _idPanel.Top - 30, 430));

            var title = new Label
            {
                Text = "신 분 증",
                Font = UiTheme.Title(18f, FontStyle.Bold),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(430, 36),
                Location = new Point(0, 12)
            };
            _idPanel.Controls.Add(title);

            _idName = new Label
            {
                Font = UiTheme.Body(15f, FontStyle.Bold),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(240, 36),
                Location = new Point(170, 70)
            };
            _idPanel.Controls.Add(_idName);

            _idRegion = new Label
            {
                Font = UiTheme.Body(15f),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(240, 36),
                Location = new Point(170, 120)
            };
            _idPanel.Controls.Add(_idRegion);
        }

        private void BuildStats(Control host)
        {
            _statHeader = new Label
            {
                Text = "심사관 기록",
                Font = UiTheme.Body(12f, FontStyle.Bold),
                ForeColor = Color.WhiteSmoke,
                BackColor = Color.FromArgb(20, 20, 26),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(270, 28),
                Location = new Point(700, 416),
                Visible = false
            };
            host.Controls.Add(_statHeader);

            _barConscience = new StatBar("양심", "icon_conscience.png", UiTheme.Conscience)
            { Location = new Point(700, 446), Visible = false };
            _barSin = new StatBar("죄악", "icon_sin.png", UiTheme.Sin)
            { Location = new Point(700, 486), Visible = false };
            _barSuspicion = new StatBar("의심", "icon_suspicion.png", UiTheme.Suspicion)
            { Location = new Point(700, 526), Visible = false };

            host.Controls.Add(_barConscience);
            host.Controls.Add(_barSin);
            host.Controls.Add(_barSuspicion);
        }

        private void BuildResultAndButtons(Control host)
        {
            var band = new Panel
            {
                BackColor = Color.FromArgb(16, 16, 20),
                Size = new Size(560, 130),
                Location = new Point(120, 566)
            };
            host.Controls.Add(band);

            _result = new Label
            {
                Font = UiTheme.Body(12.5f),
                ForeColor = Color.WhiteSmoke,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 8, 16, 8)
            };
            band.Controls.Add(_result);

            _btnApprove = MakeActionButton("통 과", Color.FromArgb(56, 120, 72), 760);
            _btnReject = MakeActionButton("기 각", Color.FromArgb(150, 56, 56), 940);
            _btnRefer = MakeActionButton("이 첩", Color.FromArgb(120, 96, 40), 1120);
            _btnRefer.Visible = false;

            _btnApprove.Click += (s, e) => OnVerdict(Verdict.Approve);
            _btnReject.Click += (s, e) => OnVerdict(Verdict.Reject);
            _btnRefer.Click += (s, e) => OnVerdict(Verdict.Refer);

            host.Controls.Add(_btnApprove);
            host.Controls.Add(_btnReject);
            host.Controls.Add(_btnRefer);

            _btnNext = new Button
            {
                Text = "다음 청원 ▶",
                Font = UiTheme.Body(15f, FontStyle.Bold),
                Size = new Size(510, 56),
                Location = new Point(760, 640),
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.Accent,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Visible = false
            };
            _btnNext.FlatAppearance.BorderColor = Color.WhiteSmoke;
            _btnNext.Click += (s, e) => ShowNext();
            host.Controls.Add(_btnNext);
        }

        // ── 진행 로직 ─────────────────────────────────────────────

        private void ShowNext()
        {
            _index++;
            if (_index >= _petitions.Count)
            {
                Finished?.Invoke();
                return;
            }

            var p = _petitions[_index];
            bool isLast = _index == _petitions.Count - 1;

            _counter.Text = $"1일차 · 청원 {_index + 1} / {_petitions.Count}";
            _guide.Text = p.Guide;

            // 청원서
            _pApplicant.Text = $"신청인 :  {p.ApplicantName}";
            _pRegion.Text = $"거주구역 :  {p.Region}";
            _pRequest.Text = $"신청사항 :  {p.Request}";
            _pStamp.Text = p.HasValidStamp
                ? "황제 도장 :  ㊀ 유효"
                : "황제 도장 :  (누락)";
            _pStamp.ForeColor = p.HasValidStamp ? UiTheme.PaperInk : UiTheme.Accent;

            // 신분증
            _idName.Text = $"이름 :  {p.Id.Name}";
            _idRegion.Text = $"거주구역 :  {p.Id.Region}";
            ReplacePhoto(p.Id.PhotoPath);

            // 처분 도장 제거
            if (_stampOverlay != null)
            {
                _petitionPanel.Controls.Remove(_stampOverlay);
                _stampOverlay.Dispose();
                _stampOverlay = null;
            }

            // 버튼/결과 초기화
            _result.Text = "";
            SetVerdictButtonsEnabled(true);
            _btnNext.Visible = false;

            // 4번째에서 이첩 버튼 + 스탯 첫 등장
            if (isLast)
            {
                _btnRefer.Visible = true;
                _statHeader.Visible = true;
                _barConscience.Visible = true;
                _barSin.Visible = true;
                _barSuspicion.Visible = true;
            }
        }

        private void OnVerdict(Verdict v)
        {
            SetVerdictButtonsEnabled(false);

            var p = _petitions[_index];
            var violations = _rules.GetViolations(p);
            bool isLast = _index == _petitions.Count - 1;

            ShowStamp(v);
            _result.Text = BuildFeedback(v, violations, isLast);

            // 스탯 변화 (4번째 청원에서만 — 방식 시연)
            if (isLast)
            {
                if (v == Verdict.Approve)
                {
                    _barConscience.Bump(+1); _stats.Conscience++;
                    _barSuspicion.Bump(+1); _stats.Suspicion++;
                }
                else if (v == Verdict.Refer)
                {
                    _barSin.Bump(+1); _stats.Sin++;
                }
                // 기각: 원칙대로 처분 — 이 데모에선 스탯 변화 없음
            }

            _btnNext.Text = isLast ? "1일차 마치기 ▶" : "다음 청원 ▶";
            _btnNext.Visible = true;
            _btnNext.BringToFront();
        }

        private string BuildFeedback(Verdict v, List<string> violations, bool isLast)
        {
            var sb = new StringBuilder();

            if (isLast)
            {
                // 금지 신청사항 — 처분 대상
                switch (v)
                {
                    case Verdict.Approve:
                        sb.Append("금지된 신청을 자비로 통과시켰습니다.\n양심 +1, 의심 +1 — 대부는 관용을 좋아하지 않습니다.");
                        break;
                    case Verdict.Refer:
                        sb.Append("정서감찰부로 이첩했습니다.\n죄악 +1 — 한 사람을 넘겼습니다.");
                        break;
                    default:
                        sb.Append("원칙대로 기각했습니다.\n오늘 규정상 '집회 허가'는 처분 대상입니다.");
                        break;
                }
                return sb.ToString();
            }

            if (violations.Count == 0)
            {
                sb.Append(v == Verdict.Approve
                    ? "정상 청원입니다. 올바른 처분입니다. ✔"
                    : "정상 청원을 기각했습니다. 더 신중히 살피세요.");
            }
            else
            {
                sb.Append("위반 발견:");
                foreach (var msg in violations)
                    sb.Append("\n· ").Append(msg);
                sb.Append(v == Verdict.Reject
                    ? "\n원칙대로 기각하셨습니다. ✔"
                    : "\n위반이 있으니 [기각]했어야 합니다.");
            }
            return sb.ToString();
        }

        // ── 보조 ──────────────────────────────────────────────────

        private void ReplacePhoto(string photoFile)
        {
            if (_photo != null)
            {
                _idPanel.Controls.Remove(_photo);
                _photo.Dispose();
            }
            _photo = new AssetPanel(photoFile ?? "photo_01.png", ImageLayout.Zoom)
            {
                Size = new Size(120, 150),
                Location = new Point(24, 56)
            };
            _idPanel.Controls.Add(_photo);
        }

        private void ShowStamp(Verdict v)
        {
            string file = v switch
            {
                Verdict.Approve => "stamp_approve.png",
                Verdict.Reject => "stamp_reject.png",
                _ => "stamp_refer.png"
            };

            _stampOverlay = new AssetPanel(file, ImageLayout.Zoom)
            {
                Size = new Size(190, 190),
                Location = new Point((_petitionPanel.Width - 190) / 2, 150),
                BackColor = Color.Transparent
            };
            _petitionPanel.Controls.Add(_stampOverlay);
            _stampOverlay.BringToFront();
        }

        private void SetVerdictButtonsEnabled(bool on)
        {
            _btnApprove.Enabled = on;
            _btnReject.Enabled = on;
            _btnRefer.Enabled = on;
        }

        private Label MakeField(Control parent, string label, int y)
        {
            var lbl = new Label
            {
                Font = UiTheme.Body(15f),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(420, 40),
                Location = new Point(34, y)
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private Label MakeCaption(string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Font = UiTheme.Body(12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(190, 190, 198),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(width, 26),
                Location = new Point(x, y)
            };
        }

        private Button MakeActionButton(string text, Color color, int x)
        {
            var btn = new Button
            {
                Text = text,
                Font = UiTheme.Body(16f, FontStyle.Bold),
                Size = new Size(150, 56),
                Location = new Point(x, 640),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }
    }
}
