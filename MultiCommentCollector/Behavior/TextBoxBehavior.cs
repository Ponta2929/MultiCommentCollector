using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultiCommentCollector.Behavior
{
    public class TextBoxBehavior : Behavior<TextBox>
    {
        private ExecutedRoutedEventHandler handler;

        public TextBoxBehavior()
        {
            handler = new(PreviewExecuted);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewTextInput += PreviewTextInput;

            if (handler is not null)
                CommandManager.AddPreviewExecutedHandler(AssociatedObject, handler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewTextInput -= PreviewTextInput;

            if (handler is not null)
                CommandManager.RemovePreviewExecutedHandler(AssociatedObject, handler);
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを許可しない
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
