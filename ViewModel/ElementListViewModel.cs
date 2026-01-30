using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca widok listy elementów
    /// </summary>
    public partial class ElementListViewModel : ViewModelBase
    {
        /// <summary>
        /// Czy widok jest rozwinięty
        /// </summary>
        [ObservableProperty] private bool _isExpanded;

        /// <summary>
        /// Czy jest błąd
        /// </summary>
        [ObservableProperty] private bool _isError;

        /// <summary>
        /// Wybrany element
        /// </summary>
        [ObservableProperty] private string? _selectedElement;

        /// <summary>
        /// Lista elementów do wyboru
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> _elementsList = new ObservableCollection<string>();

        /// <summary>
        /// Czy lista jest pusta
        /// </summary>
        [ObservableProperty] private bool _isListEmpty;

        /// <summary>
        /// Akcja wykonywana w momencie wyboru elementu
        /// </summary>
        private Action _onElementSelectionAction;

        /// <summary>
        /// Teksty na przycisku i typ elementu
        /// </summary>
        [ObservableProperty] private string _elementType, _buttonText, _popupText;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="onElementSelectionAction">Akcja wykonywana w momencie wyboru elementu</param>
        /// <param name="elementType">Typ elementu</param>
        /// <exception cref="Exception">Wyrzuca wyjątek jeśli typ nie należy do możliwych typów elementów</exception>
        public ElementListViewModel(Action onElementSelectionAction, string elementType)
        {
            _isExpanded = false;
            _onElementSelectionAction = onElementSelectionAction;
            _elementType = elementType;
            _isListEmpty = true;
            switch (_elementType)
            {
                case "Patient":
                    _buttonText = "Wybierz pacjenta";
                    _popupText = "error"; // nie powinno się pojawić
                    break;
                case "Study":
                    _buttonText = "Wybierz badanie";
                    _popupText = "Najpierw wybierz pacjenta";
                    break;
                case "Image":
                    _buttonText = "Wybierz obraz";
                    _popupText = "Najpierw wybierz badanie";
                    break;
                default:
                    throw new Exception("Unknown element type in ElementListViewModel");
            }
        }

        /// <summary>
        /// Metoda ustawia nową liste elementów
        /// </summary>
        /// <param name="newElements">Lista elementów</param>
        public void UpdateElements(IEnumerable<string> newElements)
        {
            ElementsList.Clear();
            foreach (var elem in newElements)
            {
                ElementsList.Add(elem);
            }

            if (ElementsList.Count > 0)
            {
                IsError = false;
                IsListEmpty = false;
            }
        }

        /// <summary>
        /// Metoda dodająca element do listy
        /// </summary>
        /// <param name="elementName">Element do dodania do listy</param>
        public void AddElement(string elementName)
        {
            if (!ElementsList.Contains(elementName))
            {
                ElementsList.Add(elementName);
            }
            IsListEmpty = false;
        }

        /// <summary>
        /// Metoda usuwająca wszystkie elementy z listy
        /// </summary>
        public void Clear()
        {
            ElementsList.Clear();
            IsListEmpty = true;
        }

        /// <summary>
        /// Metoda wywoływana po zmianie wybranej wartosci
        /// </summary>
        /// <param name="oldValue">Poprzednio wybrany element</param>
        /// <param name="newValue">Nowo wybrany element</param>
        partial void OnSelectedElementChanged(string? oldValue, string? newValue)
        {
            if (newValue != null)
            {
                if (newValue == oldValue)
                {
                    ToggleExpantion();
                    return;
                }
                _onElementSelectionAction();
                Hide();
            }
        }

        /// <summary>
        /// Metoda ustawia wybrany element na poprzedni
        /// </summary>
        /// <remarks>
        /// Jeśli lista jest pusta lub wybrany element jest null to nic nie dzieje
        /// </remarks>
        internal void SetToPreviousElement()
        {
            if (ElementsList.Count <= 1 || SelectedElement == null)  return;

            var curr = ElementsList.IndexOf(SelectedElement);
            var prev = (curr - 1 + ElementsList.Count) % ElementsList.Count;
            SelectedElement = ElementsList[prev];
        }

        /// <summary>
        /// Metoda ustawia wybrany element na następny
        /// </summary>
        /// <remarks>
        /// Jeśli lista jest pusta lub wybrany element jest null to nic nie dzieje
        /// </remarks>
        internal void SetToNextElement()
        {
            if (ElementsList.Count <= 1 || SelectedElement == null) return;

            var curr = ElementsList.IndexOf(SelectedElement);
            var next = (curr + 1) % ElementsList.Count;
            SelectedElement = ElementsList[next];
        }

        /// <summary>
        /// Metoda zmieniająca stan widoku
        /// </summary>
        /// <remarks>
        /// Jeśli następuje próba rozwinięcia widoku i lista elementów jest pusta
        /// to ustawia błąd
        /// </remarks>
        internal void ToggleExpantion()
        {
            if (IsExpanded)
            {
                IsError = false;
                Hide();
            }
            else
            {
                if(ElementsList.Count == 0)
                    IsError = true;
                
                IsExpanded = true;
            }
        }

        /// <summary>
        /// Metoda ukrywająca listę elementów
        /// </summary>
        internal void Hide()
        {
            IsExpanded = false;
        }

        /// <summary>
        /// Metoda pokazująca listę elementów
        /// </summary>
        internal void Show()
        {
            IsExpanded = true;
        }
    }
}
