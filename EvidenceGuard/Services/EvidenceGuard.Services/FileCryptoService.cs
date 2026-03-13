using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EvidenceGuard.Core.Interface;

namespace EvidenceGuard.Services
{
    /// <summary>
    /// 데이터의 암호화 - 복호화를 수행하는 서비스 구현 클래스입니다.
    /// 암호화는 .NET 표준 Cryptography 라이브러리를 활용해 AES 형식으로 수행했습니다.
    /// 실무에서는 고정된 키보다는 사용자 패스워드 기반으로 더 복잡성을 크게 증가시킨
    /// PBKDF2라던가, 최근에는 Argon2 같은 알고리즘을 사용하는 추세이지만
    /// 현 툴에서는 동작 구조를 우선해서 잡기 위해 키와 IV를 임의 지정해 사용했습니다.
    /// </summary>

    public class FileCryptoService : IFileCryptoService
    {        
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("EvidenceGuard_Test_Key_ICanDoit!");
        private static readonly byte[] Iv = Encoding.UTF8.GetBytes("1234567890123456");

        public async Task EncryptFileAsync(string sourcePath, string destPath)
        {
            await Task.Run(() =>
            {
                using Aes aes = Aes.Create();
                aes.Key = Key;
                aes.IV = Iv;

                using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using FileStream fsIn = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
                using FileStream fsOut = new FileStream(destPath, FileMode.Create, FileAccess.Write);
                using CryptoStream cs = new CryptoStream(fsOut, encryptor, CryptoStreamMode.Write);

                byte[] buffer = new byte[65536];
                int bytesRead;
                while ((bytesRead = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cs.Write(buffer, 0, bytesRead);
                }
            });
        }

        public async Task DecryptFileAsync(string sourcePath, string destPath)
        {
            await Task.Run(async () =>
            {
                using Aes aes = Aes.Create();
                aes.Key = Key;
                aes.IV = Iv;

                using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using FileStream fsIn = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
                using FileStream fsOut = new FileStream(destPath, FileMode.Create, FileAccess.Write);
                using CryptoStream cs = new CryptoStream(fsOut, decryptor, CryptoStreamMode.Write);

                await fsIn.CopyToAsync(cs);
            });
        }
    }
}