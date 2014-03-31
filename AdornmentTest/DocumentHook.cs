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
        IAdornmentLayer _layer;
        IWpfTextView _view;
        UHSFile uhs;
        bool changed = true;//Allways allow one first save without changing the doc

        public DocumentHook(IWpfTextView view,ITextDocument doc,EnvDTE.DTE dte)
        {
            _view = view;
            ITextBuffer buffer = view.TextBuffer;
            _layer = view.GetAdornmentLayer("Cycles");
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

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            /*
            System.Diagnostics.Debug.WriteLine(e.NewSnapshot.ContentType.TypeName);
            //System.Diagnostics.Debug.WriteLine(e.);
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                this.CreateVisuals(line);
            }*/
        }

        /// <summary>
        /// Within the given line add the scarlet box behind the a
        /// </summary>
        private void CreateVisuals(ITextViewLine line)
        {
            /*
            //grab a reference to the lines in the current TextView 
            IWpfTextViewLineCollection textViewLines = _view.TextViewLines;
            int start = line.Start;
            int end = line.End;

            //Loop through each character, and place a box around any a 
            for (int i = start; (i < end); ++i)
            {
                if (_view.TextSnapshot[i] == 'a')
                {
                    SnapshotSpan span = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(i, i + 1));
                    Geometry g = textViewLines.GetMarkerGeometry(span);
                    if (g != null)
                    {
                        GeometryDrawing drawing = new GeometryDrawing(_brush, _pen, g);
                        drawing.Freeze();

                        DrawingImage drawingImage = new DrawingImage(drawing);
                        drawingImage.Freeze();

                        Image image = new Image();
                        image.Source = drawingImage;

                        //Align the image with the top of the bounds of the text geometry
                        Canvas.SetLeft(image, g.Bounds.Left);
                        Canvas.SetTop(image, g.Bounds.Top);

                        _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
                    }
                }
            }*/
        }
    }
}
