using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    public interface IBroadcastClient
    {
        void AddClient<TMessage>(Action<TMessage> callBackAction, object validationToken = null);

        void RemoveClient(object client);
                
        void SendMessage<TMessage>(object sender, TMessage message, bool notifySender = false, Func<TMessage, object, bool> tokenValidationRule = null);
    }
}
