﻿using Caliburn.Micro;

namespace Workbench.ViewModels
{
    public class SingletonVariableEditViewModel : Screen
    {
        private string _variableName;
        private string _domainExpression;

        /// <summary>
        /// Initialize the singleton variable edit with default values.
        /// </summary>
        public SingletonVariableEditViewModel()
        {
            VariableName = string.Empty;
            DomainExpression = string.Empty;
        }

        /// <summary>
        /// Gets or sets the variable name.
        /// </summary>
        public string VariableName
        {
            get => _variableName;
            set
            {
                _variableName = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the domain expression.
        /// </summary>
        public string DomainExpression
        {
            get => _domainExpression;
            set
            {
                _domainExpression = value;
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