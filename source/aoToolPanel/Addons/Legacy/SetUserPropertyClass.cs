using System;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel.Addons.Legacy {
    /// <summary>
    /// Remote method addon that sets user properties for tab lock states (account, login, edit).
    /// </summary>
    public class SetUserPropertyClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                if (!cp.User.IsAuthenticated) { return ""; }
                string name = cp.Doc.GetText("n");
                string value = cp.Doc.GetText("v");
                if (name == "isLockedAccountTab") {
                    cp.User.SetProperty("isLockedAccountTab", value);
                }
                if (name == "tpLoginTabIsPinned") {
                    cp.User.SetProperty("tpLoginTabIsPinned", value);
                }
                if (name == "isLockedEditTab") {
                    cp.User.SetProperty("isLockedEditTab", value);
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "SetUserPropertyClass.Execute");
            }
            return "";
        }
    }
}
