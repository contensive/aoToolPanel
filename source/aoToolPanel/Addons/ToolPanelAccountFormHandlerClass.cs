
using System;
using Contensive.BaseClasses;
using static Contensive.Addons.aoToolPanel.Constants;

namespace Contensive.Addons.aoToolPanel {
    //
    //====================================================================================================
    //
    public class ToolPanelAccountFormHandlerClass : AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(CPBaseClass cp) {
            try {
                if (!cp.User.IsAuthenticated) { return ""; }
                bool errFlag = false;
                string firstName = cp.Doc.GetText("panelAccountFirstName");
                string lastName = cp.Doc.GetText("panelAccountLastName");
                string email = cp.Doc.GetText("panelAccountEmail");
                string username = cp.Doc.GetText("panelAccountUsername");
                string password = cp.Doc.GetText("panelAccountPassword");
                // if allowEmailLogin -- then ignore username input, it is not valid
                bool allowEmailLogin = cp.Site.GetBoolean("ALLOWEMAILLOGIN", false);
                //
                // check input
                //
                if ((firstName == "") | (lastName == "") | (email == "") | ((username == "") & (!allowEmailLogin))) {
                    //
                    // missing fields
                    //
                    errFlag = true;
                    cp.Doc.SetProperty("errFlag", "1");
                    cp.Doc.SetProperty("errMessage", "One or more required fields were empty.");
                } else {
                    string sql;
                    //
                    //  check for duplicate in username if account requires username
                    //
                    if (allowEmailLogin) {
                        //
                        // email login, check email provided against email and username fields
                        //
                        sql = $"(ID<>{cp.User.Id})"
                            + "and("
                                + $"(username={cp.Db.EncodeSQLText(email)})"
                                + $"or(email={cp.Db.EncodeSQLText(email)})"
                            + ")";
                    } else {
                        //
                        // check username and email provided against both email and username fields
                        //
                        sql = $"(ID<>{cp.User.Id})"
                            + "and("
                                + $"(username={cp.Db.EncodeSQLText(email)})"
                                + $"or(email={cp.Db.EncodeSQLText(email)})"
                                + $"or(username={cp.Db.EncodeSQLText(username)})"
                                + $"or(email={cp.Db.EncodeSQLText(username)})"
                            + ")";
                    }
                    using (CPCSBaseClass cs = cp.CSNew()) {
                        if (cs.Open("People", sql, "", false, "", 1, 1)) {
                            errFlag = true;
                            cp.Doc.SetProperty("errFlag", "1");
                            cp.Doc.SetProperty("errMessage", "The login requested is not available, please enter an alternate username.");
                        }
                    }
                }
                //
                if (!errFlag) {
                    using (CPCSBaseClass cs = cp.CSNew()) {
                        if (cs.Open("People", $"ID={cp.User.Id}", "", false, "", 1, 1)) {
                            cs.SetField("name", $"{firstName} {lastName}");
                            cs.SetField("firstName", firstName);
                            cs.SetField("lastName", lastName);
                            cs.SetField("email", email);
                            if (!string.IsNullOrEmpty(password)) {
                                cs.SetField("password", password);
                            }
                            if (!allowEmailLogin) {
                                cs.SetField("username", username);
                            }
                        }
                    }
                    cp.Doc.SetProperty("acctUpdated", "1");
                }
                //
                // -- return account form
                return cp.Addon.Execute(guidToolPanelAccountForm);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "ToolPanelAccountFormHandlerClass.Execute");
            }
            return "";
        }
    }
}
