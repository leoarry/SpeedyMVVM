using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    public interface IMessenger
    {
        void Register<Tmessage>(Action<Tmessage> callBack, object validationToken = null);
        void UnRegister<Tmessage>(Action<Tmessage> callBack);
        void UnRegister(object validationToken);
        bool Send<Tmessage>(object sender, Tmessage message, bool notifySender = false, Func<Tmessage, object, bool> tokenValidationRule = null);
        
        void CleanUp();
    }
}
