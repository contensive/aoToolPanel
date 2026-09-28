using System;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel.Addons.Legacy {
    /// <summary>
    /// Remote method addon that saves the tool panel draggable position to visit properties.
    /// </summary>
    public class RemoteToolPanelPositionClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                if (!cp.User.IsAuthenticated) { return ""; }
                string top = cp.Doc.GetText("top");
                string left = cp.Doc.GetText("left");
                cp.Visit.SetProperty("toolPanelPositionTop", top);
                cp.Visit.SetProperty("toolPanelPositionLeft", left);
                return $"top={top}, left={left}";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "RemoteToolPanelPositionClass.Execute");
            }
            return "";
        }
    }
}
