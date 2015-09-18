﻿using System;
using System.Windows;
using Dyna.Core.Models;

namespace DynaApp.ViewModels
{
    /// <summary>
    /// View model for a variable.
    /// </summary>
    public sealed class VariableViewModel : GraphicViewModel
    {
        private VariableModel model;
        private VariableDomainExpressionViewModel domainExpression;

        /// <summary>
        /// Initialize a variable with the new name.
        /// </summary>
        public VariableViewModel(string newName, Point newLocation, VariableDomainExpressionViewModel newDomainExpression)
            : base(newName, newLocation)
        {
            if (newDomainExpression == null)
                throw new ArgumentNullException("newDomainExpression");

            this.Model = new VariableModel();
            this.DomainExpression = newDomainExpression;
        }

        /// <summary>
        /// Initialize a variable with the new name.
        /// </summary>
        public VariableViewModel(string newName, Point newLocation)
            : base(newName, newLocation)
        {
            this.Model = new VariableModel();
            this.DomainExpression = new VariableDomainExpressionViewModel();
        }

        /// <summary>
        /// Initialize a variable with the new name.
        /// </summary>
        public VariableViewModel(string newName)
            : base(newName)
        {
            this.Model = new VariableModel();
            this.DomainExpression = new VariableDomainExpressionViewModel();
        }

        /// <summary>
        /// Initialize a variable with default values.
        /// </summary>
        public VariableViewModel()
            : this("New variable")
        {
        }

        /// <summary>
        /// Gets or sets the domain expression.
        /// </summary>
        public VariableDomainExpressionViewModel DomainExpression
        {
            get
            {
                return this.domainExpression;
            }
            set
            {
                this.domainExpression = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the variable model.
        /// </summary>
        public new VariableModel Model
        {
            get { return this.model; }
            set
            {
                base.Model = value;
                this.model = value;
            }
        }
    }
}
