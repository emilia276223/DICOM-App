using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca widok eksportu danych
    /// </summary>
    public partial class InformationPopupViewModel : ViewModelBase
    {
        /// <summary>
        /// Czy popup jest widoczny
        /// </summary>
        [ObservableProperty] private bool _isVisible;

        /// <summary>
        /// Wiadomość, jaka ma być wyświetlana w popupie
        /// </summary>
        [ObservableProperty] private string _message;


        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="message">Wiadomość, jaka ma być wyświetlana</param>
        public InformationPopupViewModel(
            string message
            )
        {
            Message = message;
        }


        /// <summary>
        /// Ustawia eksport jako uruchomiony
        /// </summary>
        public void Show()
        {
            IsVisible = true;
        }

        /// <summary>
        /// Ustawia eksport jako zatrzymany / zakończony / nieuruchomiony
        /// </summary>
        public void Hide()
        {
            IsVisible = false; 
        }
    }
}
