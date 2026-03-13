using System.Collections.Generic;
using EvidenceGuard.Core.Models;

namespace EvidenceGuard.Core.Interface
{
    /// <summary>
    /// 파일의 메타 정보 (타임스탬프)를 유지할 수 있는 서비스 인터페이스를 만들었습니다.
    /// 파일을 여는 경우에도 파일의 최근 사용일자 등의 메타 정보가 변경될 수 있기 때문에,
    /// 이 인터페이스의 메서드를 사용하여 원본 파일의 타임 스탬프를 분석용 복제 파일에 동일하게 적용해서
    /// 분석 과정 중에 생길 수 있는 메타데이터 변경을 방지하고자 했습니다.
    /// </summary>

    public interface IFileMetadataService
    {
        public void PreserveTimestamps(string sourcePath, string targetPath);
    }
}
