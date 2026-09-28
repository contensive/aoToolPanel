
using System;

namespace Contensive.Addons.aoToolPanel {
    //
    //====================================================================================================
    //
    public class PanelLoginFormClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            try {
                string s = cp.Addon.Execute(Constants.guidContensiveLoginForm);
                //
                s = cp.Html.div(s, "", "", "panelFormContainer");
                //
                return s;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "PanelLoginFormClass.Execute");
            }
            return "";
        }
    }
}
