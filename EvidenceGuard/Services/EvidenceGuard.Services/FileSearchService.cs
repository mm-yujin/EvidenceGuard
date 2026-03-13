using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EvidenceGuard.Core.Interface;
using EvidenceGuard.Core.Models;

namespace EvidenceGuard.Services
{

    /// <summary>
    /// 입력받은 탐색 패턴을 바탕으로 전체 데이터 가운데에서
    /// 증거가 될 수 있을 만한 유형의 데이터를 찾아내는 서비스 구현 클래스입니다.
    /// Regex 옵션에 Compiled를 주어서, 패턴 정규식을 미리 기계어로 번역해두어 반복 검색 성능을 최적화하도록 했습니다.
    /// </summary>



    public class FileSearchService : IFileSearchService
    {
        public async Task<List<ExtractionResult>> SearchPatternsAsync(string filePath, string pattern, IProgress<double> progress = null)
        {
            return await Task.Run(async () =>
            {
                var results = new List<ExtractionResult>();
                if (!File.Exists(filePath)) return results;

                var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var fileInfo = new FileInfo(filePath);
                long totalBytes = fileInfo.Length;
                long processedBytes = 0;

                using var reader = new StreamReader(filePath);
                string line;
                int lineNumber = 0;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    processedBytes += System.Text.Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length; //진행 바이트 체크(진행률 체크용 - 줄바꿈문자도 포함)

                    var matches = regex.Matches(line);

                    foreach (Match match in matches)
                    {
                        string detectedType = "Unknown";

                        if (match.Groups["Email"].Success) detectedType = "이메일";
                        else if (match.Groups["Phone"].Success) detectedType = "전화번호";
                        else if (match.Groups["Resident"].Success) detectedType = "주민번호";

                        results.Add(new ExtractionResult
                        {
                            FileName = fileInfo.Name,
                            FullDestinationPath = filePath,
                            LineNumber = lineNumber,
                            Keyword = match.Value,
                            TypeName = detectedType,
                            Offset = match.Index, 
                            FoundTime = DateTime.Now
                        });
                    }

                    // 주기적으로 진행률 보고 (100줄마다 - 너무 자주 보내진 않게)
                    if (lineNumber % 100 == 0 && totalBytes > 0)
                    {
                        progress?.Report(Math.Min(100, (double)processedBytes / totalBytes * 100));
                    }
                }

                progress?.Report(100); // 완료 보고
                return results;
            });
        }
    }
}