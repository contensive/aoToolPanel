using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel
{
    //
    //====================================================================================================
    //
    public class defaultLoginFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = "";
            //string js = "";
            bool errFlag = cp.Utils.EncodeBoolean(cp.Doc.get_Var("Error Flag"));    //   problem with login
            bool emailFlag = cp.Utils.EncodeBoolean(cp.Doc.get_Var("Email Flag"));  //  email password sent
            bool errEmailFlag = cp.Utils.EncodeBoolean(cp.Doc.get_Var("Email Error Flag")); //  problem with email password
            CPBlockBaseClass layout = cp.BlockNew();
            //string defaultLoginInstructions = "You are attempting to enter an access controlled area. Continue only if you have authority to enter this area. Information about your visit will be recorded for security purposes.";
            //string defaultEmailInstructions = "If you are a member of the system and can not remember your password, enter your email address below and we will email your matching username and password.";
            string hiddenString = "";
            //
            layout.OpenFile(@"toolpanel\loginform.html");
            //
            //  Login Instructions
            //
            if (errFlag)
            {
                layout.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Login Form Error", "<p class=\"ccError\">There was a problem with your login - please try again.</p>"));
            }
            else
            {
                //lO.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Login Form Instructions", defaultLoginInstructions));
            }
            //
            //  ALLOWEMAILLOGIN check
            //
            if (cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWEMAILLOGIN", "")))
            {
                layout.SetOuter("#panelRowUsername", "");
                hiddenString += cp.Html.Hidden("reqEmail", "1", "", "reqEmail");
                hiddenString += cp.Html.Hidden("reqUsername", "0", "", "reqUsername");
            }
            else
            {
                layout.SetOuter("#panelRowEmail", "");
                hiddenString += cp.Html.Hidden("reqUsername", "1", "", "reqUsername");
                hiddenString += cp.Html.Hidden("reqEmail", "0", "", "reqEmail");
            }
            //
            // ALLOWMEMBERJOIN (include the register button)
            //
            if (!cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWMEMBERJOIN", "0")))
            {
                string loginLink = layout.GetOuter("#tpLoginFormSubmit");
                layout.SetInner(".panelLoginFormList .buttonRow", loginLink);
            }
            //
            //  ALLOWNOPASSWORDLOGIN check
            //
            if (cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWNOPASSWORDLOGIN", "")))
            {
                layout.SetInner("#tpPasswordLabel", "Password"); //optional
                hiddenString += cp.Html.Hidden("reqPassword", "0", "", "reqPassword");
            }
            else
            {
                hiddenString += cp.Html.Hidden("reqPassword", "1", "", "reqPassword");
            }
            //
            //  ALLOWAUTOLOGIN check
            //
            if (!cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWAUTOLOGIN", "")))
            {
                layout.SetOuter("#panelRowAuto", "");
            }
            //
            //  add email form instrutions
            //
            if (errEmailFlag)
            {
                layout.SetInner(".panelLowerInstructionContainer", cp.Content.GetCopy("Default Email Form Error", "<p class=\"ccError\">The email address provided was not found.</p>"));
            }
            else if (emailFlag)
            {
                layout.SetInner(".panelLowerInstructionContainer", cp.Content.GetCopy("Default Email Form Email Sent", "<p class=\"ccError\">Your login has been sent the email address provided.</p>"));
            }
            else
            {
                //lO.SetInner(".panelLowerInstructionContainer", cp.Content.GetCopy("Default Email Form Instructions", defaultEmailInstructions));
            }
            //
            //  ALLOWPASSWORDEMAIL check
            //
            if (! cp.Utils.EncodeBoolean(cp.Site.GetProperty("ALLOWPASSWORDEMAIL", "")))
            {
                layout.SetOuter("#emailSendContainer", "");
            }
            //
            //  make a list of fields present so field validator can alert required fields if missing
            //
            layout.SetOuter("#hiddenRow", hiddenString);
            //
            s = layout.GetHtml();
            ////
            //js += "$(document).ready(function(){";
            ////
            //js += "     var containerHeight = $('#panelFormContainer').height();";
            ////
            //js += " $('#loginSubmit').click(function(){";
            //js += "     if (!validateLoginForm())";
            //js += "     {";
            //js += "         return false;";
            //js += "     };";
            ////
            //js += "	    var varString;";
            //js += "     var username = $('#panelLoginUsername').val();";
            //js += "     var email = $('#panelLoginEmail').val();";
            //js += "     var password = $('#panelLoginPassword').val();";
            //js += "     var auto = $('#panelLoginAuto').val();";
            ////
            //js += "	    varString = 'panelLoginUsername='+username;";
            //js += "	    varString += '&panelLoginPassword='+password;";
            //js += "	    varString += '&panelLoginEmail='+email;";
            //js += "	    varString += '&panelLoginAuto='+auto;";
            ////
            //js += "     cj.ajax.addonCallback('toolPanelLoginFormHandler', varString, tpRedirectHome, 'panelFormContainer');";
            ////
            //js += "     tpSetSpinner('panelFormContainer', 'Authenticating Account....', containerHeight);";
            //js += " });";

            //js += " $('#emailSubmit').click(function(){";
            //js += "     if (!validateEmailForm())";
            //js += "     {";
            //js += "         return false;";
            //js += "     };";
            //js += "     cj.ajax.addon('toolPanelLoginFormHandler', '', 'panelLoginForm', 'panelFormContainer', '', '');";
            //js += "     tpSetSpinner('panelFormContainer', 'Gathering Account Information....', containerHeight);";
            //js += " });";

            //js += " $('#registrationClick').click(function(){";
            //js += "     cj.ajax.addon('toolPanelDefaultRegistrationForm','', '', 'panelFormContainer', '', '');";
            //js += "     tpSetSpinner('panelFormContainer', 'Building Registration Form....', containerHeight);";
            //js += " });";
            ////
            //js += " return false;";
            //js += "});";
            ////
            //cp.Doc.AddHeadJavascript(js);
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
                cp.Doc.set_Var("panelAccountFirstName", cs.GetText("firstName"));
                cp.Doc.set_Var("panelAccountLastName", cs.GetText("lastName"));
                cp.Doc.set_Var("panelAccountEmail", cs.GetText("email"));
            }
            cs.Close();
        }
    }
}
