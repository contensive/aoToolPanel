using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel
{
    //
    //====================================================================================================
    //
    public class panelLoginFormClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string s = cp.Utils.ExecuteAddon("{37B7EDB3-7BE0-4E89-8012-16F0CDEED218}");
            //
            s = cp.Html.div(s, "", "", "panelFormContainer");
            //
            return s;
        }
    }
}
