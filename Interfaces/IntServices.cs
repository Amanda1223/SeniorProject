/*
 * Name : Amanda Steidl
 * Course : Senior Project
 * Professor : Victor Miller
 * Project : Senior Project-Final
 * Due Date : October 4, 2017
 * Current File : IntServices.cs
 * Description : Interface of function calls to be used in the client-side code. Details explained
 *                  within the Implementation file ImpServices.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Interfaces
{
    [ServiceContract(CallbackContract = typeof(IntClient))]

    /*
     * Interface for ImpServices.cs, allowing the client-side to use its functionality.
     * [OperationContract] <function_name> are all the functions able to be called in the client.
     */
    public interface IntServices
    {
        [OperationContract]
        int login(string a_username, ref List<string> a_userList);

        [OperationContract]
        void sendToAll(string a_user, string a_msg);
        
        [OperationContract]
        void sendToUser(string a_dest, string a_from, string a_msg);
        
        [OperationContract]
        void newClientConnection(string a_connected);
        
        [OperationContract]
        void disconnectionHandle(string a_currentUser);
    }
}
