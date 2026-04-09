using System.Collections.Generic;

namespace RevitExplorer.API
{
    public interface IRDBEController
    {
        void Snoop(object document, IEnumerable<object> elements);
    }
}