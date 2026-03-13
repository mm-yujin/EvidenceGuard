using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EvidenceGuard.Core.Interface;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace EvidenceGuard.Services
{

    /// <summary>
    /// 두 가지 종류의 해시 알고리즘을 선택 받아서, 해시값으로 변환 (해싱) 하거나 
    /// 그 변환된 값이 유효한지 판단하는 메서드를 구현한 클래스입니다.
    /// </summary>

    public class HashService : IHashService
    {
        public async Task<Dictionary<string, string>> MultiCalculateHashAsync(string filePath, HashType hashType1, HashType hashType2)
        {
            return await Task.Run(() =>
            {
                var results = new Dictionary<string, string>();
                if (!File.Exists(filePath)) return results;

                using var stream = File.OpenRead(filePath);
                byte[] buffer = new byte[65536]; // 65536 = 64KB
                int bytesRead;

                IDigest digest1 = CreateDigest(hashType1);
                IDigest digest2 = CreateDigest(hashType2);

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    digest1.BlockUpdate(buffer, 0, bytesRead);
                    digest2.BlockUpdate(buffer, 0, bytesRead);
                }
                
                results.Add(hashType1.ToString(), FinalizeHash(digest1));
                results.Add(hashType2.ToString(), FinalizeHash(digest2));

                return results;
            });
        }

        private IDigest CreateDigest(HashType type) 
        {
            return type switch
            {
                HashType.MD5 => new MD5Digest(),
                HashType.SHA1 => new Sha1Digest(),
                HashType.SHA256 => new Sha256Digest(),
                HashType.SHA512 => new Sha512Digest(),
                _ => throw new NotImplementedException($"{type}은 지원하지 않는 알고리즘입니다.")
            };
        }

        private string FinalizeHash(IDigest digest)
        {
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
        }

        public async Task<bool> VerifyIntegrityAsync(string filePath, HashType type1, string targetHash1, HashType type2, string targetHash2)
        {
            var currentHash = await MultiCalculateHashAsync(filePath, type1, type2);

            if (currentHash == null || currentHash.Count < 2) return false;

            bool isFirstMatch = currentHash[type1.ToString()].Equals(targetHash1, StringComparison.OrdinalIgnoreCase);
            bool isSecondMatch = currentHash[type2.ToString()].Equals(targetHash2, StringComparison.OrdinalIgnoreCase);

            return isFirstMatch && isSecondMatch;
        }
    }
}