
using Contensive.Addons.aoToolPanel;
using Contensive.BaseClasses;
using System;
using System.Diagnostics;
using static Contensive.Addons.aoToolPanel.Constants;

namespace Contensive.Addons {
    //
    //====================================================================================================
    /// <summary>
    /// Addon that is added after the body tag
    /// calls other classes for edit tool panel, the profile panel, 
    /// </summary>
    public class ToolPanelClass : Contensive.BaseClasses.AddonBaseClass {
        //
        public override object Execute(CPBaseClass cp) {
            var sw = Stopwatch.StartNew();
            try {
                //
                // -- if contensive version < 21.11.19.1 or siteProperty(allowToolPanel) is not true return
                bool isProperty = cp.Doc.IsProperty("allowToolPanel");
                if (isProperty) {
                    bool allowToolPanel = cp.Doc.GetBoolean("allowToolPanel");
                    if (!allowToolPanel) {
                        //
                        // -- allow-flag false or missing, if legacy c5 code, allow, else block
                        string[] versionSplit = cp.Version.Split('.');
                        if (versionSplit.Length < 4) { return ""; }
                        bool legacyVersion = (cp.Utils.EncodeInteger(versionSplit[0]) < 21) && (cp.Utils.EncodeInteger(versionSplit[1]) < 11) && (cp.Utils.EncodeInteger(versionSplit[2]) < 19) && (cp.Utils.EncodeInteger(versionSplit[2]) < 1);
                        if (!legacyVersion) { return ""; }
                        //
                        // -- legacy version, block admin site only
                        string adminRoute = cp.GetAppConfig().adminRoute;
                        string pathpage = cp.Request.PathPage;
                        if (pathpage.Equals("/" + adminRoute)) { return ""; }
                        if ((pathpage.Length > 10) && (pathpage.ToLowerInvariant().Substring(0, 11).Equals("/backoffice"))) { return ""; }
                    }
                }
                //
                // -- execute login form if not authenticated to execute authentication
                string loginForm = "";
                if(false) {
                    // -- block because this screws up password recovery process
                    if (!cp.User.IsAuthenticated) {
                        loginForm = cp.Addon.Execute(Constants.guidContensiveLoginForm);
                        if (string.IsNullOrEmpty(loginForm)) {
                            //
                            // -- authentication successful, redraw
                            cp.Response.Redirect(cp.Request.PathPage + "?" + cp.Request.QueryString);
                            return "";
                        }
                        // -- hack - change forget your password to recover your password
                        loginForm = loginForm.Replace("Forget Your Password", "Recover Your Password");
                    }
                }
                //
                const string layoutCacheName = "toolpanel/toolpanel.html";
                string layoutHtml = cp.Cache.GetText(layoutCacheName);
                if (string.IsNullOrEmpty(layoutHtml)) {
                    layoutHtml = cp.WwwFiles.Read(@"toolpanel\toolpanel.html");
                    cp.Cache.Store(layoutCacheName, layoutHtml);
                }

                layoutHtml = layoutHtml.Replace("{{loginForm}}", loginForm);
                using (CPBlockBaseClass layout = cp.BlockNew()) {
                    layout.Load(layoutHtml);
                    string swHints = "";
                    //
                    swHints += ", end layout load (" + sw.ElapsedMilliseconds.ToString() + ")";
                    //
                    if (cp.User.GetText(upLoginTabIsPinned, "") == "") {
                        //
                        swHints += ",loginconfig-1  (" + sw.ElapsedMilliseconds.ToString() + ")";
                        //
                        //
                        // if new user, set the default state of the login tab pin (pinned down or autohide)
                        //
                        bool isLoginTabDefaultPinned = cp.Site.GetBoolean(spLoginTabIsPinnedByDefault, false);
                        cp.User.SetProperty(upLoginTabIsPinned, isLoginTabDefaultPinned.ToString().ToLower(), 0);
                    }
                    //
                    swHints += ",loginconfig-2  (" + sw.ElapsedMilliseconds.ToString() + ")";
                    string jsOnReady = "";
                    //
                    //  set default for login tab
                    //
                    jsOnReady += cr + "isLockedAccountTab=" + cp.Site.GetBoolean("toolPanelDisableAutoHideAccountTab", false).ToString().ToLower() + ";";
                    //
                    swHints += ",loginconfig-3  (" + sw.ElapsedMilliseconds.ToString() + ")";
                    //
                    bool isAccountTabDefaultPinned = cp.Site.GetBoolean("toolPanelPinAccountTab", false);
                    if (cp.User.GetText("isLockedAccountTab") == "") {
                        cp.User.SetProperty("isLockedAccountTab", isAccountTabDefaultPinned.ToString().ToLower(), 0);
                    }
                    //
                    swHints += ",end loginconfig  (" + sw.ElapsedMilliseconds.ToString() + ")";
                    //
                    if (!cp.User.IsAuthenticated) {
                        //
                        // not logged in, return with just the login
                        //
                        layout.SetOuter("#tpAccountPanel", "");
                        layout.SetOuter("#tpAccountTab", "");
                        layout.SetOuter("#tpEditTab", "");
                        //
                        //  check if login tab allowed via settings page
                        //
                        if (!cp.Site.GetBoolean(spLoginTabAllowed, false)) {
                            layout.SetOuter("#tpLoginTab", "");
                        } else {
                            bool isLoginTabPinned = cp.User.GetBoolean(upLoginTabIsPinned, false);
                            if (isLoginTabPinned) {
                                jsOnReady += cr + "tpAbortLoginTabClose=true;";
                                jsOnReady += cr + "tpLoginTabIsPinned=true;";
                                jsOnReady += cr + "jQuery('#tpLoginLock').html('<img src=\"/toolPanel/lockClosed.png\" width=\"10\" height=\"10\">');";
                            } else {
                                jsOnReady += cr + "tpLoginTabIsPinned=false;";
                            }
                        }
                    } else {
                        //
                        // logged in, block the login
                        //
                        layout.SetOuter("#tpLoginPanel", "");
                        layout.SetOuter("#tpLoginTab", "");
                        //
                        swHints += ",end blockLogin  (" + sw.ElapsedMilliseconds.ToString() + ")";
                        string copy;
                        //
                        //  check if account tab allowed via settings page
                        //
                        if (cp.Site.GetBoolean("toolPanelShowAccountTab", false)) {
                            //
                            // Account Tab
                            //
                            bool isAccountTabLocked = cp.User.GetBoolean("isLockedAccountTab", false);
                            if (isAccountTabLocked) {
                                copy = "true";
                                jsOnReady += cr + "jQuery('#tpAccountLock').html('<img src=\"/toolPanel/lockClosed.png\" width=\"10\" height=\"10\">');";
                            } else {
                                copy = "false";
                            }
                            jsOnReady += cr + "isLockedAccountTab=" + copy + ";";
                        } else {
                            layout.SetOuter("#tpAccountTab", "");
                        }
                        //
                        swHints += ",end acounttab  (" + sw.ElapsedMilliseconds.ToString() + ")";
                        //
                        // Edit Tab
                        //
                        if (!cp.User.IsContentManager("")) {
                            layout.SetOuter("#tpEditTab", "");
                        } else {
                            //
                            // -- footer
                            string footer = ""
                                + cr + "<div class=\"tpInner\">"
                                + "<span>Contensive " + cp.Version + "</span>"
                                + "|<span>" + cp.Doc.StartTime + "</span>"
                                + "|<span><a href=\"http://support.Contensive.com/\" target=\"_blank\" class=\"ccAdminLink\">Support</a></span>"
                                + "|<span><a href=\"" + cp.Site.GetText("adminurl", "") + "\" class=\"ccAdminLink\">Admin Home</a></span>"
                                + "|<span><a href=\"/\" class=\"ccAdminLink\">Public Home</a></span>"
                                + "</div>"
                                + "";
                            footer = cp.Html.div(footer, "", "", "toolPanelFooter");
                            //
                            // -- common form open and clowe
                            string formOpen = ""
                                + "<form method=\"post\" action=\"?" + cp.Doc.RefreshQueryString + "\" style=\"display:inline;\">"
                                + "<input type=\"hidden\" name=\"ccFormSN\" value=\"1\">"
                                + "<input type=\"hidden\" name=\"1type\" value=\"do30a8vl29\">"
                                + "<input type=\"hidden\" name=\"1mb\" value=\"  Apply \">"
                                + "";
                            string formClose = "</form>";
                            //
                            swHints += ",end form  (" + sw.ElapsedMilliseconds.ToString() + ")";
                            //
                            string editHidden = "";
                            bool isDebugging = cp.Visit.GetBoolean("allowDebugging", false);
                            string debugHidden = "";
                            //
                            if (isDebugging) {
                                debugHidden = cr + "<input type=\"hidden\" name=\"1allowDebugging\" value=\"1\">";
                            }
                            {
                                //
                                // -- Edit
                                string buttonState = "1";
                                string buttonClass = "tpButtonUp";
                                if (cp.Visit.GetBoolean("allowEditing", false)) {
                                    buttonState = "0";
                                    buttonClass = "tpButtonDown";
                                }
                                string editUrl = cp.Doc.RefreshQueryString;
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "ccFormSN", "1");
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "1type", "do30a8vl29");
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "1mb", "  Apply ");
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "1allowEditing", buttonState);
                                
                                layout.SetInner("#tpEditTabContent", $"<a href=\"{cp.Request.PathPage}?{editUrl}\"  id=\"tpButtonContent\" data-toggle=\"tooltip\"  data-bs-toggle=\"tooltip\" data-placement=\"bottom\" data-bs-placement=\"bottom\" title=\"Edit content\" >Edit</a>");
                                jsOnReady += cr + "jQuery('#tpButtonContent').addClass('" + buttonClass + "');";
                            }
                            {
                                //
                                // -- widgets
                                string buttonState = "1";
                                string buttonClass = "tpButtonUp";
                                if (cp.Visit.GetBoolean("allowQuickEditor", false)) {
                                    buttonState = "0";
                                    buttonClass = "tpButtonDown";
                                }
                                string editUrl = cp.Doc.RefreshQueryString;
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "ccFormSN", "1");
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "1type", "do30a8vl29");
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "1mb", "  Apply ");
                                editUrl = cp.Utils.ModifyQueryString(editUrl, "1AllowQuickEditor", buttonState);

