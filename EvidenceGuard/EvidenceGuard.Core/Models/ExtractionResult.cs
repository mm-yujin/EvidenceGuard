using System;

namespace EvidenceGuard.Core.Models
{
    /// <summary>
    /// 분석(탐지) 결과를 저장하는 클래스입니다.
    /// 분석 대상 파일경로 등의 데이터를 구분해 저장해두었다가 출력할 때 쓰기 위함입니다.
    /// </summary>


    public class ExtractionResult
    {
        // 파일 정보
        public string FileName { get; set; }
        public string FullDestinationPath { get; set; }

        // 탐지 정보
        public long LineNumber { get; set; } // 몇 번째 줄
        public int Offset { get; set; }      // 해당 줄에서 몇 번째 글자
        public string Keyword { get; set; }  // 발견된 키워드 (내용)
        public string TypeName { get; set; } //유형

        // 메타데이터
        public DateTime FoundTime { get; set; } //시간
        public bool IsIntegrityVerified { get; set; } //무결성검증 결과
    }
}