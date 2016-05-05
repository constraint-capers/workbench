﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Google.OrTools.ConstraintSolver;
using Workbench.Core.Models;
using Workbench.Core.Nodes;
using Workbench.Core.Parsers;

namespace Workbench.Core.Solver
{
    /// <summary>
    /// Process a constraint repeater by expanding the expression the 
    /// appropriate number of times.
    /// </summary>
    internal class Repeater
    {
        private RepeaterContext context;
        private readonly OrToolsCache cache;
        private readonly Google.OrTools.ConstraintSolver.Solver solver;

        public Repeater(Google.OrTools.ConstraintSolver.Solver theSolver,
                        OrToolsCache theCache)
        {
            Contract.Requires<ArgumentNullException>(theSolver != null);
            Contract.Requires<ArgumentNullException>(theCache != null);
            this.solver = theSolver;
            this.cache = theCache;
        }

        public void Process(RepeaterContext theContext)
        {
            Contract.Requires<ArgumentNullException>(theContext != null);
            this.context = theContext;
            if (!theContext.HasRepeaters)
                ProcessSimpleConstraint(theContext.Constraint.Expression.Node);
            else
            {
                var expressionTemplateWoutExpanderText = StripExpanderFrom(theContext.Constraint.Expression.Text);
                while (theContext.Next())
                {
                    var expressionText = InsertCounterValuesInto(expressionTemplateWoutExpanderText);
                    var expandedConstraintExpressionResult = new ConstraintExpressionParser().Parse(expressionText);
                    ProcessSimpleConstraint(expandedConstraintExpressionResult.Root);
                }
            }
        }

        public RepeaterContext CreateContextFrom(ExpressionConstraintModel constraint)
        {
            return new RepeaterContext(constraint);
        }

        private void ProcessSimpleConstraint(ConstraintExpressionNode constraintExpressionNode)
        {
            Contract.Requires<ArgumentNullException>(constraintExpressionNode != null);
            Constraint newConstraint;
            var lhsExpr = GetExpressionFrom(constraintExpressionNode.InnerExpression.LeftExpression);
            if (constraintExpressionNode.InnerExpression.RightExpression.IsExpression)
            {
                var rhsExpr = GetExpressionFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsExpr);
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.IsVarable)
            {
                var rhsVariable = GetVariableFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsVariable);
            }
            else
            {
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator,
                                                   lhsExpr,
                                                   constraintExpressionNode.InnerExpression.RightExpression.GetLiteral());
            }
            this.solver.Add(newConstraint);
        }

        private Constraint CreateConstraintBy(OperatorType operatorType, IntExpr lhsExpr, IntExpr rhsExpr)
        {
            switch (operatorType)
            {
                case OperatorType.Equals:
                    return this.solver.MakeEquality(lhsExpr, rhsExpr);

                case OperatorType.NotEqual:
                    return this.solver.MakeNonEquality(lhsExpr, rhsExpr);

                case OperatorType.GreaterThanOrEqual:
                    return this.solver.MakeGreaterOrEqual(lhsExpr, rhsExpr);

                case OperatorType.LessThanOrEqual:
                    return this.solver.MakeLessOrEqual(lhsExpr, rhsExpr);

                case OperatorType.Greater:
                    return this.solver.MakeGreater(lhsExpr, rhsExpr);

                case OperatorType.Less:
                    return this.solver.MakeLess(lhsExpr, rhsExpr);

                default:
                    throw new NotImplementedException("Not sure how to represent this operator type.");
            }
        }

        private Constraint CreateConstraintBy(OperatorType operatorType, IntExpr lhsExpr, int rhs)
        {
            switch (operatorType)
            {
                case OperatorType.Equals:
                    return this.solver.MakeEquality(lhsExpr, rhs);

                case OperatorType.NotEqual:
                    return this.solver.MakeNonEquality(lhsExpr, rhs);

                case OperatorType.GreaterThanOrEqual:
                    return this.solver.MakeGreaterOrEqual(lhsExpr, rhs);

                case OperatorType.LessThanOrEqual:
                    return this.solver.MakeLessOrEqual(lhsExpr, rhs);

                case OperatorType.Greater:
                    return this.solver.MakeGreater(lhsExpr, rhs);

                case OperatorType.Less:
                    return this.solver.MakeLess(lhsExpr, rhs);

                default:
                    throw new NotImplementedException("Not sure how to represent this operator type.");
            }
        }

        private IntExpr GetExpressionFrom(ExpressionNode theExpression)
        {
            if (theExpression.IsExpression)
            {
                IntExpr variableExpression;
                VariableExpressionOperatorType op;
                LiteralNode literal;
                if (theExpression.IsSingletonExpression)
                {
                    var singletonExpression = (SingletonVariableReferenceExpressionNode)theExpression.InnerExpression;
                    variableExpression = GetVariableFrom((SingletonVariableReferenceExpressionNode)theExpression.InnerExpression);
                    op = singletonExpression.Operator;
                    literal = singletonExpression.Literal;
                }
                else
                {
                    var aggregateExpression = (AggregateVariableReferenceExpressionNode)theExpression.InnerExpression;
                    variableExpression = GetVariableFrom((AggregateVariableReferenceExpressionNode)theExpression.InnerExpression);
                    op = aggregateExpression.Operator;
                    literal = aggregateExpression.Literal;
                }

                switch (op)
                {
                    case VariableExpressionOperatorType.Add:
                        return this.solver.MakeSum(variableExpression, literal.Value);

                    case VariableExpressionOperatorType.Minus:
                        return this.solver.MakeSum(variableExpression, -literal.Value);

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return GetVariableFrom(theExpression);
            }
        }

        private IntVar GetVariableFrom(ExpressionNode theExpression)
        {
            Debug.Assert(!theExpression.IsLiteral);

            if (theExpression.IsSingletonReference)
            {
                var singletonVariableReference = (SingletonVariableReferenceNode)theExpression.InnerExpression;
                return this.cache.GetSingletonVariableByName(singletonVariableReference.VariableName);
            }

            var aggregateVariableReference = (AggregateVariableReferenceNode)theExpression.InnerExpression;
            return this.cache.GetAggregateVariableByName(aggregateVariableReference.VariableName, aggregateVariableReference.Subscript);
        }

        private IntExpr GetVariableFrom(AggregateVariableReferenceExpressionNode aggregateExpression)
        {
            return this.cache.GetAggregateVariableByName(aggregateExpression.VariableReference.VariableName, aggregateExpression.VariableReference.Subscript);
        }

        private IntExpr GetVariableFrom(SingletonVariableReferenceExpressionNode singletonExpression)
        {
            return this.cache.GetSingletonVariableByName(singletonExpression.VariableReference.VariableName);
        }

        private string StripExpanderFrom(string expressionText)
        {
            var expanderKeywordPos = expressionText.IndexOf("|", StringComparison.Ordinal);
            var raw = expressionText.Substring(0, expanderKeywordPos);
            return raw.Trim();
        }

        private string InsertCounterValuesInto(string expressionTemplateText)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(expressionTemplateText));

            var accumulatingTemplateText = expressionTemplateText;
            foreach (var aCounter in this.context.Counters)
            {
                accumulatingTemplateText = InsertCounterValueInto(accumulatingTemplateText,
                                                                  aCounter.CounterName,
                                                                  aCounter.CurrentValue);
            }

            return accumulatingTemplateText;
        }

        private string InsertCounterValueInto(string expressionTemplateText, string counterName, int counterValue)
        {
            return expressionTemplateText.Replace(counterName, Convert.ToString(counterValue));
        }
    }
}