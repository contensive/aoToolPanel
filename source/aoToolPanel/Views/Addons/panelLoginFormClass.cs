using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel {
    //
    //====================================================================================================
    //
    public class PanelLoginFormClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string s = cp.Addon.Execute(Constants.guidDefaultLoginForm);
            //
            s = cp.Html.div(s, "", "", "panelFormContainer");
            //
            return s;
        }
    }
}
