using System;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel.Addons.Legacy {
    /// <summary>
    /// ProcessRunOnce addon that disables the legacy tools panel on install.
    /// </summary>
    public class ToolPanelOnInstallClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                cp.Site.SetProperty("allowLegacyToolsPanel", "0");
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "ToolPanelOnInstallClass.Execute");
            }
            return "";
        }
    }
}
