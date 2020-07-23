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
            string username = cp.Doc.GetText("panelLoginUsername");
            string password = cp.Doc.GetText("panelLoginPassword");
            string email = cp.Doc.GetText("panelLoginEmail");
            bool autoLogin = cp.Utils.EncodeBoolean(cp.Doc.GetText("panelLoginAuto"));
            string reqEmail = cp.Doc.GetText("panelEmailEmail");
            CPCSBaseClass cs = cp.CSNew();
            string login = "";
            //
            if (reqEmail != "") {
                if (cs.Open("People", "email=" + cp.Db.EncodeSQLText(reqEmail), "", false, "", 1, 1)) {
                    cp.Doc.SetProperty("Email Flag", "1");
                    cp.Email.sendPassword(reqEmail);
                } else {
                    cp.Doc.SetProperty("Email Error Flag", "1");
                }
                cs.Close();
                //
                s = cp.Addon.Execute(Constants.guidDefaultLoginForm);
            } else {
                //
                if (!cp.Site.GetBoolean("ALLOWEMAILLOGIN", false)) {
                    login = username;
                } else {
                    login = email;
                }
                //
                if (!cp.User.Login(login, password, autoLogin)) {
                    cp.Doc.SetProperty("Error Flag", "1");
                    s = cp.Addon.Execute(Constants.guidDefaultLoginForm);
                }
            }
            //
            return s;
        }
    }
}
