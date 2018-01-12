﻿using Caliburn.Micro;
using Workbench.ViewModels;
using Moq;
using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Services;

namespace Workbench.UI.Tests.Unit.ViewModels
{
    [TestFixture]
    public class WorkAreaViewModelEmptyTests
    {
        [Test]
        public void SolveWithValidModelReturnsSuccessStatus()
        {
            var sut = CreateValidWorkspace();
            var actualStatus = sut.SolveModel();
            Assert.That(actualStatus.IsSuccess, Is.True);
        }

        private static WorkAreaViewModel CreateValidWorkspace()
        {
            var workspaceViewModel = new WorkAreaViewModel(CreateDataService(),
                                                            CreateWindowManager(),
                                                            CreateEventAggregator(),
                                                            CreateViewModelService(),
                                                            CreateViewModelFactory());
            var variableViewModel = new SingletonVariableViewModel(new SingletonVariableGraphicModel(workspaceViewModel.Model.Model, "x"),
                                                                   Mock.Of<IEventAggregator>());
            workspaceViewModel.Model.AddSingletonVariable(variableViewModel);
            variableViewModel.DomainExpression.Text = "1..10";
            var constraintViewModel = new ExpressionConstraintViewModel(new ExpressionConstraintGraphicModel("x", string.Empty));
            workspaceViewModel.Model.AddConstraint(constraintViewModel);
            constraintViewModel.Expression.Text = "$x > 1";

            return workspaceViewModel;
        }

        private static IDataService CreateDataService()
        {
            return new DataService(Mock.Of<IWorkspaceReaderWriter>());
        }

        private static IViewModelFactory CreateViewModelFactory()
        {
            return new ViewModelFactory(CreateEventAggregator(), CreateWindowManager());
        }

        private static IViewModelService CreateViewModelService()
        {
            return new ViewModelService(CreateViewModelFactory());
        }

        private static IEventAggregator CreateEventAggregator()
        {
            return new Mock<IEventAggregator>().Object;
        }

        private static IWindowManager CreateWindowManager()
        {
            return new Mock<IWindowManager>().Object;
        }
    }
}