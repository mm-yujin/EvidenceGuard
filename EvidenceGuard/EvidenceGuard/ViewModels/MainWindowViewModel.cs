using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Microsoft.Win32;
using EvidenceGuard.Core.Interface;
using EvidenceGuard.Core.Models;
using EvidenceGuard.Services;
using System.Linq;
using EvidenceGuard.Services.Interfaces;

namespace EvidenceGuard.ViewModels
{
    /// <summary>
    /// 메인 뷰 모델 클래스입니다.
    /// 동작하는 서비스들을 화면(뷰) 와 엮어주는 연결 통로 역할이자 이벤트에 따른 동작들을 도맡아 하는 클래스입니다.
    /// </summary>


    public class MainWindowViewModel : BindableBase
    {
        private readonly IHashService _hashService;
        private readonly IFileCryptoService _cryptoService;
        private readonly IFileSearchService _searchService;
        private readonly IFileMetadataService _metadataService;
        private readonly IReportService _reportService;
        private readonly INotificationService _notificationService;
        private readonly IPerformanceService _performanceService;

        private string _filePath;
        private string _status;
        private string _hash1Result;
        private string _hash2Result;

        public string FilePath { get => _filePath; set => SetProperty(ref _filePath, value); }
        public string Status { get => _status; set => SetProperty(ref _status, value); }
        public string Hash1Result { get => _hash1Result; set => SetProperty(ref _hash1Result, value); }
        public string Hash2Result { get => _hash2Result; set => SetProperty(ref _hash2Result, value); }


        private HashType _selectedHash1 = HashType.MD5;
        private HashType _selectedHash2 = HashType.SHA256;

        public ObservableCollection<HashType> HashAlgorithms { get; } = new(Enum.GetValues<HashType>());

        public HashType SelectedHash1 { get => _selectedHash1; set => SetProperty(ref _selectedHash1, value); }
        public HashType SelectedHash2 { get => _selectedHash2; set => SetProperty(ref _selectedHash2, value); }


        // XAML이랑 연결할 딜리게이트커맨드
        public DelegateCommand SelectFileCommand { get; }
        public DelegateCommand RunAcquisitionCommand { get; } //암호화한거 획득
        public DelegateCommand RunRestitutionCommand { get; } //암호화문을 복호화하면서 분해+메타데이터맞추기+무결성검증+선별적획득
        public DelegateCommand SaveReportCommand { get; } //리포트 저장


        //결과물
        private ObservableCollection<ExtractionResult> _extractionResults = new();
        public ObservableCollection<ExtractionResult> ExtractionResults
        {
            get => _extractionResults;
            set => SetProperty(ref _extractionResults, value);
        }


        //퍼포먼스
        private double _cpuUsage;
        public double CpuUsage
        {
            get => _cpuUsage;
            set => SetProperty(ref _cpuUsage, value);
        }

        private double _ramUsage;
        public double RamUsage
        {
            get => _ramUsage;
            set => SetProperty(ref _ramUsage, value);
        }

        private List<double> _cpuHistory = new();
        private List<double> _ramHistory = new();


        public MainWindowViewModel(IHashService hashService, IFileCryptoService cryptoService, IFileSearchService searchService, 
            IFileMetadataService metadataService, IReportService reportService, INotificationService notificationService, IPerformanceService performanceService)
        {
            _hashService = hashService;
            _cryptoService = cryptoService;
            _searchService = searchService;
            _metadataService = metadataService;
            _reportService = reportService;
            _notificationService = notificationService;
            _performanceService = performanceService;


            SelectFileCommand = new DelegateCommand(ExecuteSelectFile);
            RunAcquisitionCommand = new DelegateCommand(async () => await ExecuteAcquisition(), CanExecuteAcquisition)
                                    .ObservesProperty(() => FilePath);
            RunRestitutionCommand = new DelegateCommand(async () => await ExecuteRestitution(), CanExecuteRestitution)
                                    .ObservesProperty(() => Hash1Result)
                                    .ObservesProperty(() => Hash2Result);
            SaveReportCommand = new DelegateCommand(ExecuteSaveReport);


            //퍼포먼스 체크 시작
            _performanceService = performanceService;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                CpuUsage = _performanceService.GetCpuUsage();
                RamUsage = _performanceService.GetAvailableRam();

                _cpuHistory.Add(CpuUsage);
                _ramHistory.Add(RamUsage);
            };
            timer.Start();
        }


