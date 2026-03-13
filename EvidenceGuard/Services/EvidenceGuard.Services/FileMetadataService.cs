using System.IO;
using EvidenceGuard.Core;
using EvidenceGuard.Core.Interface;

namespace EvidenceGuard.Services
{

    /// <summary>
    /// 원본 데이터의 무결성을 지키기 위해 타임스탬프 같은 메타데이터를 원래 것으로 복원하는 서비스 구현 클래스입니다.
    /// </summary>


    public class FileMetadataService : IFileMetadataService
    {
        public void PreserveTimestamps(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath) || !File.Exists(targetPath)) return;

            var sourceInfo = new FileInfo(sourcePath);

            File.SetCreationTime(targetPath, sourceInfo.CreationTime);
            File.SetLastWriteTime(targetPath, sourceInfo.LastWriteTime);
            File.SetLastAccessTime(targetPath, sourceInfo.LastAccessTime);
        }
    }
}