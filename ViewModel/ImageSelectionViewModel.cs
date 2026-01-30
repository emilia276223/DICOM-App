using CommunityToolkit.Mvvm.ComponentModel;
using DICOMApp.Utils;
using System;
using System.Collections.Generic;

namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca widok wyboru obrazów
    /// </summary>
    public partial class ImageSelectionViewModel : ViewModelBase
    {
        /// <summary>
        /// Lista pacjentów
        /// </summary>
        [ObservableProperty] private ElementListViewModel _patients;
        
        /// <summary>
        /// Lista badań
        /// </summary>
        [ObservableProperty] private ElementListViewModel _studies;

        /// <summary>
        /// Lista obrazów
        /// </summary>
        [ObservableProperty] private ElementListViewModel _images;

        /// <summary>
        /// Czy został wybrany obraz
        /// </summary>
        [ObservableProperty] private bool _isImageSelected;

        /// <summary>
        /// Akcja uruchamiana w momencie wyboru obrazu
        /// </summary>
        private readonly Action<string, string, string> _onImageSelectionAction;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="onImageSelectionAction">Akcja uruchamiana w momencie wyboru obrazu</param>
        public ImageSelectionViewModel(Action<string, string, string> onImageSelectionAction)
        {
            _patients = new ElementListViewModel(() => {OnPatientSelection();} , "Patient");
            _studies = new ElementListViewModel(() => {OnStudySelection();}, "Study");
            _images = new ElementListViewModel(() => {OnImageSelection();}, "Image");
            _isImageSelected = false;
            _onImageSelectionAction = onImageSelectionAction;

            AddPatientsList(DICOMDirectoryHandler.GetPatients());
        }

        /// <summary>
        /// Metoda dodająca pacjenta do listy pacjentów
        /// </summary>
        /// <param name="patientName">Nazwisko pakcjenta do dodania do listy</param>
        public void AddPatient(string patientName)
        {
            Patients.AddElement(patientName);
        }

        /// <summary>
        /// Metoda usuwająca wszystkich pacjentów z listy
        /// </summary>
        public void ClearPatients()
        {
            Patients.Clear();
        }

        /// <summary>
        /// Metoda dodająca listę pacjentów do listy
        /// </summary>
        /// <param name="patientNames">Lista pacjentów</param>
        public void AddPatientsList(IEnumerable<string> patientNames)
        {
            foreach (var name in patientNames)
            {
                AddPatient(name);
            }
        }

        /// <summary>
        /// Metoda wywoływana po zmianie wybranej wartosci w liscie pacjentów
        /// Ustawia listę badań pacjenta do wyboru
        /// </summary>
        private void OnPatientSelection()
        {
            if(Patients.SelectedElement == null) return;

            Patients.Hide();
            Studies.Show();
            Images.Hide();

            var newStudies = DICOMDirectoryHandler.GetStudiesForPatient(Patients.SelectedElement);
            Studies.UpdateElements(newStudies);
            Images.Clear();
            IsImageSelected = false;
        }

        /// <summary>
        /// Metoda wywoływana po zmianie wybranej wartosci w liscie obrazów
        /// Wykonuje akcje uruchamiana w momencie wyboru obrazu
        /// </summary>
        private void OnImageSelection()
        {
            if(Images.SelectedElement == null) return;

            Images.Hide();
            Patients.Hide();
            Studies.Hide();

            if (Patients.SelectedElement != null && Studies.SelectedElement != null)
            {
                IsImageSelected = true;
                _onImageSelectionAction(Patients.SelectedElement, Studies.SelectedElement, Images.SelectedElement);
            }
        }

        /// <summary>
        /// Metoda wywoływana po zmianie wybranej wartosci w liscie badań
        /// Ustawia listę obrazów do wyboru
        /// </summary>
        private void OnStudySelection()
        {
            if (Studies.SelectedElement == null) return;

            if (Patients.SelectedElement != null)
            {
                Images.UpdateElements(DICOMDirectoryHandler.GetImagesForStudy(Patients.SelectedElement, Studies.SelectedElement));

                Studies.Hide();
                Images.Show();
                Patients.Hide();
                IsImageSelected = false;
            }
        }

        /// <summary>
        /// Metoda zmieniająca wybrany obraz na następny na liście
        /// </summary>
        public void GetNextImage()
        {
            Images.SetToNextElement();
        }

        /// <summary>
        /// Metoda zmieniająca wybrany obraz na poprzedni na liście
        /// </summary>
        public void GetPreviousImage()
        {
            Images.SetToPreviousElement();
        }
    }
}
