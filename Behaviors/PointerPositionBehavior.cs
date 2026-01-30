using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace DICOMApp.Behaviors
{
    /// <summary>
    /// Behavior umożliwiający powiązanie pozycji wskaźnika myszy z komendami
    /// </summary>
    public class PointerPositionBehavior : Behavior<Image>
    {
        /// <summary>
        /// Powiązanie komendy zdarzenia naciśnięcia wskaźnika myszy
        /// </summary>
        public static readonly StyledProperty<ICommand?> PointerPressedCommandProperty =
            AvaloniaProperty.Register<PointerPositionBehavior, ICommand?>(
                nameof(PointerPressedCommand));

        /// <summary>
        /// Powiązanie komendy zdarzenia zwolnienia wskaźnika myszy
        /// </summary>
        public static readonly StyledProperty<ICommand?> PointerReleasedCommandProperty =
            AvaloniaProperty.Register<PointerPositionBehavior, ICommand?>(
                nameof(PointerReleasedCommand));

        /// <summary>
        /// Powiązanie komendy zdarzenia ruchu wskaźnika myszy
        /// </summary>
        public static readonly StyledProperty<ICommand?> PointerMovedCommandProperty =
            AvaloniaProperty.Register<PointerPositionBehavior, ICommand?>(
                nameof(PointerMovedCommand));

        /// <summary>
        /// Ustawia lub pobiera komendę wywoływaną po naciśnięciu przycisku myszy
        /// </summary>
        public ICommand? PointerPressedCommand
        {
            get => GetValue(PointerPressedCommandProperty);
            set => SetValue(PointerPressedCommandProperty, value);
        }

        /// <summary>
        /// Ustawia lub pobiera komendę wywoływaną po zwolnieniu przycisku myszy
        /// </summary>
        public ICommand? PointerReleasedCommand
        {
            get => GetValue(PointerReleasedCommandProperty);
            set => SetValue(PointerReleasedCommandProperty, value);
        }


        /// <summary>
        /// Ustawia lub pobiera komendę wywoływaną po ruchu wskaźnika myszy
        /// </summary>
        public ICommand? PointerMovedCommand
        {
            get => GetValue(PointerMovedCommandProperty);
            set => SetValue(PointerMovedCommandProperty, value);
        }

        /// <summary>
        /// Dodaje obsługę zdarzeń wskaźnika myszy do powiązanego obiektu
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if(AssociatedObject == null) return;

            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerPressed += OnPointerPressed;
            AssociatedObject.PointerReleased += OnPointerReleased;
        }

        /// <summary>
        /// Dodaje obsługę zdarzeń wskaźnika myszy do powiązanego obiektu
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null) return;

            AssociatedObject.PointerMoved -= OnPointerMoved;
            AssociatedObject.PointerPressed -= OnPointerPressed;
            AssociatedObject.PointerReleased -= OnPointerReleased;
        }


        /// <summary>
        /// Obsługuje zdarzenie ruchu wskaźnika myszy
        /// </summary>
        /// <param name="sender">Objekt wysyłający zdarzenie</param>
        /// <param name="args">Argumenty zdarzenia</param>
        private void OnPointerMoved(object? sender, PointerEventArgs args)
        {
            var p = args.GetPosition(AssociatedObject);
            if (PointerMovedCommand?.CanExecute(p) == true)
                PointerMovedCommand.Execute(p);
        }

        /// <summary>
        /// Obsługuje zdarzenie naciśnięcia przycisku myszy
        /// </summary>
        /// <param name="sender">Objekt wysyłający zdarzenie</param>
        /// <param name="e">Argumenty zdarzenia</param>
        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var p = e.GetPosition(AssociatedObject);

            if (PointerPressedCommand?.CanExecute(p) == true)
                PointerPressedCommand.Execute(p);
        }

        /// <summary>
        /// Obsługuje zdarzenie zwolnienia przycisku myszy
        /// </summary>
        /// <param name="sender">Objekt wysyłający zdarzenie</param>
        /// <param name="e">Argumenty zdarzenia</param>
        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var p = e.GetPosition(AssociatedObject);
            if (PointerReleasedCommand?.CanExecute(p) == true)
                PointerReleasedCommand.Execute(p);
        }
    }
}
