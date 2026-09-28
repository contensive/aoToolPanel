
using System;

namespace Contensive.Addons.aoToolPanel {
    //
    //====================================================================================================
    //
    public class PanelRegistrationFormClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            try {
                string s = cp.Addon.Execute("{E31F7A5B-FE69-4CF5-BD16-7F368192D956}");
                //
                s = cp.Html.div(s, "", "", "panelFormContainer");
                //
                return s;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "PanelRegistrationFormClass.Execute");
            }
            return "";
        }
    }
}
