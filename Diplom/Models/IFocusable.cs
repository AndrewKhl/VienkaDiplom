using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Models
{
    public interface IFocusable
    {
        event Action FocusedElement;
        void SetFocusBorder();
        void UnsetFocusBorder();
    }
}
