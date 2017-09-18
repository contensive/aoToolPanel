using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel
{
    public class panelAccountFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = cp.Utils.ExecuteAddon("{7A057715-5CDD-4D9F-A12A-5CCB61F93974}");
            //
            s = cp.Html.div(s, "", "", "panelFormContainer");
            //
            return s;
        }
    }
    //
    public class defaultAccountFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = "";
            string js = "";
            bool updated = cp.Utils.EncodeBoolean(cp.Doc.get_Var("acctUpdated"));
            bool errFlag = cp.Utils.EncodeBoolean(cp.Doc.get_Var("errFlag"));
            bool allowEmailLogin = cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWEMAILLOGIN","0"));
            CPBlockBaseClass lO = cp.BlockNew();
            //
            loadForm(cp);
            //
            lO.OpenLayout("Default Account Form");
            //
            if (updated||errFlag)
            {
                if (errFlag)
                {
                    lO.SetInner(".panelInstructionContainer", cp.Html.p(cp.Doc.get_Var("errMessage"),"","ccError",""));
                }
                else
                {
                    lO.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Profile Form Updated", "Your profile has been updated."));
                }
            }
            else
            {
                lO.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Account Form Instructions", "Use the form below to update your account information."));
            }
            //
            if (allowEmailLogin)
            {
                lO.SetOuter("#panelRowUsername", "");
            }
            s = lO.GetHtml();
            //
            js += "$(document).ready(function(){";
            js += " $('#panelAccountFirstName').val('" + cp.Doc.get_Var("panelAccountFirstName") + "');";
            js += " $('#panelAccountLastName').val('" + cp.Doc.get_Var("panelAccountLastName") + "');";
            js += " $('#panelAccountEmail').val('" + cp.Doc.get_Var("panelAccountEmail") + "');";
            js += " $('#panelAccountUsername').val('" + cp.Doc.get_Var("panelAccountUsername") + "');";
            js += " $('#panelAccountPassword').val('" + cp.Doc.get_Var("panelAccountPassword") + "');";
            js += "});";
            //
            cp.Doc.AddHeadJavascript(js);
            //
            return s;
        }
        //
        //
        //
        void loadForm(Contensive.BaseClasses.CPBaseClass cp)
        {
            CPCSBaseClass cs = cp.CSNew();
            //
            if (cs.Open("People", "ID=" + cp.User.Id, "", false, "", 1, 1))
            {
                cp.Doc.set_Var("panelAccountFirstName", cs.GetText("firstName"));
                cp.Doc.set_Var("panelAccountLastName", cs.GetText("lastName"));
                cp.Doc.set_Var("panelAccountEmail", cs.GetText("email"));
                cp.Doc.set_Var("panelAccountUsername", cs.GetText("username"));
                cp.Doc.set_Var("panelAccountPassword", cs.GetText("password"));
            }
            cs.Close();
        }
    }
    //
    public class defaultAccountFormHandlerClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            bool errFlag = false;
            string s = "";
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
            if ((firstName == "") | (lastName == "") | (email == "") | (password == "") | ((username == "") & (!allowEmailLogin)))
            {
                //
                // missing fields
                //
                errFlag = true;
                cp.Doc.set_Var("errFlag", "1");
                cp.Doc.set_Var("errMessage", "One or more required fields were empty.");
            }
            else
            {
                //
                //  check for duplicate in username if account requires username
                //
                if (allowEmailLogin)
                {
                    //
                    // email loging, check email provided against email and username fields
                    //
                    sql = "(ID<>" + cp.User.Id + ")"
                        + "and("
                            + "(username=" + cp.Db.EncodeSQLText(email) + ")"
                            + "or(email=" + cp.Db.EncodeSQLText(email) + ")"
                        + ")"
                        + "";
                }
                else
                {
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
                if (cs.Open("People", sql, "", false, "", 1, 1))
                {
                    errFlag = true;
                    cp.Doc.set_Var("errFlag", "1");
                    cp.Doc.set_Var("errMessage", "The login requested is not available, please enter an alternate username.");
                }
                cs.Close();
            }
            //
            if (!errFlag)
            {
                if (cs.Open("People", "ID=" + cp.User.Id, "", false, "", 1, 1))
                {
                    cs.SetField("name", firstName + " " + lastName);
                    cs.SetField("firstName", firstName);
                    cs.SetField("lastName", lastName);
                    cs.SetField("email", email);
                    cs.SetField("password", password);
                    if (!allowEmailLogin)
                    {
                        cs.SetField("username", username);
                    }
                }
                cs.Close();
                cp.Doc.set_Var("acctUpdated", "1");
            }
            //
            s = cp.Utils.ExecuteAddon("{7A057715-5CDD-4D9F-A12A-5CCB61F93974}");
            //
            return s;
        }
    }
}
