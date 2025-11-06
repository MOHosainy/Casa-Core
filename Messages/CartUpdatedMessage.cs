using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OURSTORE.Messages
{
    public class CartUpdatedMessage : ValueChangedMessage<bool>
    {
        public CartUpdatedMessage(bool value) : base(value) { }
    }
}
