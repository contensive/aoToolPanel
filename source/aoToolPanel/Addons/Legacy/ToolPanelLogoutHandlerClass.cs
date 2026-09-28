using System;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel.Addons.Legacy {
    /// <summary>
    /// Remote method addon that logs out the current user.
    /// </summary>
    public class ToolPanelLogoutHandlerClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                if (!cp.User.IsAuthenticated) { return ""; }
                cp.User.Logout();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "ToolPanelLogoutHandlerClass.Execute");
            }
            return "";
        }
    }
}
