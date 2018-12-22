using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    interface IConnectable
    {
		List<ConnectionLine> connectionLines { get; }
	}
}
