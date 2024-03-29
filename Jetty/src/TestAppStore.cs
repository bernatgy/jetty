﻿using System.Collections.Generic;

namespace Jetty
{
    public class TestAppStore : AppStore
    {
        private List<Widget> _launchers = new List<Widget>();

        public TestAppStore()
        {
            this._launchers.Add(new AppLauncherWidget("Mozilla Firefox", Properties.Resources.HP_Firefox_icon, @"C:\Program Files\Mozilla Firefox\firefox.exe"));
            this._launchers.Add(new AppLauncherWidget("Mozilla Firefoxx", Properties.Resources.HP_Firefox_icon, @"C:\Program Files\Mozilla Firefox\firefox.exe"));
            this._launchers.Add(new AppLauncherWidget("Mozilla Firefoxxx", Properties.Resources.HP_Firefox_icon, @"C:\Program Files\Mozilla Firefox\firefox.exe"));
        }

        public override void Add(Widget item)
        {
            this._launchers.Add(item);
        }

        public override void Remove(Widget item)
        {
            if (this._launchers.Contains(item))
                this._launchers.Remove(item);
        }

        public override Widget Get(int index)
        {
            return this._launchers[index];
        }

        public override List<Widget> GetAll()
        {
            return this._launchers;
        }

        public override void Reload()
        { }

        public override void Store()
        { }

        public override void Dispose()
        { }
    }
}
