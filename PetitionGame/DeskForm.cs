using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PetitionGame
{
    /// <summary>
    /// 책상 화면 — 튜토리얼 코어.
    /// 세로형 청원서(petition_blank) 위에 신청 정보를 올리고, 우측에 신분증 카드를 둔다.
    /// [통과][기각][이첩] → Rulebook 판정 → 결과 → 다음. 4번째 청원에서 [이첩]·스탯 첫 등장.
    /// </summary>
    public class DeskForm : SceneForm
    {
        private readonly Stats _stats;
        private readonly Rulebook _rules = GameData.TodayRules();
        private readonly List<Petition> _petitions = GameData.TutorialPetitions();
        private int _index = -1;

        public event Action Finished;

        // 청원서 패널 위치/크기 (세로형 양식 비율 0.75)
        private const int PX = 110, PY = 96, PW = 384, PH = 512;

        // 상단
        private Label _counter;
        private Label _guide;

        // 청원서
        private AssetPanel _petitionPanel;
        private AssetPanel _petitionPhoto;
        private Label _pApplicant, _pRegion, _pRequest, _pStamp;
        private AssetPanel _stampOverlay;

        // 신분증 카드 (id_blank 양식)
        private AssetPanel _idCard;
        private AssetPanel _idPhoto;
        private Label _idName, _idRegion;

        // 신분증 사진칸 위치 (양식 기준)
        private static readonly Rectangle IdPhotoBox = new Rectangle(40, 46, 120, 157);

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
            BuildIdCard(bg);
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
                Size = new Size(250, 36),
                Location = new Point(24, 16)
            };
            host.Controls.Add(_counter);

            var bubble = new Panel
            {
                BackColor = Color.FromArgb(20, 20, 26),
                Size = new Size(800, 60),
                Location = new Point(300, 16)
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
            host.Controls.Add(MakeCaption("청원서", PX, PY - 28, PW));

            _petitionPanel = new AssetPanel("petition_blank.png")
            {
                Size = new Size(PW, PH),
                Location = new Point(PX, PY)
            };
            host.Controls.Add(_petitionPanel);

            // 양식 칸에 맞춘 신청 정보 (목업으로 정렬 확인한 좌표)
            _pApplicant = MakeField(_petitionPanel, 38, 69, 300);
            _pRegion = MakeField(_petitionPanel, 38, 100, 300);
            _pRequest = MakeField(_petitionPanel, 38, 205, 320);
            _pStamp = MakeField(_petitionPanel, 38, 404, 260);

            // 양식 우상단 사진칸
            // (실제 사진은 ReplacePhotos 에서 주입)
        }

        private void BuildIdCard(Control host)
        {
            host.Controls.Add(MakeCaption("신분증", 560, PY - 28, 380));

            _idCard = new AssetPanel("id_blank.png")
            {
                Size = new Size(380, 254),
                Location = new Point(560, PY)
            };
            host.Controls.Add(_idCard);

            // 사진칸 (양식 좌측) — 실제 사진은 ReplacePhotos 에서 주입
            _idName = new Label
            {
                Font = UiTheme.Body(13f, FontStyle.Bold),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(198, 28),
                Location = new Point(170, 50)
            };
            _idCard.Controls.Add(_idName);

            _idRegion = new Label
            {
                Font = UiTheme.Body(12f),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(198, 28),
                Location = new Point(170, 84)
            };
            _idCard.Controls.Add(_idRegion);
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
                Location = new Point(960, PY),
                Visible = false
            };
            host.Controls.Add(_statHeader);

            _barConscience = new StatBar("양심", "icon_conscience.png", UiTheme.Conscience)
            { Location = new Point(960, PY + 32), Visible = false };
            _barSin = new StatBar("죄악", "icon_sin.png", UiTheme.Sin)
            { Location = new Point(960, PY + 72), Visible = false };
            _barSuspicion = new StatBar("의심", "icon_suspicion.png", UiTheme.Suspicion)
            { Location = new Point(960, PY + 112), Visible = false };

            host.Controls.Add(_barConscience);
            host.Controls.Add(_barSin);
            host.Controls.Add(_barSuspicion);
        }

        private void BuildResultAndButtons(Control host)
        {
            var band = new Panel
            {
                BackColor = Color.FromArgb(16, 16, 20),
                Size = new Size(660, 130),
                Location = new Point(560, 380)
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

            _btnApprove = MakeActionButton("통 과", Color.FromArgb(56, 120, 72), 560);
            _btnReject = MakeActionButton("기 각", Color.FromArgb(150, 56, 56), 745);
            _btnRefer = MakeActionButton("이 첩", Color.FromArgb(120, 96, 40), 930);
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
                Size = new Size(540, 56),
                Location = new Point(560, 540),
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

            // 청원서 (양식 위 텍스트)
            _pApplicant.Text = $"신청인 :  {p.ApplicantName}";
            _pRegion.Text = $"거주구역 :  {p.Region}";
            _pRequest.Text = $"신청사항 :  {p.Request}";
            _pStamp.Text = p.HasValidStamp ? "황제 도장 :  ㊀ 유효" : "황제 도장 :  (누락)";
            _pStamp.ForeColor = p.HasValidStamp ? UiTheme.PaperInk : UiTheme.Accent;

            // 신분증
            _idName.Text = $"이름 :  {p.Id.Name}";
            _idRegion.Text = $"거주구역 :  {p.Id.Region}";
            ReplacePhotos(p.Id.PhotoPath);

            // 처분 도장 제거
            if (_stampOverlay != null)
            {
                _petitionPanel.Controls.Remove(_stampOverlay);
                _stampOverlay.Dispose();
                _stampOverlay = null;
            }

            _result.Text = "";
            SetVerdictButtonsEnabled(true);
            _btnNext.Visible = false;

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

        private void ReplacePhotos(string photoFile)
        {
            string file = photoFile ?? "photo_01.png";

            // 신분증 사진 (양식 사진칸)
            if (_idPhoto != null)
            {
                _idCard.Controls.Remove(_idPhoto);
                _idPhoto.Dispose();
            }
            _idPhoto = new AssetPanel(file, ImageLayout.Zoom)
            {
                Location = IdPhotoBox.Location,
                Size = IdPhotoBox.Size,
                BackColor = Color.Transparent
            };
            _idCard.Controls.Add(_idPhoto);
            _idPhoto.SendToBack(); // 이름/거주구역 라벨이 위에 오도록

            // 청원서 사진칸 (양식 우상단)
            if (_petitionPhoto != null)
            {
                _petitionPanel.Controls.Remove(_petitionPhoto);
                _petitionPhoto.Dispose();
            }
            _petitionPhoto = new AssetPanel(file, ImageLayout.Zoom)
            {
                Location = new Point(253, 59),
                Size = new Size(90, 105),
                BackColor = Color.Transparent
            };
            _petitionPanel.Controls.Add(_petitionPhoto);
        }

        private void ShowStamp(Verdict v)
        {
            string file = v switch
            {
                Verdict.Approve => "stamp_approve.png",
                Verdict.Reject => "stamp_reject.png",
                _ => "stamp_refer.png"
            };

            // 양식 우하단 도장칸 위치
            _stampOverlay = new AssetPanel(file, ImageLayout.Zoom)
            {
                Location = new Point(251, 389),
                Size = new Size(96, 72),
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

        private Label MakeField(Control parent, int x, int y, int w)
        {
            var lbl = new Label
            {
                Font = UiTheme.Body(13.5f, FontStyle.Bold),
                ForeColor = UiTheme.PaperInk,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(w, 26),
                Location = new Point(x, y)
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
                ForeColor = Color.FromArgb(200, 200, 208),
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
                Size = new Size(170, 56),
                Location = new Point(x, 540),
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
