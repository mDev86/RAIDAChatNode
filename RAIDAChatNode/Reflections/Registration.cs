﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Reflections
{
    public class Registration : IReflectionActions
    {

        /* { "execFun": "registration", 
                "data": {
                    "login": "shREk", 
                    "password": "qwerty", 
                    "nickName": "gosha",
                    "transactionId": "80f7efc032dd4a7c97f69fca51ad3001"
                }
            }
        */

        public OutputSocketMessageWithUsers Execute(object val, string myLogin)
        {
            OutputSocketMessage output = new OutputSocketMessage("registration", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();


            RegistrationInfo info = DeserializeObject.ParseJSON<RegistrationInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }

            using (var db = new RaidaContext())
            {

                if(db.Members.Any(it => it.login.Equals(info.login.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    output.success = false;
                    output.msgError = "This login already exists";
                }
                else
                {
                    Guid privateId = Guid.NewGuid();
                    Members member = new Members
                    {
                        private_id = privateId,
                        login = info.login.Trim().ToLower(),
                        pass = info.password,
                        nick_name = info.nickName,
                        last_use = 0,
                        description_fragment = "",
                        photo_fragment = new byte[5],
                        kb_bandwidth_used = 0,
                        away_busy_ready = "ready"
                    };
                    db.Members.Add(member);
                    db.SaveChanges();

                    //    Transaction.saveTransaction(info.transactionId, Transaction.TABLE_NAME.MEMBERS, privateId.ToString(), privateId);
                }
            }
            rez.msgForOwner = output;
            return rez;
        }
    }
}