
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contensive.Addons.aoToolPanel {
    static class constants
    {
        //
        // -- 
        public const int Version = 1;
        //
        // -- 
        public const string guidToolPanelAccountForm = "{7A057715-5CDD-4D9F-A12A-5CCB61F93974}";
        //
        public const string cr = "\r\n\t";
        //
        // When true, displays the Login tab at the top of every page.
        //
        public const string spLoginTabAllowed = "toolPanelShowLoginTab";
        //
        // When checked, the state of the Login tab for new users is pinned (will not auto-hide when not in use and stay locked in the open position)
        //
        public const string spLoginTabIsPinnedByDefault = "toolPanelPinLoginTab";
        //
        // when true, the login tab is pinned open for this visit (prevents auto-hide)
        //
        public const string upLoginTabIsPinned = "tpLoginTabIsPinned";
    }
}
