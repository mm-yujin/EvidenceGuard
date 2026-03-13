using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGuard.Core.Interface
{

    /// <summary>
    /// 파일을 암호화 / 복호화하는 서비스 인터페이스입니다.
    /// 원본 또는 대상 파일의 path를 인자로 받아 동작하는 것을 기본 구조로 잡았습니다.
    /// </summary>

    public interface IFileCryptoService
    {
        Task EncryptFileAsync(string source, string dest);

        Task DecryptFileAsync(string source, string dest);
    }
}
