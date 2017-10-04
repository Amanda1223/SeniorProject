/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : IntClient.cs
 * Description : Interface of function calls to be used in the client-side code. Details explained
 *                  within the Implementation file ImpClient.cs
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    [ServiceContract]
    public interface IntClient
    {

        [OperationContract]
        void frwdMsg(string a_message, string a_user);
        
        [OperationContract]
        void whisperMsg(string a_msg, string a_user);
        
        [OperationContract]
        void addUserToList(string a_connected);

        [OperationContract]
        void removeUserFromList(string a_disconnected);
    }
}
