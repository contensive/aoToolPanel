using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel
{
    public class panelRegistrationFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = cp.Utils.ExecuteAddon("{E31F7A5B-FE69-4CF5-BD16-7F368192D956}");
            //
            s = cp.Html.div(s, "", "", "panelFormContainer");
            //
            return s;
        }
    }
    //
    public class defaultRegistrationFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = "";
            string sS = "";
            bool updated = cp.Utils.EncodeBoolean(cp.Doc.get_Var("acctUpdated"));
            bool errFlag = cp.Utils.EncodeBoolean(cp.Doc.get_Var("errFlag"));
            CPBlockBaseClass lO = cp.BlockNew();
            string hiddenString = "";
            //
            //loadForm(cp);
            //
            lO.OpenLayout("Default Registration Form");
            //
            if (updated || errFlag)
            {
                if (errFlag)
                {
                    lO.SetInner(".panelInstructionContainer", cp.Html.p(cp.Doc.get_Var("errMessage"), "", "ccError", ""));
                }
                else
                {
                    lO.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Profile Form Updated", "Your profile has been updated."));
                }
            }
            else
            {
                lO.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Account Registration Instructions", "To register, complete and submit this form."));
            }
            //
            //  ALLOWEMAILLOGIN check
            //
            if (cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWEMAILLOGIN", "")))
            {
                lO.SetOuter("#panelRowUsername", "");
                hiddenString += cp.Html.Hidden("reqUsername", "0", "", "reqUsername");
            }
            else
            {
                hiddenString += cp.Html.Hidden("reqUsername", "1", "", "reqUsername");
            }
            //
            //
            //  ALLOWNOPASSWORDLOGIN check
            //
            if (cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWNOPASSWORDLOGIN", "")))
            {
                lO.SetInner(".passwordRequired", "");
                hiddenString += cp.Html.Hidden("reqPassword", "0", "", "reqPassword");
            }
            else
            {
                hiddenString += cp.Html.Hidden("reqPassword", "1", "", "reqPassword");
            }
            //
            //  make a list of fields present so field validator can alert required fields if missing
            //
            lO.SetInner("#hiddenRow", hiddenString);
            //
            s = lO.GetHtml();
            //
            sS += "$(document).ready(function(){";
            //
            sS += " var containerHeight = $('#panelFormContainer').height();";
            //
            sS += " $('#panelRegistrationFirstName').val('" + cp.Doc.get_Var("panelRegistrationFirstName") + "');";
            sS += " $('#panelRegistrationLastName').val('" + cp.Doc.get_Var("panelRegistrationLastName") + "');";
            sS += " $('#panelRegistrationEmail').val('" + cp.Doc.get_Var("panelRegistrationEmail") + "');";
            sS += " $('#panelRegistrationUsername').val('" + cp.Doc.get_Var("panelRegistrationUsername") + "');";
            sS += " $('#panelRegistrationPassword').val('" + cp.Doc.get_Var("panelRegistrationPassword") + "');";
            //
            sS += " $('#accountSubmit').click(function(){";
            sS += "     if (!validateRegistrationForm())";
            sS += "     {";
            sS += "         return false;";
            sS += "     };";
            //
            sS += "     var varString;";
            //
            sS += "	    var firstName = $('#panelRegistrationFirstName').val();";
            sS += "	    var lastName = $('#panelRegistrationLastName').val();";
            sS += "	    var email = $('#panelRegistrationEmail').val();";
            sS += "	    var username = $('#panelRegistrationUsername').val();";
            sS += "	    var password = $('#panelRegistrationPassword').val();";
            //
            sS += "	    varString = 'panelRegistrationFirstName='+firstName;";
            sS += "	    varString += '&panelRegistrationLastName='+lastName;";
            sS += "	    varString += '&panelRegistrationEmail='+email;";
            sS += "	    varString += '&panelRegistrationUsername='+username;";
            sS += "	    varString += '&panelRegistrationPassword='+password;";
            //
            sS += "     cj.ajax.addonCallback('toolpanelRegistrationFormHandler', varString, tpRedirectHome, 'panelFormContainer');";
            sS += "     tpSetSpinner('panelFormContainer', 'Updating Account....', containerHeight);";
            sS += " });";
            //
            sS += " $('#logoutClick').click(function(){";
            sS += "     cj.ajax.addonCallback('toolPanelLogoutHandler', '', tpRedirectHome, 'panelFormContainer');";
            sS += "     tpSetSpinner('panelFormContainer', 'Logging Out....', containerHeight);";
            sS += " });";
            //
            sS += " return false;";
            sS += "});";
            //
            cp.Doc.AddHeadJavascript(sS);
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
                cp.Doc.set_Var("panelRegistrationFirstName", cs.GetText("firstName"));
                cp.Doc.set_Var("panelRegistrationLastName", cs.GetText("lastName"));
                cp.Doc.set_Var("panelRegistrationEmail", cs.GetText("email"));
                cp.Doc.set_Var("panelRegistrationUsername", cs.GetText("username"));
                cp.Doc.set_Var("panelRegistrationPassword", cs.GetText("password"));
            }
            cs.Close();
        }
    }
    //
    public class defaultRegistrationFormHandlerClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            bool errFlag = false;
            string s = "";
            string firstName = cp.Doc.GetText("panelRegistrationFirstName");
            string lastName = cp.Doc.GetText("panelRegistrationLastName");
            string email = cp.Doc.GetText("panelRegistrationEmail");
            string username = cp.Doc.GetText("panelRegistrationUsername");
            string password = cp.Doc.GetText("panelRegistrationPassword");
            bool usernameValid = !cp.Site.GetBoolean("ALLOWEMAILLOGIN", "");
            CPCSBaseClass cs = cp.CSNew();
            //
            //  check for duplicate in username if account requires username
            //
            if (usernameValid)
            {
                if (cs.Open("People", "(ID<>" + cp.User.Id + ") and (username=" + cp.Db.EncodeSQLText(username) + ")", "", false, "", 1, 1))
                {
                    errFlag = true;
                    cp.Doc.set_Var("errFlag", "1");
                    cp.Doc.set_Var("errMessage", "The username requested is not available, please enter an alternate username.");
                }
                cs.Close();
            }
            if (!errFlag)
            {
                if (cs.Open("People", "(ID<>" + cp.User.Id + ") and (email=" + cp.Db.EncodeSQLText(email) + ")", "", false, "", 1, 1))
                {
                    errFlag = true;
                    cp.Doc.set_Var("errFlag", "1");
                    cp.Doc.set_Var("errMessage", "The email entered is already registered. Please verify you have not already registered or enter an alternate email address.");
                }
                cs.Close();
            }
            //
            if (!errFlag)
            {
                if ((cp.User.IsRecognized )&&( !cp.User.IsAuthenticated ))
                {
                    cp.User.Logout();
                }
                if (cs.Open("People", "ID=" + cp.User.Id, "", false, "", 1, 1))
                {
                    cs.SetField("name", firstName + " " + lastName);
                    cs.SetField("firstName", firstName);
                    cs.SetField("lastName", lastName);
                    cs.SetField("email", email);
                    cs.SetField("password", password);
                    if (usernameValid)
                    {
                        cs.SetField("username", username);
                    }
                }
                cs.Close();
                //
                //  authenticate the user
                //
                cp.User.LoginByID(cp.User.Id.ToString() );
                //
                s = "";
            }
            else
            {
                s = cp.Utils.ExecuteAddon("{E31F7A5B-FE69-4CF5-BD16-7F368192D956}");
            }
            //
            return s;
        }
    }
}