                                layout.SetInner("#tpEditTabBlocks", $"<a href=\"{cp.Request.PathPage}?{editUrl}\"  id=\"tpButtonWidget\" data-toggle=\"tooltip\"  data-bs-toggle=\"tooltip\" data-placement=\"bottom\" data-bs-placement=\"bottom\" title=\"Add/remove widgets\" >Widgets</a>");
                                jsOnReady += cr + "jQuery('#tpButtonWidget').addClass('" + buttonClass + "');";
                            }
                            {
                                //
                                // -- page
                                layout.SetInner("#tpEditTabPage", $"<a href=\"{cp.Site.GetText("adminUrl")}?af=4&aa=2&ad=1&cid={cp.Content.GetID("page content")}&id={cp.Doc.PageId}\"  data-toggle=\"tooltip\"  data-bs-toggle=\"tooltip\" data-placement=\"bottom\" data-bs-placement=\"bottom\" title=\"Edit page settings\" >Page</a>");
                            }
                            {
                                //
                                // -- Template
                                layout.SetInner("#tpEditTabTemplate", "<a href=\"" + cp.Site.GetText("adminUrl") + "?af=4&aa=2&ad=1&cid=" + cp.Content.GetID("page templates") + "&id=" + cp.Doc.TemplateId + "\"  data-toggle=\"tooltip\"  data-bs-toggle=\"tooltip\" data-placement=\"bottom\" data-bs-placement=\"bottom\" title=\"Edit the template used for this page\">Template</a>");
                            }
                            //
                            swHints += ",end editbutton  (" + sw.ElapsedMilliseconds.ToString() + ")";
                            //
                            // Advanced button
                            //
                            //{
                            //    string buttonState = "1";
                            //    string buttonClass = "tpButtonUp";
                            //    if (cp.Visit.GetBoolean("AllowAdvancedEditor", false)) {
                            //        buttonState = "0";
                            //        buttonClass = "tpButtonDown";
                            //        editHidden = "<input type=\"hidden\" name=\"1AllowAdvancedEditor\" value=\"1\">";
                            //    }
                            //    copy = ""
                            //        + debugHidden
                            //        + cr + "<input type=\"hidden\" name=\"1AllowAdvancedEditor\" value=\"" + buttonState + "\">"
                            //        + cr + "<a id=\"tpButtonAdvanced\" href=\"#\">Advanced</a>";
                            //    copy = ""
                            //        + cr + formOpen
                            //        + cp.Html.Indent(copy, 1)
                            //        + cr + formClose;
                            //    layout.SetInner("#tpEditTabAdv", copy);
                            //    jsOnReady += cr + "jQuery('#tpButtonAdvanced').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                            //    jsOnReady += cr + "jQuery('#tpButtonAdvanced').addClass('" + buttonClass + "');";
                            //    //
                            //    swHints += ",end advancedbutton (" + sw.ElapsedMilliseconds.ToString() + ")";
                            //}
                            //
                            // Debug Button
                            //
                            {
                                if (cp.User.IsDeveloper) {
                                    string buttonState = "1";
                                    string buttonClass = "tpButtonUp";
                                    if (isDebugging) {
                                        buttonState = "0";
                                        buttonClass = "tpButtonDown";
                                        //editHidden = "<input type=\"hidden\" name=\"1AllowAdvancedEditor\" value=\"1\">";
                                    }
                                    copy = ""
                                        + editHidden
                                        + cr + "<input type=\"hidden\" name=\"1allowDebugging\" value=\"" + buttonState + "\">"
                                        + cr + "<a id=\"tpButtonDebug\" href=\"#\" data-toggle=\"tooltip\"  data-bs-toggle=\"tooltip\" data-placement=\"bottom\" data-bs-placement=\"bottom\" title=\"Toggle debug mode to view html hints for debugging.\">Debug</a>";
                                    copy = ""
                                        + cr + formOpen
                                        + cp.Html.Indent(copy, 1)
                                        + cr + formClose;
                                    layout.SetInner("#tpEditTabDebug", copy);
                                    jsOnReady += cr + "jQuery('#tpButtonDebug').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                                    jsOnReady += cr + "jQuery('#tpButtonDebug').addClass('" + buttonClass + "');";
                                } else {
                                    layout.SetOuter("#tpEditTabDebug", "");
                                }
                            }
                            //
                            swHints += ",end debugbutton  (" + sw.ElapsedMilliseconds.ToString() + ")";
                            //
                            // Admin button
                            //
                            copy = "<a href=\"" + cp.Site.GetText("adminUrl", "/admin") + "\"  data-toggle=\"tooltip\"  data-bs-toggle=\"tooltip\" data-placement=\"bottom\" data-bs-placement=\"bottom\" title=\"Link to the admin site.\">Admin</a>";
                            layout.SetInner("#tpEditTabAdmin", copy);
                        }
                    }
                    string doc = ""
                        + cp.Html.div("<!-- start -->" + layout.GetHtml() + "<!-- end -->", "", "", "toolPanel")
                        + cp.Html.div(cp.Doc.Body, "", "", "toolPanelContent")
                        + "";
                    cp.Doc.Body = doc;
                    string manualJs = ""
                        + "function bindReadyState(callback){"
                        + " if (document.readyState != 'loading') callback();"
                        + " else if (document.addEventListener) document.addEventListener('DOMContentLoaded', callback);"
                        + " else document.attachEvent('onreadystatechange', function(){ if (document.readyState == 'complete') callback(); });"
                        + "}"
                        + " bindReadyState(function(){" + jsOnReady + "});"
                        + "";
                    string manualStyles = "#toolPanel #tpDraggable {left:" + cp.Visit.GetInteger("toolPanelPositionLeft") + "px;display:block}";
                    cp.Doc.AddHeadJavascript(manualJs);
                    cp.Doc.AddHeadStyle(manualStyles);
                    //
                    swHints += ",exit  (" + sw.ElapsedMilliseconds.ToString() + ")";
                    //
                    cp.Site.TestPoint("ToolPanel Hints [" + swHints + "]");
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return "";
        }
    }
}
