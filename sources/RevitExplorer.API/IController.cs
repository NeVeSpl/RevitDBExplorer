using System.Collections.Generic;

namespace RevitExplorer.API
{
    public interface IController
    {
        void Snoop(object document, IEnumerable<object> elements);
    }
}