using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_RichEditBox_Search_Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private bool _formatChanged = false;
        private string _oldQuery = string.Empty;
        private Color _highLihgtColor = Color.FromArgb(255, 150, 190, 255);
        private List<ITextRange> _foundKeys = new List<ITextRange>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void txtContent_Loaded(object sender, RoutedEventArgs e)
        {
            // Loading Sample File
            var sampleFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SampleText/SampleTextFile.txt"));
            using (Stream st = (await sampleFile.OpenReadAsync()).AsStream())
            using (TextReader reader = new StreamReader(st))
            {
                this.txtContent.Document.SetText(Windows.UI.Text.TextSetOptions.None, await reader.ReadToEndAsync());
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string query = txtSearchBox.Text;
            FindText(query, true);
        }

        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchBox.Text.Length == 0)
                ResetTextFormat();
        }

        private void txtSearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string query = txtSearchBox.Text;
                FindText(query);
            }
        }

        private void FindText(string query, bool Isfocus = false)
        {
            if (string.IsNullOrEmpty(query))
                return;

            if (!_oldQuery.Equals(query))
                ResetTextFormat();

            _oldQuery = query;
            string docText;
            txtContent.Document.GetText(Windows.UI.Text.TextGetOptions.None, out docText);
            var start = txtContent.Document.Selection.EndPosition;
            var end = docText.Length;
            var range = txtContent.Document.GetRange(start, end);
            int result = range.FindText(query, end - start, Windows.UI.Text.FindOptions.None);
            _foundKeys.Add(range);

            if (result == 0)
            {
                txtContent.Document.Selection.SetRange(0, 0);
            }
            else
            {
                txtContent.Document.Selection.SetRange(range.StartPosition, range.EndPosition);
                range.CharacterFormat.BackgroundColor = _highLihgtColor;
                _formatChanged = true;
                range.ScrollIntoView(Windows.UI.Text.PointOptions.None);
                if (Isfocus)
                    txtContent.Focus(FocusState.Keyboard);
            }
        }

        private void ResetTextFormat()
        {
            if (_formatChanged)
            {
                var bkcolor = ((SolidColorBrush)txtContent.Background).Color;
                foreach (var item in _foundKeys)
                    item.CharacterFormat.BackgroundColor = bkcolor;
                _formatChanged = false;
                _foundKeys.Clear();
            }
        }
    }
}
