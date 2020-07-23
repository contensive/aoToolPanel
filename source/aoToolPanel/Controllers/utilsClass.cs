//using System;
//using System.Collections.Generic;
//using System.Text;
//using Contensive.BaseClasses;

//namespace Contensive.Addons.aoToolPanel
//{
//    public class userAuthenticatedClass : Contensive.BaseClasses.AddonBaseClass
//    {
//        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
//        {
//            string s = "";
//            string username = cp.Doc.GetText("panelLoginUsername");
//            string password = cp.Doc.GetText("panelLoginPassword");
//            string email = cp.Doc.GetText("panelLoginEmail");
//            bool autoLogin = cp.Utils.EncodeBoolean(cp.Doc.GetText("panelLoginAuto"));
//            CPCSBaseClass cs = cp.CSNew();
//            //
//            if (cp.Site.GetBoolean("ALLOWEMAILLOGIN", ""))
//            {
//                if (cs.Open("People", "email=" + cp.Db.EncodeSQLText(email), "", false, "", 1, 1))
//                {
//                    cp.User.LoginByID(cs.GetInteger("ID").toString(), autoLogin);
//                }
//                cs.Close();
//            }
//            else
//            {
//                cp.User.Login(username, password, autoLogin);
//            }
//            if (cp.User.IsAuthenticated())
//            {
//                s = "true";
//            }
//            else
//            {
//                s = "false";
//            }
//            //
//            return s;
//        }
//    }
//}
