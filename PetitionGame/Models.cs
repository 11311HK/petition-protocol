using System;
using System.Collections.Generic;
using System.Linq;

namespace PetitionGame
{
    // ─────────────────────────────────────────────────────────────
    // §0 재사용 모델 — 기존 Program.cs 의 IdCard / Petition / Rulebook / Stats
    //    (저장소에 Program.cs 가 없어 플랜 §4 명세대로 동일 형태로 재구성)
    //    UI 코드와 분리하기 위해 이 파일에는 순수 도메인 로직만 둔다.
    // ─────────────────────────────────────────────────────────────

    /// <summary>신분증</summary>
    public class IdCard
    {
        public string Name { get; set; }       // 이름
        public string Region { get; set; }      // 거주구역
        public string PhotoPath { get; set; }   // 증명사진 에셋 파일명

        public IdCard(string name, string region, string photoPath = null)
        {
            Name = name;
            Region = region;
            PhotoPath = photoPath;
        }
    }

    /// <summary>청원서</summary>
    public class Petition
    {
        public string ApplicantName { get; set; }   // 신청인
        public string Region { get; set; }          // 청원서에 적힌 거주구역
        public string Request { get; set; }          // 신청사항
        public bool HasValidStamp { get; set; }      // 황제 도장 유효 여부 (O/X)
        public IdCard Id { get; set; }               // 첨부된 신분증
        public string Guide { get; set; }            // 튜토리얼 가이드 문구

        public Petition(string applicantName, string region, string request,
                        bool hasValidStamp, IdCard id, string guide = null)
        {
            ApplicantName = applicantName;
            Region = region;
            Request = request;
            HasValidStamp = hasValidStamp;
            Id = id;
            Guide = guide;
        }
    }

    /// <summary>규정집 — 오늘의 규정에 따라 위반 사항을 판정한다.</summary>
    public class Rulebook
    {
        /// <summary>오늘의 금지 신청사항 (예: 집회 허가, 금서 반환)</summary>
        public HashSet<string> ForbiddenRequests { get; }

        public Rulebook(IEnumerable<string> forbiddenRequests = null)
        {
            ForbiddenRequests = new HashSet<string>(
                forbiddenRequests ?? new[] { "집회 허가", "금서 반환" });
        }

        /// <summary>청원서를 검사해 위반 항목 목록을 돌려준다. 비어 있으면 정상 청원.</summary>
        public List<string> GetViolations(Petition p)
        {
            var violations = new List<string>();

            if (p == null) return violations;

            if (p.Id == null)
            {
                violations.Add("신분증 누락");
                return violations;
            }

            // 신청인 = 신분증 이름 일치
            if (!Same(p.ApplicantName, p.Id.Name))
                violations.Add($"신청인 불일치 (청원서 '{p.ApplicantName}' ≠ 신분증 '{p.Id.Name}')");

            // 거주구역 = 신분증 거주구역 일치
            if (!Same(p.Region, p.Id.Region))
                violations.Add($"거주구역 불일치 (청원서 '{p.Region}' ≠ 신분증 '{p.Id.Region}')");

            // 황제 도장 유효
            if (!p.HasValidStamp)
                violations.Add("황제 도장 누락/무효");

            // 금지 신청사항
            if (p.Request != null && ForbiddenRequests.Contains(p.Request.Trim()))
                violations.Add($"금지된 신청사항: {p.Request}");

            return violations;
        }

        private static bool Same(string a, string b)
            => string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.Ordinal);
    }

    /// <summary>심사관 스탯</summary>
    public class Stats
    {
        public int Conscience { get; set; }   // 양심
        public int Sin { get; set; }          // 죄악
        public int Suspicion { get; set; }    // 의심

        public override string ToString()
            => $"양심 {Conscience} · 죄악 {Sin} · 의심 {Suspicion}";
    }

    /// <summary>심사관의 처분</summary>
    public enum Verdict
    {
        Approve,  // 통과
        Reject,   // 기각
        Refer     // 이첩 (정서감찰부 송치)
    }
}
