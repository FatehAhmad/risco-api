using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Utility;

namespace RiscoNotificationService
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task.Run(() => Utility.GenerateEmailNotification());
            Utility.GenerateEmailNotification();
            
        }
    }
}
