

namespace EvidenceGuard.Core.Interface
{

    /// <summary>
    /// 퍼포먼스를 체크하는 서비스 인터페이스입니다.
    /// 현재 CPU의 사용률과 사용 가능한 메모리 용량을 메인으로 체크하고,
    /// 이 작업이 수행되는 환경이 안정적인 환경이라는 것을 보여주기 위한 지표로서 체크합니다.
    /// 체크된 정보는 기기 이름 등과 같이 보고서에 기재합니다.
    /// </summary>

    public interface IPerformanceService
    {
        double GetCpuUsage();      // 현재 CPU 사용률 (%)
        double GetAvailableRam();  // 가용 메모리 (MB)
        string GetMachineName();   // 분석 기기 이름 (보고서 식별용)
    }
}