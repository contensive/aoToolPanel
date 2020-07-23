using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel
{
    //
    //====================================================================================================
    //
    public class defaultRegistrationFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = "";
            string sS = "";
            bool updated = cp.Utils.EncodeBoolean(cp.Doc.GetText("acctUpdated"));
            bool errFlag = cp.Utils.EncodeBoolean(cp.Doc.GetText("errFlag"));
            CPBlockBaseClass layout = cp.BlockNew();
            string hiddenString = "";
            //
            //loadForm(cp);
            //
            layout.OpenFile(@"toolspanel\registrationform.html");
            //
            if (updated || errFlag)
            {
                if (errFlag)
                {
                    layout.SetInner(".panelInstructionContainer", cp.Html.p(cp.Doc.GetText("errMessage"), "", "ccError", ""));
                }
                else
                {
                    layout.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Profile Form Updated", "Your profile has been updated."));
                }
            }
            else
            {
                layout.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Account Registration Instructions", "To register, complete and submit this form."));
            }
            //
            //  ALLOWEMAILLOGIN check
            //
            if (cp.Site.GetBoolean("ALLOWEMAILLOGIN", false))
            {
                layout.SetOuter("#panelRowUsername", "");
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
            if (cp.Site.GetBoolean("ALLOWNOPASSWORDLOGIN", false))
            {
                layout.SetInner(".passwordRequired", "");
                hiddenString += cp.Html.Hidden("reqPassword", "0", "", "reqPassword");
            }
            else
            {
                hiddenString += cp.Html.Hidden("reqPassword", "1", "", "reqPassword");
            }
            //
            //  make a list of fields present so field validator can alert required fields if missing
            //
            layout.SetInner("#hiddenRow", hiddenString);
            //
            s = layout.GetHtml();
            //
            sS += "$(document).ready(function(){";
            //
            sS += " var containerHeight = $('#panelFormContainer').height();";
            //
            sS += " $('#panelRegistrationFirstName').val('" + cp.Doc.GetText("panelRegistrationFirstName") + "');";
            sS += " $('#panelRegistrationLastName').val('" + cp.Doc.GetText("panelRegistrationLastName") + "');";
            sS += " $('#panelRegistrationEmail').val('" + cp.Doc.GetText("panelRegistrationEmail") + "');";
            sS += " $('#panelRegistrationUsername').val('" + cp.Doc.GetText("panelRegistrationUsername") + "');";
            sS += " $('#panelRegistrationPassword').val('" + cp.Doc.GetText("panelRegistrationPassword") + "');";
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
        //====================================================================================================
        //
        void loadForm(Contensive.BaseClasses.CPBaseClass cp)
        {
            CPCSBaseClass cs = cp.CSNew();
            //
            if (cs.Open("People", "ID=" + cp.User.Id, "", false, "", 1, 1))
            {
                cp.Doc.SetProperty("panelRegistrationFirstName", cs.GetText("firstName"));
                cp.Doc.SetProperty("panelRegistrationLastName", cs.GetText("lastName"));
                cp.Doc.SetProperty("panelRegistrationEmail", cs.GetText("email"));
                cp.Doc.SetProperty("panelRegistrationUsername", cs.GetText("username"));
                cp.Doc.SetProperty("panelRegistrationPassword", cs.GetText("password"));
            }
            cs.Close();
        }
    }
}
