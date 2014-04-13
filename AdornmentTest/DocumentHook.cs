using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Cycles;
using Microsoft.VisualStudio.VCProjectEngine;

namespace Cycles
{
    ///<summary>
    ///Cycles places red boxes behind all the "A"s in the editor window
    ///</summary>+        
    public class DocumentHook
    {
        //IAdornmentLayer _layer;
        //IWpfTextView _view;
        UHSFile uhs;
        bool changed = true;//Allways allow one first save without changing the doc

        public DocumentHook(IWpfTextView view,ITextDocument doc,EnvDTE.DTE dte)
        {
           // _view = view;
            ITextBuffer buffer = view.TextBuffer;
            //_layer = view.GetAdornmentLayer("Cycles");
            uhs = new UHSFile(doc,dte);
            buffer.Changed += buffer_Changed;
            doc.FileActionOccurred += doc_FileActionOccurred;
        }

        void doc_FileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
           if( e.FileActionType == FileActionTypes.ContentSavedToDisk && changed)
           {
               uhs.parse();
               changed = false;
           }
        }

        void buffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            changed = true;
        }

    }
}
