using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvidenceGuard.Core.Models;

namespace EvidenceGuard.Core.Interface
{
    /// <summary>
    /// 파일에서 선택적인 정보를 찾아내기 위한 서비스 인터페이스입니다.
    /// 파일의 경로와 정규식(Regex) 패턴을 파라미터로 받아 진행하는 것을 원칙으로 했습니다.    /// 
    /// </summary>

    public interface IFileSearchService
    {
        Task<List<ExtractionResult>> SearchPatternsAsync(string filePath, string pattern, IProgress<double> progress = null);
    }
}
