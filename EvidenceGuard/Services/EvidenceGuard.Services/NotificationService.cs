using System.Windows;
using System.Media; // 비프음을 위해 필요
using EvidenceGuard.Core.Interface;

namespace EvidenceGuard.Services
{
    /// <summary>
    /// 작업이 끝났을 때 등 여러 상황에서 활용할 수 있는 '알림' 서비스 구현 클래스입니다.
    /// 여기에서는 소리와 알림창 (청각/시각) 방식을 구현해두었습니다.
    /// 알림창의 경우에는 현재 켜져 있는 메인 윈도우를 찾아서 부모로 설정하게 했습니다.
    /// </summary>

    public class NotificationService : INotificationService
    {
        public void PlayCompletionSound()
        {
            // 기본 별표(Asterisk) 소리 재생
            //SystemSounds.Asterisk.Play();
            System.Console.Beep(500, 300); // 좀더 명확하게 변경- 500Hz로 0.3초간 재생
        }

        public void ShowAlert(string title, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(Application.Current.MainWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
    }
}