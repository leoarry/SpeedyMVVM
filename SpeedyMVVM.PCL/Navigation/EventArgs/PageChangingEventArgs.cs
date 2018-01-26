using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Navigation
{
    public class PageChangingEventArgs:EventArgs
    {
        public NavigationRequestEnum NavigationRequest;

        public PageChangingEventArgs(NavigationRequestEnum request)
        {
            NavigationRequest = request;
        }
    }
}
