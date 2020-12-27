
namespace Contensive.Addons.aoToolPanel {
    //
    //====================================================================================================
    //
    public class PanelLoginFormClass : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp) {
            string s = cp.Addon.Execute(Constants.guidContensiveLoginForm);
            //string s = cp.Addon.Execute(Constants.guidLoginForm);
            //
            s = cp.Html.div(s, "", "", "panelFormContainer");
            //
            return s;
        }
    }
}
