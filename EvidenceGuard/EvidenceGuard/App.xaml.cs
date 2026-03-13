using System.Windows;
using EvidenceGuard.Core.Interface;

//using EvidenceGuard.Modules.ModuleName;
using EvidenceGuard.Services;
using EvidenceGuard.Services.Interfaces;
using EvidenceGuard.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace EvidenceGuard
{
    /// <summary>
    /// 애플리케이션의 진입점 클래스입니다.
    /// Prism Library를 사용하여 MVVM 패턴의 초기화, 의존성 주입(DI), 모듈 구성을 수행합니다.
    /// </summary>
    
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// 서비스 및 인터페이스를 IoC 컨테이너에 등록합니다.
        /// 싱글톤 여부에 따라 프로그램 전체에서 상태를 공유할지, 매번 새 인스턴스를 생성할지 결정합니다.
        /// </summary>

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();

            containerRegistry.RegisterSingleton<IFileCryptoService, FileCryptoService>();
            containerRegistry.RegisterSingleton<IHashService, HashService>();
            containerRegistry.RegisterSingleton<IFileSearchService, FileSearchService>();
            containerRegistry.RegisterSingleton<IFileMetadataService, FileMetadataService>();

            containerRegistry.Register<IPerformanceService, PerformanceService>();
            containerRegistry.Register<IReportService, ReportService>();
            containerRegistry.Register<INotificationService, NotificationService>();
        }


        /// <summary>
        /// 외부 모듈(Module)을 카탈로그에 추가하여 기능을 확장할 때 사용합니다.
        /// </summary>

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //moduleCatalog.AddModule<ModuleNameModule>();
        }
    }
}
