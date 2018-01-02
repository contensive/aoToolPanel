
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using static Contensive.Addons.aoToolPanel.constants;

namespace Contensive.Addons
{
    //
    //====================================================================================================
    //
    public class toolPanelClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        public override object Execute(Contensive.BaseClasses.CPBaseClass cp)
        {
            string jsOnReady = "";
            string doc;
            string copy;
            bool isDebugging = false;
            CPBlockBaseClass layout = cp.BlockNew();
            //string buttonLeftDown = "<a class=\"tpButtonDown\" href=\"?ccFormSN=1&1Type=do30a8vl29&1mb=%20%20Apply%20";
            //string buttonLeftUp = "<a class=\"tpButtonUp\" href=\"?ccFormSN=1&1Type=do30a8vl29&1mb=%20%20Apply%20";
            //string buttonRight = "\">";
            //string debugState = "";
            string editHidden = "";
            string footer = "";
            string formOpen = "";
            string formClose = "";
            string debugHidden = "";
            string buttonState = "";
            string buttonClass="";
            string previewHidden = "";
            bool isPreviewing = false;
            bool isAccountTabLocked = false;
            bool isLoginTabPinned = false;
            bool isLoginTabDefaultPinned = false;
            bool isAccountTabDefaultPinned = false;
            //
            const string layoutCacheName = "toolpanel/toolpanel.html";
            string layoutHtml = cp.Cache.Read(layoutCacheName);
            if ( string.IsNullOrEmpty( layoutHtml )) {
                layoutHtml = cp.File.Read( cp.Site.PhysicalWWWPath + @"toolpanel\toolpanel.html");
                cp.Cache.Save(layoutCacheName, layoutHtml);
            }
            layout.Load(layoutHtml);
            //layout.OpenFile(@"toolpanel\toolpanel.html");
            //
            if (cp.User.GetProperty(upLoginTabIsPinned, "", 0)=="")
            {
                //
                // if new user, set the default state of the login tab pin (pinned down or autohide)
                //
                isLoginTabDefaultPinned = cp.Utils.EncodeBoolean(cp.Site.GetProperty(spLoginTabIsPinnedByDefault, "0"));
                cp.User.SetProperty(upLoginTabIsPinned, isLoginTabDefaultPinned.ToString().ToLower(), 0);
            }
            //
            //  set default for login tab
            //
            jsOnReady += cr + "isLockedAccountTab=" + cp.Site.GetBoolean("toolPanelDisableAutoHideAccountTab", "0").ToString().ToLower() + ";";
            isAccountTabDefaultPinned = cp.Utils.EncodeBoolean(cp.Site.GetProperty("toolPanelPinAccountTab", "0"));
            if (cp.User.GetProperty("isLockedAccountTab", "", 0)=="")
            {
                cp.User.SetProperty("isLockedAccountTab", isAccountTabDefaultPinned.ToString().ToLower(), 0);
            }
            //
            if (!cp.User.IsAuthenticated)
            {
                //
                // not logged in, return with just the login
                //
                layout.SetOuter("#tpAccountPanel", "");
                layout.SetOuter("#tpAccountTab", "");
                layout.SetOuter("#tpEditTab", "");
                //
                //  check if login tab allowed via settings page
                //
                if (!cp.Utils.EncodeBoolean(cp.Site.GetProperty(spLoginTabAllowed, "")))
                {
                    layout.SetOuter("#tpLoginTab", "");
                }
                else
                {
                    isLoginTabPinned = cp.Utils.EncodeBoolean(cp.User.GetProperty(upLoginTabIsPinned, "0", 0));
                    if (isLoginTabPinned)
                    {
                        jsOnReady += cr + "tpAbortLoginTabClose=true;";
                        jsOnReady += cr + "tpLoginTabIsPinned=true;";
                        jsOnReady += cr + "jQuery('#tpLoginLock').html('<img src=\"/toolPanel/lockClosed.png\" width=\"10\" height=\"10\">');";
                    }
                    else
                    {
                        jsOnReady += cr + "tpLoginTabIsPinned=false;";
                    }
                }
            }
            else
            {
                //
                // logged in, block the login
                //
                layout.SetOuter("#tpLoginPanel", "");
                layout.SetOuter("#tpLoginTab", "");
                //
                //  check if account tab allowed via settings page
                //
                if (cp.Utils.EncodeBoolean(cp.Site.GetProperty("toolPanelShowAccountTab", "")))
                {
                    //
                    // Account Tab
                    //
                    isAccountTabLocked = cp.Utils.EncodeBoolean(cp.User.GetProperty("isLockedAccountTab", "0", 0));
                    if (isAccountTabLocked)
                    {
                        copy = "true";
                        jsOnReady += cr + "jQuery('#tpAccountLock').html('<img src=\"/toolPanel/lockClosed.png\" width=\"10\" height=\"10\">');";
                    }
                    else
                    {
                        copy = "false";
                    }
                    jsOnReady += cr + "isLockedAccountTab=" + copy + ";";
                }
                else
                {
                    layout.SetOuter("#tpAccountTab", "");
                }
                //
                //
                // Edit Tab
                //
                if (!cp.User.IsContentManager(""))
                {
                    layout.SetOuter("#tpEditTab", "");
                }
                else
                {
                    //
                    footer = ""
                        + cr + "<div class=\"tpInner\">"
                        + "<span>Contensive " + cp.Version + "</span>"
                        + "|<span>" + cp.Doc.StartTime + "</span>"
                        + "|<span><a href=\"http://support.Contensive.com/\" target=\"_blank\" class=\"ccAdminLink\">Support</a></span>"
                        + "|<span><a href=\"" + cp.Site.GetProperty("adminurl","") + "\" class=\"ccAdminLink\">Admin Home</a></span>"
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
                    editHidden = "";
                    isDebugging = cp.Utils.EncodeBoolean(cp.Visit.GetProperty("allowDebugging", "0", 0));
                    isPreviewing = cp.Utils.EncodeBoolean(cp.Visit.GetProperty("AllowWorkflowRendering", "0", 0));
                    //
                    if (isDebugging)
                    {
                        debugHidden = cr + "<input type=\"hidden\" name=\"1allowDebugging\" value=\"1\">";
                    }
                    if (isPreviewing)
                    {
                        previewHidden = cr + "<input type=\"hidden\" name=\"1allowWorkflowRendering\" value=\"1\">";
                    }
                    //
                    // Edit button
                    //
                    buttonState = "1";
                    buttonClass = "tpButtonUp";
                    if (cp.Utils.EncodeBoolean(cp.Visit.GetProperty("allowEditing", "0", 0)))
                    {   
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
                        + cp.Html.Indent( copy, 1  )
                        + cr + formClose;
                    layout.SetInner("#tpEditTabEdit", copy);
                    jsOnReady += cr + "jQuery('#tpButtonEdit').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                    jsOnReady += cr + "jQuery('#tpButtonEdit').addClass('" + buttonClass + "');";
                    //
                    // Quick Edit button
                    //
                    buttonState = "1";
                    buttonClass = "tpButtonUp";
                    if (cp.Utils.EncodeBoolean(cp.Visit.GetProperty("allowQuickEditor", "0", 0)))
                    {   
                        buttonState = "0";
                        buttonClass = "tpButtonDown";
                        editHidden = "<input type=\"hidden\" name=\"1AllowQuickEditor\" value=\"1\">";
                    }
                    copy = ""
                        + debugHidden
                        + previewHidden
                        + cr + "<input type=\"hidden\" name=\"1AllowQuickEditor\" value=\"" + buttonState + "\">"
                        + cr + "<a id=\"tpButtonQuick\" href=\"#\">Quick&nbsp;Edit</a>";
                    copy = ""
                        + cr + formOpen
                        + cp.Html.Indent( copy, 1 )
                        + cr + formClose;
                    layout.SetInner("#tpEditTabQuick", copy);
                    jsOnReady += cr + "jQuery('#tpButtonQuick').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                    jsOnReady += cr + "jQuery('#tpButtonQuick').addClass('" + buttonClass + "');";
                    //
                    // Advanced button
                    //
                    buttonState = "1";
                    buttonClass = "tpButtonUp";
                    if (cp.Utils.EncodeBoolean(cp.Visit.GetProperty("AllowAdvancedEditor", "0", 0)))
                    {
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
                    // Preview button
                    //
                    if (!cp.Utils.EncodeBoolean( cp.Site.GetProperty("allowWorkflowAuthoring","0")))
                    {
                        layout.SetOuter("#tpEditTabPreview", "");
                    }
                    else
                    {
                        buttonState = "1";
                        buttonClass = "tpButtonUp";
                        if (isPreviewing)
                        {
                            buttonState = "0";
                            buttonClass = "tpButtonDown";
                            previewHidden = "<input type=\"hidden\" name=\"1AllowWorkflowRendering\" value=\"1\">";
                        }
                        copy = ""
                            + cr + editHidden
                            + cr + debugHidden
                            + cr + "<input type=\"hidden\" name=\"1AllowWorkflowRendering\" value=\"" + buttonState + "\">"
                            + cr + "<a id=\"tpButtonPreview\" href=\"#\">Preview</a>";
                        copy = ""
                            + cr + formOpen
                            + cp.Html.Indent(copy, 1)
                            + cr + formClose;
                        layout.SetInner("#tpEditTabPreview", copy);
                        jsOnReady += cr + "jQuery('#tpButtonPreview').click( function () { jQuery(this).parents('form:first').submit(); return false });";
                        jsOnReady += cr + "jQuery('#tpButtonPreview').addClass('" + buttonClass + "');";
                    }
                    //
                    // Debug Button
                    //
                    if (cp.User.IsDeveloper)
                    {
                        buttonState = "1";
                        buttonClass = "tpButtonUp";
                        if (isDebugging)
                        {
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
                    }
                    else
                    {
                        layout.SetOuter("#tpEditTabDebug", "");
                    }
                    //
                    // Admin button
                    //
                    copy = "<a href=\"" + cp.Site.GetProperty("adminUrl","/admin") + "\">Admin</a>";
                    layout.SetInner("#tpEditTabAdmin", copy);
                }
            }
            doc = ""
                + cp.Html.div("<!-- start -->" + layout.GetHtml() + "<!-- end -->", "", "", "toolPanel")
                + cp.Html.div(cp.Doc.Body, "", "", "toolPanelContent")
                //+ footer
                + "";
            cp.Doc.Body = doc;
            string bodyEnd = ""
                + cr + "<script type=\"text/javascript\" language=\"javascript\">jQuery(document).ready(function(){" + jsOnReady + "})</script>"
                + cr + "<style>#toolPanel #tpDraggable {left:" + cp.Visit.GetInteger("toolPanelPositionLeft") + "px;display:block}</style>";
            cp.Doc.AddBodyEnd(bodyEnd);
            //cp.Doc.AddBodyEnd(cr + "<script type=\"text/javascript\" language=\"javascript\">" + jsOnReady + cr + "</script>");
            return "";
        }
    }
}
