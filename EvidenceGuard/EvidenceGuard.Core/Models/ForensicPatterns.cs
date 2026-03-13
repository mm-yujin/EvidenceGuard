using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGuard.Core.Models
{
 
    /// <summary>
    /// 데이터 탐색 시 활용할 포렌식 패턴을 별도로 저장해두었습니다. 추후 다른 곳에서도 활용할 수 있도록
    /// 패턴 종류들을 모아놓기 위한 별도 static 클래스입니다.
    /// 패턴은 정규식(Regex)을 활용한 string 형태이고, 유형별로 "(?<그룹이름> ... )" 형태로 해서 추후 그룹 구분을 할 수 있도록 했습니다.
    /// </summary>

    public static class ForensicPatterns
    {
        public const string Email = @"(?<Email>[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})";
        public const string PhoneNumber = @"(?<Phone>\d{2,3}-\d{3,4}-\d{4})";
        public const string ResidentNumber = @"(?<Resident>\d{6}-[1-4]\d{6})";
    }
}
