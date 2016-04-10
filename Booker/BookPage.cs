using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booker
{
    class BookPage
    {
        public int startLineNumber;
        public int endLineNumber;

        public BookPage(int sl, int el)
        {
            this.startLineNumber = sl;
            this.endLineNumber = el;
        }
    }
}
