﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuppressMessageBulbItem.cs" company="http://stylecop.codeplex.com">
//   MS-PL
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the Microsoft 
//   Public License. A copy of the license can be found in the License.html 
//   file at the root of this distribution. If you cannot locate the  
//   Microsoft Public License, please send an email to dlr@microsoft.com. 
//   By using this source code in any fashion, you are agreeing to be bound 
//   by the terms of the Microsoft Public License. You must not remove this 
//   notice, or any other, from this software.
// </license>
// <summary>
//   BulbItem - SuppressMessageBulbItem.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace StyleCop.ReSharper513.BulbItems.Framework
{
    #region Using Directives

    using JetBrains.Application.Progress;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.CodeStyle;
    using JetBrains.ReSharper.Psi.CSharp;
    using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.TextControl;

    using StyleCop.ReSharper513.Core;
    using StyleCop.ReSharper513.Options;

    #endregion

    /// <summary>
    /// BulbItem - SuppressMessageBulbItem.
    /// </summary>
    internal class SuppressMessageBulbItem : V5BulbItemImpl
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Rule.
        /// </summary>
        public Rule Rule { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute inner.
        /// </summary>
        /// <param name="solution">
        /// The solution.
        /// </param>
        /// <param name="textControl">
        /// The text control.
        /// </param>
        public override void ExecuteTransactionInner(ISolution solution, ITextControl textControl)
        {
            IDeclaration declaration = Utils.GetDeclarationClosestToTextControl(solution, textControl);

            if (declaration != null)
            {
                string rulesNamespace = this.Rule.Namespace;

                string ruleText = string.Format("{0}:{1}", this.Rule.CheckId, this.Rule.Name);

                string justificationText = StyleCopOptions.Instance.SuppressStyleCopAttributeJustificationText;

                IAttributesOwnerDeclaration attributesOwnerDeclaration = declaration as IAttributesOwnerDeclaration;

                if (attributesOwnerDeclaration != null)
                {
                    CSharpElementFactory factory = CSharpElementFactory.GetInstance(declaration.GetPsiModule());

                    ITypeElement typeElement = Utils.GetTypeElement(declaration, "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute");

                    IAttribute attribute = factory.CreateAttribute(typeElement);

                    ICSharpArgument newArg1 = attribute.AddArgumentAfter(Utils.CreateConstructorArgumentValueExpression(declaration.GetPsiModule(), rulesNamespace), null);

                    ICSharpArgument newArg2 = attribute.AddArgumentAfter(Utils.CreateConstructorArgumentValueExpression(declaration.GetPsiModule(), ruleText), newArg1);

                    attribute.AddArgumentAfter(Utils.CreateArgumentValueExpression(declaration.GetPsiModule(), "Justification = \"" + justificationText + "\""), newArg2);

                    attributesOwnerDeclaration.AddAttributeAfter(attribute, null);

                    IFile file = declaration.ToTreeNode().GetContainingFile();

                    CSharpFormatterHelper.FormatterInstance.FormatFile(
                        file, SolutionCodeStyleSettings.GetInstance(solution).CodeStyleSettings, CodeFormatProfile.DEFAULT, NullProgressIndicator.Instance);
                }
            }
        }

        #endregion
    }
}