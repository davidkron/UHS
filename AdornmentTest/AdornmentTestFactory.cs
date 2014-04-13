using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

[assembly: System.CLSCompliant(true)]
namespace Cycles
{
    #region Adornment Factory
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("C/C++")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class AdornmentTestFactory : IWpfTextViewCreationListener
    {
        /// <summary>
        /// Defines the adornment layer for the adornment. This layer is ordered 
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]

        [Name("Cycles")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        public AdornmentLayerDefinition editorAdornmentLayer = null;
        [Import]internal ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        [Import]internal SVsServiceProvider ServiceProvider = null;

        /// <summary>
        /// Instantiates a Cycles manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            ITextDocument document;
            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
            {
                System.Diagnostics.Debug.WriteLine(document.FilePath);
                if(document.FilePath.EndsWith("uhs",System.StringComparison.OrdinalIgnoreCase))
                {
                    DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
                    new DocumentHook(textView,document,dte);
                }
            }
        }
    }
    #endregion //Adornment Factory
}
