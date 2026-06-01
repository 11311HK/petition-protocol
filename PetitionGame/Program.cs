using System;
using System.Windows.Forms;

namespace PetitionGame
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameContext());
        }
    }

    /// <summary>
    /// §2 씬 흐름 제어:
    /// Title → Prologue → Desk(튜토리얼) → DayEnd → 종료.
    /// 스탯(Stats)을 전 씬에 걸쳐 공유한다.
    /// </summary>
    internal sealed class GameContext : ApplicationContext
    {
        private readonly Stats _stats = new Stats();

        public GameContext()
        {
            ShowTitle();
        }

        private void ShowTitle()
        {
            var f = new TitleForm();
            f.StartRequested += () => SwitchTo(BuildPrologue());
            SwitchTo(f);
        }

        private PrologueForm BuildPrologue()
        {
            var f = new PrologueForm();
            f.Finished += () => SwitchTo(BuildDesk());
            return f;
        }

        private DeskForm BuildDesk()
        {
            var f = new DeskForm(_stats);
            f.Finished += () => SwitchTo(BuildDayEnd());
            return f;
        }

        private DayEndForm BuildDayEnd()
        {
            var f = new DayEndForm(_stats);
            f.ExitRequested += ExitThread;
            return f;
        }

        /// <summary>이전 폼을 닫고 다음 폼을 메인으로 전환. (앱은 종료되지 않음)</summary>
        private void SwitchTo(Form next)
        {
            var prev = MainForm;
            MainForm = next;          // MainForm 재지정 → prev 종료 시 앱이 끝나지 않음
            next.Show();
            prev?.Close();
        }
    }
}
