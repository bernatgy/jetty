using System;
using System.Collections.Generic;

namespace Jetty
{
    public abstract class AppStore : IDisposable
    {
        public abstract void Reload();
        public abstract List<Widget> GetAll();
        public abstract Widget Get(int index);
        public abstract void Store();
        public abstract void Add(Widget item);
        public abstract void Remove(Widget item);
        public abstract void Dispose();
    }
}
