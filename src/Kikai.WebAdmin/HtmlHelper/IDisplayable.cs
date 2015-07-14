
namespace Kikai.WebAdmin.HtmlHelper
{
    public interface IDisplayable
    {
        string Text { get; }

        int Value { get; }

        bool IsCheckedOrSelected { get; }
    }
}
