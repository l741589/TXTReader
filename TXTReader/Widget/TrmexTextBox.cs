using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TXTReader.Widget {
    class TrmexTextBox : RichTextBox {
        public TrmexTextBox()
            : base() {
            TextChanged += TrmexTextBox_TextChanged;
        }


        public String Text {
            get {
                return new TextRange(Document.ContentStart, Document.ContentEnd).Text; 
            }
            set {
                Document = new FlowDocument(new Paragraph(new Run(value))); 
            }
        }

        void TrmexTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            ToolTip = Text;
        }

    }
}
