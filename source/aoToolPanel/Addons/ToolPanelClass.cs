
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
                string swHints = "";
                string jsOnReady = "";
                string doc;
                string copy;
                bool isDebugging = false;
                CPBlockBaseClass layout = cp.BlockNew();
                string editHidden = "";
                string footer = "";
                string formOpen = "";
                string formClose = "";
                string debugHidden = "";
                string buttonState = "";
                string buttonClass = "";
                string previewHidden = "";
                bool isAccountTabLocked = false;
                bool isLoginTabPinned = false;
                bool isLoginTabDefaultPinned = false;
                bool isAccountTabDefaultPinned = false;
                //
                const string layoutCacheName = "toolpanel/toolpanel.html";
                string layoutHtml = cp.Cache.GetText(layoutCacheName);
                if (string.IsNullOrEmpty(layoutHtml)) {
                    layoutHtml = cp.WwwFiles.Read( @"toolpanel\toolpanel.html");
                    cp.Cache.Store(layoutCacheName, layoutHtml);
                }
                //
                // -- add login form
                layoutHtml = layoutHtml.Replace("{{loginForm}}", cp.Addon.Execute(Constants.guidContensiveLoginForm));
                layout.Load(layoutHtml);
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
                    isLoginTabDefaultPinned = cp.Site.GetBoolean(spLoginTabIsPinnedByDefault, false);
                    cp.User.SetProperty(upLoginTabIsPinned, isLoginTabDefaultPinned.ToString().ToLower(), 0);
                }
                //
                swHints += ",loginconfig-2  (" + sw.ElapsedMilliseconds.ToString() + ")";
                //
                //  set default for login tab
                //
                jsOnReady += cr + "isLockedAccountTab=" + cp.Site.GetBoolean("toolPanelDisableAutoHideAccountTab", false).ToString().ToLower() + ";";
                //
                swHints += ",loginconfig-3  (" + sw.ElapsedMilliseconds.ToString() + ")";
                //
                isAccountTabDefaultPinned = cp.Site.GetBoolean("toolPanelPinAccountTab", false);
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
                        isLoginTabPinned = cp.User.GetBoolean(upLoginTabIsPinned, false);
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
                    //
                    //  check if account tab allowed via settings page
                    //
                    if (cp.Site.GetBoolean("toolPanelShowAccountTab", false)) {
                        //
                        // Account Tab
                        //
                        isAccountTabLocked = cp.User.GetBoolean("isLockedAccountTab", false);
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
                        footer = ""
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
                        formOpen = ""
                            + "<form method=\"post\" action=\"?" + cp.Doc.RefreshQueryString + "\" style=\"display:inline;\">"
                            + "<input type=\"hidden\" name=\"ccFormSN\" value=\"1\">"
                            + "<input type=\"hidden\" name=\"1type\" value=\"do30a8vl29\">"
                            + "<input type=\"hidden\" name=\"1mb\" value=\"  Apply \">"
                            + "";
                        formClose = "</form>";
                        //
                        swHints += ",end form  (" + sw.ElapsedMilliseconds.ToString() + ")";
                        //
                        editHidden = "";
                        isDebugging = cp.Visit.GetBoolean("allowDebugging", false);
                        //
                        if (isDebugging) {
                            debugHidden = cr + "<input type=\"hidden\" name=\"1allowDebugging\" value=\"1\">";
                        }
                        //
                        // Edit button
                        //
                        buttonState = "1";
                        buttonClass = "tpButtonUp";
                        if (cp.Visit.GetBoolean("allowEditing", false)) {
                            buttonState = "0";
                            buttonClass = "tpButtonDown";
                            editHidden = "<input type=\"hidden\" name=\"1allowEditing\" value=\"1\">";
                        }
                        copy = ""
                            + debugHidden
                            + previewHidden
                            + cr + "<input type=\"hidden\" name=\"1allowEditing\" value=\"" + buttonState + "\">"
                            + cr + "<a id=\"tpButtonEdit\" href=\"#\">Edit</a>";
                        copy = ""
                            + cr + formOpen
                            + cp.Html.Indent(copy, 1)
                            + cr + formClose;
                        layout.SetInner("#tpEditTabEdit", copy);
                        jsOnReady += cr + "jQuery('#tpButtonEdit').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                        jsOnReady += cr + "jQuery('#tpButtonEdit').addClass('" + buttonClass + "');";
                        //
                        swHints += ",end editbutton  (" + sw.ElapsedMilliseconds.ToString() + ")";
                        //
                        // Quick Edit button
                        //
                        buttonState = "1";
                        buttonClass = "tpButtonUp";
                        if (cp.Visit.GetBoolean("allowQuickEditor", false)) {
                            buttonState = "0";
                            buttonClass = "tpButtonDown";
                            editHidden = "<input type=\"hidden\" name=\"1AllowQuickEditor\" value=\"1\">";
                        }
                        string quickCaption = (cp.Site.GetBoolean("ALLOW ADDONLIST EDITOR FOR QUICK EDITOR", false)) ? "Page&nbsp;Builder" : "Quick&nbsp;Edit";
                        copy = ""
                            + debugHidden
                            + previewHidden
                            + cr + "<input type=\"hidden\" name=\"1AllowQuickEditor\" value=\"" + buttonState + "\">"
                            + cr + "<a id=\"tpButtonQuick\" href=\"#\">" + quickCaption + "</a>";
                        copy = ""
                            + cr + formOpen
                            + cp.Html.Indent(copy, 1)
                            + cr + formClose;
                        layout.SetInner("#tpEditTabQuick", copy);
                        jsOnReady += cr + "jQuery('#tpButtonQuick').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                        jsOnReady += cr + "jQuery('#tpButtonQuick').addClass('" + buttonClass + "');";
                        //
                        swHints += ",end quickeditbutton (" + sw.ElapsedMilliseconds.ToString() + ")";
                        //
                        // Advanced button
                        //
                        buttonState = "1";
                        buttonClass = "tpButtonUp";
                        if (cp.Visit.GetBoolean("AllowAdvancedEditor", false)) {
                            buttonState = "0";
                            buttonClass = "tpButtonDown";
                            editHidden = "<input type=\"hidden\" name=\"1AllowAdvancedEditor\" value=\"1\">";
                        }
                        copy = ""
                            + debugHidden
                            + previewHidden
                            + cr + "<input type=\"hidden\" name=\"1AllowAdvancedEditor\" value=\"" + buttonState + "\">"
                            + cr + "<a id=\"tpButtonAdvanced\" href=\"#\">Advanced</a>";
                        copy = ""
                            + cr + formOpen
                            + cp.Html.Indent(copy, 1)
                            + cr + formClose;
                        layout.SetInner("#tpEditTabAdv", copy);
                        jsOnReady += cr + "jQuery('#tpButtonAdvanced').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                        jsOnReady += cr + "jQuery('#tpButtonAdvanced').addClass('" + buttonClass + "');";
                        //
                        swHints += ",end advancedbutton (" + sw.ElapsedMilliseconds.ToString() + ")";
                        //
                        // Debug Button
                        //
                        if (cp.User.IsDeveloper) {
                            buttonState = "1";
                            buttonClass = "tpButtonUp";
                            if (isDebugging) {
                                buttonState = "0";
                                buttonClass = "tpButtonDown";
                                //editHidden = "<input type=\"hidden\" name=\"1AllowAdvancedEditor\" value=\"1\">";
                            }
                            copy = ""
                                + editHidden
                                + previewHidden
                                + cr + "<input type=\"hidden\" name=\"1allowDebugging\" value=\"" + buttonState + "\">"
                                + cr + "<a id=\"tpButtonDebug\" href=\"#\">Debug</a>";
                            copy = ""
                                + cr + formOpen
                                + cp.Html.Indent(copy, 1)
                                + cr + formClose;
                            layout.SetInner("#tpEditTabDebug", copy);
                            jsOnReady += cr + "jQuery('#tpButtonDebug').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                            jsOnReady += cr + "jQuery('#tpButtonDebug').addClass('" + buttonClass + "');";
                            //copy = "0";
                            //if (allowDebugging)
                            //{
                            //    copy = "1";
                            //}
                            //copy = ""
                            //    + formOpen
                            //    + "<a href=\"#\" class=\"tpButtonUp\">Debug</a>"
                            //    + editHidden
                            //    + "<input type=\"hidden\" name=\"1allowDebugging\" value=\"" + copy + "\">"
                            //    + formClose;
                            //block.SetInner("#tpEditTabDebug", copy);
                        } else {
                            layout.SetOuter("#tpEditTabDebug", "");
                        }
                        //
                        swHints += ",end debugbutton  (" + sw.ElapsedMilliseconds.ToString() + ")";
                        //
                        // Admin button
                        //
                        copy = "<a href=\"" + cp.Site.GetText("adminUrl", "/admin") + "\">Admin</a>";
                        layout.SetInner("#tpEditTabAdmin", copy);
                    }
                }
                doc = ""
                    + cp.Html.div("<!-- start -->" + layout.GetHtml() + "<!-- end -->", "", "", "toolPanel")
                    + cp.Html.div(cp.Doc.Body, "", "", "toolPanelContent")
                    + "";
                cp.Doc.Body = doc;
                //string manualJs = "jQuery(document).ready(function(){" + jsOnReady + "})";
                string manualJs = ""
                    + "function bindReadyState(callback){"
                    + " if (document.readyState != 'loading') callback();"
                    + " else if (document.addEventListener) document.addEventListener('DOMContentLoaded', callback);"
                    + " else document.attachEvent('onreadystatechange', function(){ if (document.readyState == 'complete') callback(); });"
                    + "}"
                    + " bindReadyState(function(){" + jsOnReady + "});"
                    + "";
                string manualStyles = "#toolPanel #tpDraggable {left:" + cp.Visit.GetInteger("toolPanelPositionLeft") + "px;display:block}";
                //string bodyEnd = ""
                //    + cr + "<script type=\"text/javascript\" language=\"javascript\">jQuery(document).ready(function(){" + jsOnReady + "})</script>"
                //    + cr + "<style>#toolPanel #tpDraggable {left:" + cp.Visit.GetInteger("toolPanelPositionLeft") + "px;display:block}</style>";
                cp.Doc.AddHeadJavascript(manualJs);
                cp.Doc.AddHeadStyle(manualStyles);
                //cp.Doc.AddBodyEnd(bodyEnd);
                //
                swHints += ",exit  (" + sw.ElapsedMilliseconds.ToString() + ")";
                //
                cp.Site.TestPoint("ToolPanel Hints [" + swHints + "]");
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return "";
        }
    }
}
