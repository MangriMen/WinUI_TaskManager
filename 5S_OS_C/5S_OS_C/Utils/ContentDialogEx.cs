using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace _5S_OS_C.Utils
{
    public class ContentDialogEx : ContentDialog
    {
        public string Result
        {
            get
            {
                foreach (FrameworkElement child in (Content as StackPanel).Children)
                {
                    if (child.Name == "Result")
                    {
                        return (child as TextBox).Text;
                    }
                }

                return null;
            }
        }

        private static ContentDialogEx CreateBaseDialogWithContent(XamlRoot parentXamlRoot, string title, string message, string closeButtonText, UIElement content)
        {
            StackPanel dlgContent = new()
            {
                Orientation = Orientation.Vertical,
                Spacing = 10,
            };
            dlgContent.Children.Add(new TextBlock()
            {
                Text = message,
                Name = "AdditionalMessage"
            });

            if (content != null)
            {
                dlgContent.Children.Add(content);
            }

            return new()
            {
                Title = title,
                CloseButtonText = closeButtonText,
                XamlRoot = parentXamlRoot,
                Content = dlgContent
            };
        }
        public static ContentDialogEx Request(XamlRoot parentXamlRoot, string title, string message, string closeButtonText)
        {
            return CreateBaseDialogWithContent(parentXamlRoot, title, message, closeButtonText, new TextBox()
            {
                Name = "Result"
            });
        }

        public static ContentDialogEx Exception(XamlRoot parentXamlRoot, string title, string message, string closeButtonText)
        {
            return CreateBaseDialogWithContent(parentXamlRoot, title, message, closeButtonText, null);
        }
    }
}
