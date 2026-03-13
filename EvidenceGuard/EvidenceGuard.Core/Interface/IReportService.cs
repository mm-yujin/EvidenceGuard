using System.Collections.Generic;
using EvidenceGuard.Core.Models;

namespace EvidenceGuard.Core.Interface
{

    /// <summary>
    /// 분석(탐지) 결과를 출력하기 위한 서비스 인터페이스입니다.
    /// 저장된 결과(ExtractionResult)를 선택한 형태의 파일 (CSV 또는 PDF) 으로 저장할 수 있도록 했습니다.
    /// 저장할 파일 경로와 선택적 획득 (탐지) 결과, 선택한 해시 알고리즘 두 종류의 결과값과 작업중의 평균 퍼포먼스를 인자로 받아 처리합니다.
    /// </summary>

    public interface IReportService
    {
        void GenerateCsvReport(string savePath, List<ExtractionResult> results, string hash1, string hash2, double avgCpu, double avgRam);
        void GeneratePdfReport(string savePath, List<ExtractionResult> results, string hash1, string hash2, double avgCpu, double avgRam);
    }
}