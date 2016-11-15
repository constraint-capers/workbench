using Caliburn.Micro;

namespace Workbench.ViewModels
{
    public class SolutionEditorViewModel : Screen
    {
        private string bindingExpression;

        /// <summary>
        /// Gets or sets the visualizer binding expression.
        /// </summary>
        public string BindingExpression
        {
            get { return this.bindingExpression; }
            set
            {
                this.bindingExpression = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Okay button clicked.
        /// </summary>
        public void AcceptButton()
        {
            TryClose(true);
        }
    }
}
