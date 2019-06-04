using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel {

    //
    //====================================================================================================
    //
    public class defaultLoginFormHandlerClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string s = "";
            string username = cp.Doc.get_Var("panelLoginUsername");
            string password = cp.Doc.get_Var("panelLoginPassword");
            string email = cp.Doc.get_Var("panelLoginEmail");
            bool autoLogin = cp.Utils.EncodeBoolean(cp.Doc.get_Var("panelLoginAuto"));
            string reqEmail = cp.Doc.get_Var("panelEmailEmail");
            CPCSBaseClass cs = cp.CSNew();
            string login = "";
            //
            if (reqEmail != "") {
                if (cs.Open("People", "email=" + cp.Db.EncodeSQLText(reqEmail), "", false, "", 1, 1)) {
                    cp.Doc.set_Var("Email Flag", "1");
                    cp.Email.sendPassword(reqEmail);
                } else {
                    cp.Doc.set_Var("Email Error Flag", "1");
                }
                cs.Close();
                //
                s = cp.Utils.ExecuteAddon(Constants.guidDefaultLoginForm);
            } else {
                //
                if (!cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWEMAILLOGIN", ""))) {
                    login = username;
                } else {
                    login = email;
                }
                //
                if (!cp.User.Login(login, password, autoLogin)) {
                    cp.Doc.set_Var("Error Flag", "1");
                    s = cp.Utils.ExecuteAddon(Constants.guidDefaultLoginForm);
                }
            }
            //
            return s;
        }
    }
}
