using System;
using System.Diagnostics;
using EvidenceGuard.Core.Interface;

namespace EvidenceGuard.Services
{

    /// <summary>
    /// 퍼포먼스를 체크하는 서비스 구현 클래스입니다.
    /// 체크 대상은 CPU의 사용량과 메모리의 여유 용량입니다.
    /// </summary>

    public class PerformanceService : IPerformanceService
    {
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _ramCounter;

        public PerformanceService()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            // 첫 번째 호출은 항상 0을 반환하므로 미리 한 번 호출해두어야 한다고 한다. 생성자에서 불러둔다.
            _cpuCounter.NextValue();
        }

        public double GetCpuUsage()
        {
            return Math.Round(_cpuCounter.NextValue(), 1);
        }

        public double GetAvailableRam()
        {
            return _ramCounter.NextValue();
        }

        public string GetMachineName()
        {
            return Environment.MachineName;
        }
    }
}