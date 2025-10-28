using System.Windows.Input;

namespace FLandT_laba1_ver4.Common
{
    public sealed class DelegateCommand : ICommand
    {
        // Что выполняем по факту.
        private readonly Action _exec;

        // Когда команда доступна. Если null — считаем, что всегда доступна.
        private readonly Func<bool>? _can;

        public DelegateCommand(Action exec, Func<bool>? can = null) { _exec = exec; _can = can; }

        public bool CanExecute(object? parameter) => _can?.Invoke() ?? true;

        public void Execute(object? parameter) => _exec();

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
