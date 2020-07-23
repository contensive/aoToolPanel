using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel
{
    //
    //====================================================================================================
    //
    public class toolPanelAccountFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string result = "";
            try {
                bool updated = cp.Doc.GetBoolean("acctUpdated");
                bool errFlag = cp.Doc.GetBoolean("errFlag");
                bool allowEmailLogin = cp.Doc.GetBoolean("ALLOWEMAILLOGIN", false);
                var person = Models.Db.personModel.create(cp, cp.User.Id);
                if (person == null) {
                    cp.Site.ErrorReport("Person failed to open from user.id");
                    result = "<p>There was a problem locating your user record. Please refresh this page and try again.</p>";
                } else {
                    CPBlockBaseClass layout = cp.BlockNew();
                    //dim layoutHtml as string = cp.Content.getLayout("Tool Panel Account Form",)
                    layout.OpenFile("toolpanel/accountform.html");
                    //
                    if (updated || errFlag) {
                        if (errFlag) {
                            layout.SetInner(".panelInstructionContainer", cp.Html.p(cp.Doc.GetText("errMessage"), "", "ccError", ""));
                        } else {
                            layout.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Profile Form Updated", "Your profile has been updated."));
                        }
                    } else {
                        layout.SetInner(".panelInstructionContainer", cp.Content.GetCopy("Default Account Form Instructions", "Use the form below to update your account information."));
                    }
                    if (allowEmailLogin) {
                        layout.SetOuter("#panelRowUsername", "");
                    }
                    layout.SetOuter("#panelAccountFirstName", layout.GetOuter("#panelAccountFirstName").Replace(">", " value=\"" + person.FirstName + "\">"));
                    layout.SetOuter("#panelAccountLastName", layout.GetOuter("#panelAccountLastName").Replace(">", " value=\"" + person.LastName + "\">"));
                    layout.SetOuter("#panelAccountEmail", layout.GetOuter("#panelAccountEmail").Replace(">", " value=\"" + person.Email + "\">"));
                    layout.SetOuter("#panelAccountUsername", layout.GetOuter("#panelAccountUsername").Replace(">", " value=\"" + person.Username + "\">"));
                    layout.SetOuter("#panelAccountPassword", layout.GetOuter("#panelAccountPassword").Replace(">", " value=\"" + person.Password + "\">"));
                    result = layout.GetHtml();
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
    }

}
