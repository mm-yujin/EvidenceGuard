using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EvidenceGuard.Converters
{

    /// <summary>
    /// Window Resource로 XAML에서 가져다 쓸 컨버터 클래스입니다.
    /// 이 프로젝트의 해시 동작 시에 해싱을 멀티로 수행하기 때문에,
    /// 그에 넘겨줄 파라미터 등이 멀티로 필요해 멀티 컨버트를 할 클래스를 만들었습니다.
    /// </summary>

    public class HashTypeToEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Length < 2 || value[0] == null || value[1] == null) return true;

            return !value[0].ToString().Equals(value[1].ToString());
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {

            return null;
        }
    }
}
