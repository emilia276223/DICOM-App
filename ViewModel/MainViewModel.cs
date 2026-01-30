using CommunityToolkit.Mvvm.ComponentModel;
using DICOMApp.Behaviors;
using DICOMApp.Data;
using DICOMApp.Interfaces;
using DICOMApp.MLModel;
using DICOMApp.Repository;
using DICOMApp.Utils;
using DICOMApp.ViewModel.Lines;
using LiveChartsCore.Geo;
using System.IO;
using System.Threading.Tasks;


namespace DICOMApp.ViewModel
{

    /// <summary>
    /// ViewModel głównego okna
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        //-- View Modele  --//
        [ObservableProperty] private ChartViewModel _chartVM = new ChartViewModel(); // wykres
        [ObservableProperty] private ImageViewModel _imageVM = new ImageViewModel(); // obraz
        [ObservableProperty] private HeatmapControlViewModel _heatmapHandler = new HeatmapControlViewModel(); // heatmapy i kontrolki
        [ObservableProperty] private ImageSelectionViewModel _imageSelectionVM; // wybór obrazu
        [ObservableProperty] private MagnifiedImageViewModel _magnifiedImageVM = new MagnifiedImageViewModel(); // obraz powiększony

        /// <summary>
        /// Informacje o eksporcie i imporcie danych
        /// </summary>
        [ObservableProperty] private InformationPopupViewModel _dataExportPopup = new InformationPopupViewModel("Dane zostały wyeksportowane do pliku CSV"); // informacja o eksporcie danych
        [ObservableProperty] private InformationPopupViewModel _dataImportPopup = new InformationPopupViewModel("Obrazy zostały zaimportowane"); // informacja o imporcie plików

        // linie na obrazie
        [ObservableProperty] private ILineViewModel _lineVM = new LineWithCapsViewModel("red");
        [ObservableProperty] private ILineViewModel _lineVM2 = new LineWithCapsViewModel("blue");


        /// <summary>
        /// Obiekt umożliwiający wybór plików
        /// </summary>
        private IFilePickerService _filePickerService;


        /// <summary>
        /// Obiekt umożliwiający wybór zachowania na obrazie
        /// </summary>
        [ObservableProperty] private PointerModeBehavior _pointerMode;


        /// <summary>
        /// Obiekt umożliwiający komunikacje z baza danych
        /// </summary>
        private IDataHandler _dbHandler = new SQLDataBaseHandler();


        private IExportHandler _dataExportHandler;

        /// <summary>
        /// Obiekt modelu używanego do predykcji
        /// </summary>
        private IMLPlugin _MLPlugin = new ChangingMlModel();

        /// <summary>
        /// ID obrazu
        /// </summary>
        internal string _imageID;

        /// <summary>
        /// Metadane obrazu pobrane z pliku DICOM
        /// </summary>
        [ObservableProperty] private DICOMMetadata _dicomMetadata;

        [ObservableProperty] private LinesBehavior _linesB;

        /// <summary>
        /// Konstruktor obiektu MainViewModel
        /// </summary>
        /// <param name="filePickerService">Serwis umożliwiający wybór plików za pomocą okna eksploratora plików</param>
        public MainViewModel(IFilePickerService filePickerService)
        {
            // inicjalizacja listy pacjentów
            _imageSelectionVM = new ImageSelectionViewModel(
                (string a, string b, string c) => OnImageSelected(a, b, c)
            );
            // inicjalizacja obiektu pzetwarzającego odpowiednio zachowania myszki na obrazie
            _pointerMode = new PointerModeBehavior(MagnifiedImageVM, LineVM, LineVM2);

            // inicjalizacja obiektu pzetwarzającego odpowiednio zachowania myszki na powiększonym obrazie
            _linesB = new LinesBehavior(_lineVM, _lineVM2, MagnifiedImageVM);

            // zapisanie zainicjalizowanego serwisu plikow
            _filePickerService = filePickerService;

            // handler eksportu danych
            _dataExportHandler = new DataExportHandler(_dbHandler);

            // otwozenie pierwszego obrazu
            OpenImage("avares:Assets/images/000021.dcm");
        }

        /// <summary>
        /// Zamknięcie okna, w tym zamknięcie bazy danych
        /// </summary>
        public void Close()
        {
            _dbHandler.CloseConnection();
        }

        /// <summary>
        /// Metoda zapisująca zaznaczenia na obrazie
        /// </summary>
        public void SaveChosenPoints()
        {
            // pobranie punktów z obu linii na duzym obrazie - te zawsze są
            var points = LineVM.GetChosenPoints();
            points.AddRange(LineVM2.GetChosenPoints());

            // zapisanie zaznaczenia do bazy danych
            _dbHandler.SaveChosenPoints(points, 
                _imageID, 
                LineVM2.PhysicalLength,
                DicomMetadata);

            // update wykresu
            ChartVM.UpdateChart(_dbHandler.GetOpticNerveLenValuesForStudiesOfPatient(DicomMetadata.PatientID));
        }

        /// <summary>
        /// Metoda wyświetlająca wybrany obraz na podstawie podanych parametrów
        /// </summary>
        /// <param name="patientID">ID pacjenta</param>
        /// <param name="studyID">ID badania</param>
        /// <param name="fileName">Nazwa do pliku (sama nazwa, nie ścieżka)</param>
        public void OnImageSelected(string patientID, string studyID, string fileName)
        {
            string path = DICOMDirectoryHandler.GetFullImagePath(patientID, studyID, fileName);
            OpenImage(path);
        }

