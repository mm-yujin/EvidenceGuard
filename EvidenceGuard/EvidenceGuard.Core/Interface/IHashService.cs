using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGuard.Core.Interface
{
    public enum HashType
    {
        MD5,
        SHA1,
        SHA256,
        SHA512
    }

    /// <summary>
    /// 디지털 증거의 무결성을 검증하는 것을 주 목적으로 멀티 해싱을 수행하는 서비스 인터페이스입니다.
    /// 하나의 해시 알고리즘만을 사용하는 경우에 비해 해시 충돌이나 해시 알고리즘 취약점에 대비할 수 있도록 두 종류를 함께 쓰게 했습니다.
    /// 해시 알고리즘 종류는 위의 HashType 열거체 값 중 하나를 고르도록 했습니다.
    /// </summary>
    public interface IHashService
    {
        Task<Dictionary<string, string>> MultiCalculateHashAsync(string filePath, HashType hashType1, HashType hashType2);

        Task<bool> VerifyIntegrityAsync(string filePath, HashType type1, string targetHash1, HashType type2, string targetHash2);
    }
}
