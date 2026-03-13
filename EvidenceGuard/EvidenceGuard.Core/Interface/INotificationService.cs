

namespace EvidenceGuard.Core.Interface
{

    /// <summary>
    /// 알림 전용 서비스 인터페이스입니다.
    /// 비프음 (삑 하는 소리) 을 출력한다거나 윈도우 팝업 창을 띄운다거나 해서,
    /// 분량이 많은 포렌식 작업을 할 때에는 돌리고 다른 작업을 할 일이 많을 것이므로
    /// 작업이 끝났다는 알림을 받을 수 있도록 했습니다. (MD_NEXT 의 스펙을 참고했습니다.)
    /// </summary>


    public interface INotificationService
    {
        void PlayCompletionSound(); 
        void ShowAlert(string title, string message); 
    }
}