        /// <summary>
        /// Metoda umożliwiająca otworzenie nowego modelu ONNX
        /// i wykorzystywanie go od momentu jej wykonania do wszystkich predycji
        /// </summary>
        public async Task GetNewModel()
        {
            var filepath = await _filePickerService.PickONNXFileAsync();
            if (filepath == null)
                return;
            _MLPlugin.ReloadModel(filepath);
        }

        /// <summary>
        /// Metoda otwierająca katalog z plikami DICOM
        /// i ładująca je wszystkie do aplikacji
        /// </summary>
        /// <returns></returns>
        public async Task OpenDicomDirectory()
        {
            var files = await _filePickerService.PickDicomDirectory();
            foreach (var file in files) LoadAndRegisterImage(file);
            DataImportPopup.Show();
        }

        /// <summary>
        /// Metoda otwierająca nowy obraz wybrany w eksploratorze plików
        /// </summary>
        /// <returns></returns>
        public async Task OpenNewImage()
        {
            var file = await _filePickerService.PickImageFileAsync();
            
            // jezeli plik nie został wybrany, to nic nie robimy
            if (file == null)
                return;

            OpenImage(file);
        }

        /// <summary>
        /// Metoda eksportująca dane z bazy danych
        /// </summary>
        /// <returns></returns>
        public async Task ExportData()
        {
            _dataExportHandler.ExportData();
            DataExportPopup.Show();
        }

        /// <summary>
        /// Metoda otwierająca nowy obraz i zapisująca go do katalogu
        /// oraz jako PNG do katalogu eksportu
        /// </summary>
        /// <param name="file">Plik, z którego wczytujemy obraz</param>
        /// <returns>DICOMImageData reprezentujący wczytany obraz lub null</returns>
        private DICOMImageData? LoadAndRegisterImage(string file)
        {
            var dicomImageData = DICOMImageLoader.LoadAndRegisterDICOMImage(file);
            if (dicomImageData == null)
                return null;

            // dodanie obrazu do bazy danych
            _dbHandler.AddImageInfo(
                dicomImageData.Metadata, 
                dicomImageData.ImageID);

            // dodanie pacjenta do listy pacjentów
            ImageSelectionVM.AddPatient(
                dicomImageData.Metadata.PatientID);

            return dicomImageData;
        }


        /// <summary>
        /// Czy podpowiedzi AI są aktywne
        /// </summary>
        [ObservableProperty] private bool _isAIEnabled = false;


        private void SaveMagnifiedImageCenterPosition()
        {
            if (_imageID == null || MagnifiedImageVM.IsVisible == false) return;
            // jeśli był jakiś ustawiony obraz oraz powiększenie:
     
            var center = MagnifiedImageVM.GetCenterOfMagnifiedImage();
            _dbHandler.SaveMagnifiedImageCenterInfo(
                    _imageID,
                    center.Item1,
                    center.Item2);
        }

        /// <summary>
        /// Metoda otwierająca nowy obraz:
        ///     * wyświetlenie nowego obrazu
        ///     * update skali linii
        ///     * predykcja punktów przez model
        ///     * wydobycie heatmap z modelu
        ///     * update okna powększającego
        /// </summary>
        /// <param name="file">Ścieżka do pliku z obrazem DICOM do otwarcia</param>
        internal void OpenImage(string file)
        {
            // zapisujemy pozycje centrum wycinka powększającego
            SaveMagnifiedImageCenterPosition();

            var result = LoadAndRegisterImage(file);
            // załadowanie obrazu (jeśli się da)
            if (result == null)
            { 
                return; // jeśli nie udało się załadować nic więcej nie robimy
            }

            // update wykresu jeśli został zmieniony pacjent
            if(DicomMetadata != null &&  result.Metadata.PatientID != DicomMetadata.PatientID )
                ChartVM.UpdateChart(_dbHandler.GetOpticNerveLenValuesForStudiesOfPatient(result.Metadata.PatientID));

            UpdateLocalState(result);

            // update skali linii
            var scaler = DICOMScalerFactory.GetDICOMScaler(file);
            LinesB.UpdateScalerForOriginalLines(scaler, ImageVM._image_scale, ImageVM.ImageWidth, ImageVM.ImageHeight);

            // Update predykcji modelu
            UpdatePrediction(result.Image);

            // okno powiększające
            MagnifiedImageVM.UpdateImage(result.Image, ImageVM._image_scale, scaler);
            
        }

        private void UpdateLocalState(DICOMImageData dicomImageData)
        {
            _imageID = dicomImageData.ImageID;
            ImageVM.UpdateImage(dicomImageData.Image);
            DicomMetadata = dicomImageData.Metadata;
        }

        private void UpdatePrediction(ImageData image)
        {
            if (!IsAIEnabled)
            {
                var p = new Avalonia.Point(0, 0);
                LinesB.SetLinesToPrediction([p, p, p, p]);
                return;
            }

            // predykcja punktów
            var pred = _MLPlugin.GetPredictionFromImageData(image);
            var scaledPred = new Avalonia.Point[4];
            for (int i = 0; i < 4; i++) scaledPred[i] = (pred.Points[i] / ImageVM._image_scale);
            LinesB.SetLinesToPrediction(scaledPred);

            // heatmapy
            HeatmapHandler.UpdateHeatmaps(pred.Heatmaps);
        }
    }
}
