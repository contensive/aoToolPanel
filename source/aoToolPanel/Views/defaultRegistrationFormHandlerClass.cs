using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel
{
    //
    //====================================================================================================
    //
    public class defaultRegistrationFormHandlerClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        //====================================================================================================
        //
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
