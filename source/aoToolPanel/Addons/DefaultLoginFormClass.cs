
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel {
    //
    //====================================================================================================
    //
    public class DefaultLoginFormClass : AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(CPBaseClass cp)
        {
            string s = "";
            bool errFlag = cp.Utils.EncodeBoolean(cp.Doc.GetText("Error Flag"));    //   problem with login
            bool emailFlag = cp.Utils.EncodeBoolean(cp.Doc.GetText("Email Flag"));  //  email password sent
            bool errEmailFlag = cp.Utils.EncodeBoolean(cp.Doc.GetText("Email Error Flag")); //  problem with email password
            CPBlockBaseClass layout = cp.BlockNew();
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
                layout.SetOuter(".panelInstructionContainer", "");
            }
            //
            //  ALLOWEMAILLOGIN check
            //
            if (cp.Site.GetBoolean("ALLOWEMAILLOGIN", false))
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
            if (!cp.Site.GetBoolean("ALLOWMEMBERJOIN", false))
            {
                string loginLink = layout.GetOuter("#tpLoginFormSubmit");
                layout.SetInner(".panelLoginFormList .buttonRow", loginLink);
            }
            //
            //  ALLOWNOPASSWORDLOGIN check
            //
            if (cp.Site.GetBoolean("ALLOWNOPASSWORDLOGIN", false))
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
            if (!cp.Site.GetBoolean("ALLOWAUTOLOGIN", false))
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
                layout.SetInner(".panelLowerInstructionContainer", cp.Content.GetCopy("Default Email Form Email Sent", "<p class=\"ccError\">Your login has been sent to the email address provided.</p>"));
            }
            else
            {
                layout.SetOuter(".panelLowerInstructionContainer", "");
            }
            //
            //  ALLOWPASSWORDEMAIL check
            //
            if (! cp.Site.GetBoolean("ALLOWPASSWORDEMAIL", false))
            {
                layout.SetOuter("#emailSendContainer", "");
            }
            //
            //  make a list of fields present so field validator can alert required fields if missing
            //
            layout.SetOuter("#hiddenRow", hiddenString);
            //
            s = layout.GetHtml();
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
                cp.Doc.SetProperty("panelAccountFirstName", cs.GetText("firstName"));
                cp.Doc.SetProperty("panelAccountLastName", cs.GetText("lastName"));
                cp.Doc.SetProperty("panelAccountEmail", cs.GetText("email"));
            }
            cs.Close();
        }
    }
}