        private void ExecuteSelectFile()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true) FilePath = dialog.FileName;
        }

        private bool CanExecuteAcquisition() => !string.IsNullOrEmpty(FilePath);
        private bool CanExecuteRestitution() => !string.IsNullOrEmpty(Hash1Result) && !string.IsNullOrEmpty(Hash2Result);

        private async Task ExecuteAcquisition()
        {
            try
            {
                _cpuHistory.Clear();
                _ramHistory.Clear();

                Status = "분석 및 보안 획득 진행 중...";

                var hashes = await _hashService.MultiCalculateHashAsync(FilePath, SelectedHash1, SelectedHash2);
                Hash1Result = hashes[SelectedHash1.ToString()];
                Hash2Result = hashes[SelectedHash2.ToString()];

                string encryptedPath = FilePath + ".evd";
                await _cryptoService.EncryptFileAsync(FilePath, encryptedPath);

                Status = $"완료! 암호화 파일 생성됨: {encryptedPath}";
            }
            catch (Exception ex)
            {
                Status = $"암호화 중 오류 발생: {ex.Message}";
            }
        }

        private async Task ExecuteRestitution()
        {
            if (Hash1Result == null || Hash2Result == null)
            {
                Status = $"아직 암호화 처리를 수행하지 않았음";
                return;
            }

            try
            {
                ExtractionResults.Clear();

                string encryptedPath = FilePath + ".evd";
                string decryptedPath = FilePath + "_decrypted.txt";

                if (!System.IO.File.Exists(encryptedPath))
                {
                    Status = "암호화된 증거 파일(.evd)을 찾을 수 없습니다.";
                    return;
                }

                Status = "복호화 진행 중...";
                await _cryptoService.DecryptFileAsync(encryptedPath, decryptedPath);


                Status = "무결성 교차 검증 중...";
                var hashVerify = await _hashService.VerifyIntegrityAsync(decryptedPath, SelectedHash1, Hash1Result, SelectedHash2, Hash2Result);
                if (!hashVerify)
                {
                    Status = "경고: 무결성 검증 실패! 증거가 변조되었을 가능성이 있습니다.";
                    return;
                }

                _metadataService.PreserveTimestamps(FilePath, decryptedPath);

                Status = "키워드 및 개인정보 선별 추출하는 중....";
                string searchPattern = $"{ForensicPatterns.Email}|{ForensicPatterns.PhoneNumber}|{ForensicPatterns.ResidentNumber}";

                var searchResults = await _searchService.SearchPatternsAsync(decryptedPath, searchPattern);

                foreach (var result in searchResults)
                {
                    result.IsIntegrityVerified = hashVerify; //무결성 확인 여부도 
                    ExtractionResults.Add(result);
                }

                Status = $"분석 완료! {searchResults.Count}건의 증거 발견됨";
            }
            catch (Exception ex)
            {
                Status = $"복호화 중 오류 발생(복원 실패): {ex.Message}";
            }
        }

        private void ExecuteSaveReport()
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV 리포트 (*.csv)|*.csv|PDF 리포트 (*.pdf)|*.pdf",
                FileName = $"Forensic_Report_{DateTime.Now:yyyyMMdd}",
                Title = "분석 결과 보고서 저장"
            };

            if (saveDialog.ShowDialog() == true)
            {
                string extension = System.IO.Path.GetExtension(saveDialog.FileName).ToLower();

                try
                {
                    double avgCpu = _cpuHistory.Count > 0 ? Math.Round(_cpuHistory.Average(), 1) : 0;
                    double avgRam = _ramHistory.Count > 0 ? Math.Round(_ramHistory.Average(), 1) : 0;

                    if (extension == ".csv")
                    {
                        _reportService.GenerateCsvReport(saveDialog.FileName, ExtractionResults.ToList(), Hash1Result, Hash2Result, avgCpu, avgRam);
                    }
                    else if (extension == ".pdf")
                    {
                        _reportService.GeneratePdfReport(saveDialog.FileName, ExtractionResults.ToList(), Hash1Result, Hash2Result, avgCpu, avgRam);
                    }

                    //완료 알림
                    _notificationService.ShowAlert("보고서 생성 완료", $"경로: {saveDialog.FileName}\n성공적으로 저장되었습니다.");
                    _notificationService.PlayCompletionSound(); //삑 소리
                }
                catch (Exception ex)
                {
                    string errorMsg = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                    Status = $"보고서 저장 중 오류 발생: {errorMsg}";
                }
            }
        }
    }
}