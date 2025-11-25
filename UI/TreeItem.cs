using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FLandT_laba1_ver4.UI
{
    public sealed class TreeItem : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get => _text;
            set { if (_text == value) return; _text = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TreeItem> Children { get; } = new();

        public TreeItem(string text) => _text = text;

        public void Add(TreeItem child) => Children.Add(child);
        public override string ToString() => Text;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}