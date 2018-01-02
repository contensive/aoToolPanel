using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using static Contensive.Addons.aoToolPanel.constants;

namespace Contensive.Addons.aoToolPanel
{
    //
    //====================================================================================================
    //
    public class toolPanelAccountFormHandlerClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            bool errFlag = false;
            string firstName = cp.Doc.get_Var("panelAccountFirstName");
            string lastName = cp.Doc.get_Var("panelAccountLastName");
            string email = cp.Doc.get_Var("panelAccountEmail");
            string username = cp.Doc.get_Var("panelAccountUsername");
            string password = cp.Doc.get_Var("panelAccountPassword");
            CPCSBaseClass cs = cp.CSNew();
            string sql = "";
            // if allowEmailLogin -- then ignore username input, it is not valid
            bool allowEmailLogin = cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWEMAILLOGIN", ""));
            //
            // check input
            //
            if ((firstName == "") | (lastName == "") | (email == "") | (password == "") | ((username == "") & (!allowEmailLogin))) {
                //
                // missing fields
                //
                errFlag = true;
                cp.Doc.set_Var("errFlag", "1");
                cp.Doc.set_Var("errMessage", "One or more required fields were empty.");
            } else {
                //
                //  check for duplicate in username if account requires username
                //
                if (allowEmailLogin) {
                    //
                    // email loging, check email provided against email and username fields
                    //
                    sql = "(ID<>" + cp.User.Id + ")"
                        + "and("
                            + "(username=" + cp.Db.EncodeSQLText(email) + ")"
                            + "or(email=" + cp.Db.EncodeSQLText(email) + ")"
                        + ")"
                        + "";
                } else {
                    //
                    // check username and email provided against both email and username fields
                    //
                    sql = "(ID<>" + cp.User.Id + ")"
                        + "and("
                            + "(username=" + cp.Db.EncodeSQLText(email) + ")"
                            + "or(email=" + cp.Db.EncodeSQLText(email) + ")"
                            + "or(username=" + cp.Db.EncodeSQLText(username) + ")"
                            + "or(email=" + cp.Db.EncodeSQLText(username) + ")"
                        + ")"
                        + "";
                }
                if (cs.Open("People", sql, "", false, "", 1, 1)) {
                    errFlag = true;
                    cp.Doc.set_Var("errFlag", "1");
                    cp.Doc.set_Var("errMessage", "The login requested is not available, please enter an alternate username.");
                }
                cs.Close();
            }
            //
            if (!errFlag) {
                if (cs.Open("People", "ID=" + cp.User.Id, "", false, "", 1, 1)) {
                    cs.SetField("name", firstName + " " + lastName);
                    cs.SetField("firstName", firstName);
                    cs.SetField("lastName", lastName);
                    cs.SetField("email", email);
                    cs.SetField("password", password);
                    if (!allowEmailLogin) {
                        cs.SetField("username", username);
                    }
                }
                cs.Close();
                cp.Doc.set_Var("acctUpdated", "1");
            }
            //
            // -- return account form
            return cp.Utils.ExecuteAddon(guidToolPanelAccountForm); ;
        }
    }
}
