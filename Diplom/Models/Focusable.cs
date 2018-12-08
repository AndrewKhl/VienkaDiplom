using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Models
{
    interface Focusable
    {
        event Action FocusedElement;
        void UnsetFocusBorder();
    }
}
