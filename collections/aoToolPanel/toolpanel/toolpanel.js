/*
properties
*/
var tpLoginTabIsPinned = false;
var isLockedAccountTab = false;
var isLoadedLoginPanel = false;
var isOpenLoginPanel = false;
var tpAbortLoginTabClose = false;
var timerIdLoginTab = 0;
/*
set tool panel position
*/
var coordinates = function (element) {
    element = $(element);
    var top = element.position().top;
    var left = element.position().left;
    $('#results').text('X: ' + left + ' ' + 'Y: ' + top);
    var queryString = 'top=' + top + '&left=' + left;
    cj.remote({ 'method': 'remoteToolPanelPosition', 'queryString': queryString });
}
/*
bind
*/
$(document).ready(function() {
    $('#tpDraggable').draggable({
        axis: "x"
    }, {
        stop: function () {
            coordinates('#tpDraggable');
        }
    });
    $('body').on('mouseup', '#tpDraggable', function(e){});
    $('body').on('keypress', '#panelLoginPassword', function (e) { tpLoginFormKeypress(e) }); 
    $('body').on('keypress', '#panelLoginEmail', function (e) { tpLoginFormKeypress(e) });
    $('body').on('keypress', '#panelLoginUsername', function (e) { tpLoginFormKeypress(e) });
    $('body').on('mouseenter', '#tpTabZone', function (e) { tpMouseEnterTabZone() });
    $('body').on('mouseleave', '#tpTabZone', function (e) { tpMouseLeaveTabZone() });
    $('body').on('click', '#tpAccountLock', function (e) { tpClickAccountTabLock() });
    $('body').on('mouseenter', '#tpAccountTab', function (e) { tpMouseEnterAccountTab() });
    $('body').on('mouseleave', '#tpAccountTab', function (e) { tpMouseLeaveAccountTab() });
    $('body').on('click', '#tpAccountButton', function (e) { tpToggleAccountPanel() });
    $('body').on('click', '#tpLoginLock', function (e) { clickLoginTabLock() });
    $('body').on('mouseenter', '#tpLoginTab', function (e) { mouseEnterLoginTab() });
    $('body').on('mouseleave', '#tpLoginTab', function (e) { mouseLeaveLoginTab() });
    $('body').on('click', '#tpLoginButton', function (e) { tpToggleLoginPanel() });
    $('body').on('click', '#tpLoginFormSubmit', function (e) { tpLoginFormSubmit() });
    $('body').on('click', '#tpEmailFormSubmit', function (e) { tpEmailFormSubmit() });
    $('body').on('click', '#tpLoginFormRegister', function (e) { tpRequestRegister() });
    $('body').on('click', '#accountSubmit', function (e) { tpAccountSubmit() });
    $('body').on('click', '#logoutClick', function (e) { tpAccountLogoutClick() });
    $('body').on('click', '#tpRegisterSubmit', function (e) { tpRegisterSubmit() });
})
/*
utils
*/
function tpSetSpinner(divID, message, containerHeight) {
    var divContent;
    divContent = '<div class="spinner"><img src="/images/transpin.gif" /><p>' + message + '</p></div>';
    $('#' + divID).html(divContent);
    $('.spinner').height(containerHeight);
}
/*
*/
function tpGetParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}
/*
*/
function tpRedirectHome(returnVal, container) {
    var testString = returnVal;
    //
    testString = testString.replace(/(\r\n|\n|\r)/gm, ' ').replace(/(\s)/g, '');
    //
    if (testString == '') {
        window.location = document.head.baseURI;
    } else {
        $('#' + container).html(returnVal);
    }
}
/*
tab zone
*/
function tpMouseEnterTabZone() {
    tpIsWantedAccountTab = true;
    $("#tpAccountTab").slideDown("slow");
    tpAbortLoginTabClose = true;
    $("#tpLoginTab").slideDown("slow");
}
/*
*/
function tpMouseLeaveTabZone() {
    tpMouseLeaveAccountTab();
    mouseLeaveLoginTab();
}
/*
account tab
*/
var tpIsLoadedAccountPanel = false;
var tpIsOpenAccountPanel = false;
var tpIsWantedAccountTab = false;
var tpTimerIdAccountTab = 0;
/*
*/
function tpToggleAccountPanel() {
    if (tpIsOpenAccountPanel) {
        tpCloseAccountPanel();
    } else {
        tpOpenAccountPanel();
    }
    return false;
}
/*
*/
function tpOpenAccountPanel() {
    tpIsOpenAccountPanel = true;
    if (!tpIsLoadedAccountPanel) {
        setAccountWait();
        cj.ajax.addonCallback('toolPanelAccountForm', '', function (content, ignore) {
            tpClearAccountWait();
            $('#tpAccountPanel').html(content);
            tpOpenAccountPanelReady();
            tpIsLoadedAccountPanel = true;
        });
    } else {
        tpOpenAccountPanelReady();
    }
}
/*
*/
function tpOpenAccountPanelReady() {
    closeLoginPanel();
    $("#tpAccountTab").addClass("tpOpen");
    $("#tpAccountPanel").slideDown("slow");
}
/*
*/
function setAccountWait() {
    $('#tpAccountWait').css('visibility', 'visible');
}
/*
*/
function tpClearAccountWait() {
    $('#tpAccountWait').css('visibility', 'hidden');
}
/*
*/
function tpCloseAccountPanel() {
    $("#tpAccountPanel").slideUp("slow");
    $("#tpAccountTab").removeClass("tpOpen");
    tpIsOpenAccountPanel = false;
}
/*
*/
function tpClickAccountTabLock() {
    if (isLockedAccountTab) {
        $('#tpAccountLock').html('<img src="/toolPanel/lockOpen.png" height="10" width="10">');
        cj.ajax.addon('setUserProperty', 'n=isLockedAccountTab&v=0');
        isLockedAccountTab = false;
    } else {
        $('#tpAccountLock').html('<img src="/toolPanel/lockClosed.png" height="10" width="10">');
        cj.ajax.addon('setUserProperty', 'n=isLockedAccountTab&v=1');
        isLockedAccountTab = true;
    }

}
/*
*/
function tpMouseEnterAccountTab() {
    if (!isLockedAccountTab) {
        if (tpTimerIdAccountTab != 0) {
            clearTimeout(tpTimerIdAccountTab);
            tpTimerIdAccountTab = 0;
        }
        tpIsWantedAccountTab = true;
    }
}
/*
*/
function tpMouseLeaveAccountTab() {
    if ((!isLockedAccountTab) & (!tpIsOpenAccountPanel)) {
        if (tpTimerIdAccountTab != 0) {
            clearTimeout(tpTimerIdAccountTab);
            tpTimerIdAccountTab = 0;
        }
        tpTimerIdAccountTab = setTimeout("accountTabClose()", 2000);
        tpIsWantedAccountTab = false;
    }
}
/*
*/
function accountTabClose() {
    if (!tpIsWantedAccountTab) {
        $("#tpAccountTab").slideUp("slow");
    } else {
        tpIsWantedAccountTab = tpIsWantedAccountTab;
    }
}
/*
login
*/
function tpToggleLoginPanel() {
    if (isOpenLoginPanel) {
        closeLoginPanel();
    } else {
        tpOpenLoginPanel();
    }
    return false;
};
/*
*/
function tpOpenLoginPanel() {
    isOpenLoginPanel = true;
    if (!isLoadedLoginPanel) {
        setLoginWait();
        cj.ajax.addonCallback('toolPanelLoginForm', '', function (content, ignore) { tpStoreAndOpenLoginPanel(content) });
    } else {
        tpOpenLoginPanel_contentReady();
    }
}
/*
*/
function tpStoreAndOpenLoginPanel(content) {
    clearLoginWait();
    $('#tpLoginPanel').html(content);
    tpOpenLoginPanel_contentReady();
    isLoadedLoginPanel = true;
}
/*
*/
function tpOpenLoginPanel_contentReady() {
    tpCloseAccountPanel();
    $("#tpLoginTab").addClass("tpOpen");
    $("#tpLoginPanel").slideDown("slow");
}
/*
*/
function setLoginWait() {
    $('#tpLoginWait').css('visibility', 'visible');
}
/*
*/
function clearLoginWait() {
    $('#tpLoginWait').css('visibility', 'hidden');
}
/*
*/
function closeLoginPanel() {
    $("#tpLoginPanel").slideUp("slow");
    $("#tpLoginTab").removeClass("tpOpen");
    isOpenLoginPanel = false;
}
/*
*/
function clickLoginTabLock() {
    if (tpLoginTabIsPinned) {
        $('#tpLoginLock').html('<img src="/toolPanel/lockOpen.png" height="10" width="10">');
        cj.ajax.addon('setUserProperty', 'n=tpLoginTabIsPinned&v=0');
        tpLoginTabIsPinned = false;
    } else {
        $('#tpLoginLock').html('<img src="/toolPanel/lockClosed.png" height="10" width="10">');
        cj.ajax.addon('setUserProperty', 'n=tpLoginTabIsPinned&v=1');
        tpLoginTabIsPinned = true;
    }
}
/*
*/
function mouseEnterLoginTab() {
    if (!tpLoginTabIsPinned) {
        if (timerIdLoginTab != 0) {
            clearTimeout(timerIdLoginTab);
            timerIdLoginTab = 0;
        }
        tpAbortLoginTabClose = true;
    }
}
/*
*/
function mouseLeaveLoginTab() {
    if ((!tpLoginTabIsPinned) & (!isOpenLoginPanel)) {
        if (timerIdLoginTab != 0) {
            clearTimeout(timerIdLoginTab);
            timerIdLoginTab = 0;
        }
        timerIdLoginTab = setTimeout("LoginTabClose()", 2000);
        tpAbortLoginTabClose = false;
    }
}
/*
*/
function LoginTabClose() {
    if (!tpAbortLoginTabClose) {
        $("#tpLoginTab").slideUp("slow");
    } else {
        tpAbortLoginTabClose = tpAbortLoginTabClose;
    }
}
/*
*/
function tpLoginFormSubmit() {
    var varString;
    if (!validateLoginForm()) {
        return false;
    } else {
        //
        varString = 'panelLoginUsername=' + $('#panelLoginUsername').val();
        varString += '&panelLoginPassword=' + $('#panelLoginPassword').val();
        varString += '&panelLoginEmail=' + $('#panelLoginEmail').val();
        varString += '&panelLoginAuto=' + $('#panelLoginAuto').val();
        //
        cj.ajax.addonCallback('toolPanelLoginFormHandler', varString, tpRedirectHome, 'panelFormContainer');
        //
        tpSetSpinner('panelFormContainer', 'Authenticating Account....', $('#panelFormContainer').height());
        return false;
    }
}
/*
*/
function tpLoginFormKeypress(e) {
    if (e.which == 13) {
        tpLoginFormSubmit();
        e.preventDefault();
        return false;
    }
}
/*
account form
*/
function validateAccountForm() {
    var errFlag;
    var errMsg;
    //
    var firstName = $('#panelAccountFirstName').val();
    var lastName = $('#panelAccountLastName').val();
    var email = $('#panelAccountEmail').val();
    //
    if (firstName == null || firstName == '') {
        $('#panelAccountFirstName').css('background-color', '#dddddd');
        errFlag = 1;
    }
    else {
        $('#panelAccountFirstName').css('background-color', '#ffffff');
    };
    if (lastName == null || lastName == '') {
        $('#panelAccountLastName').css('background-color', '#dddddd');
        errFlag = 1;
    }
    else {
        $('#panelAccountLastName').css('background-color', '#ffffff');
    };
    //
    if (email == null || email == '') {
        $('#panelAccountEmail').css('background-color', '#dddddd');
        errFlag = 1;
    }
    else {
        $('#panelAccountEmail').css('background-color', '#ffffff');
    };
    //
    if (errFlag == 1) {
        if (errMsg == '' || errMsg == null || errMsg == 'undefined') {
            errMsg = 'Please complete all required fields in order to continue.';
        }
        alert(errMsg);
        return false;
    }
    else {
        return true;
    };
};
/*
*/
function tpAccountSubmit() {
    if (!validateAccountForm()) {
        return false;
    }
    var varString;
    varString = 'panelAccountFirstName=' + $('#panelAccountFirstName').val();
    varString += '&panelAccountLastName=' + $('#panelAccountLastName').val();
    varString += '&panelAccountEmail=' + $('#panelAccountEmail').val();
    varString += '&panelAccountUsername=' + $('#panelAccountUsername').val();
    varString += '&panelAccountPassword=' + $('#panelAccountPassword').val();
    //
    cj.ajax.addon('toolPanelAccountFormHandler', varString, '', 'panelFormContainer', '', '');
    tpSetSpinner('panelFormContainer', 'Updating Account....', $('#panelFormContainer').height());
    return false;
}
/*
*/
function tpAccountLogoutClick() {
    cj.ajax.addonCallback('toolPanelLogoutHandler', '', tpRedirectHome, 'panelFormContainer');
    tpSetSpinner('panelFormContainer', 'Logging Out....', $('#panelFormContainer').height());
    return false;
}
/*
login form
*/
function validateLoginForm() {
    var errFlag;
    var errMsg;
    //
    var username = $('#panelLoginUsername').val();
    var password = $('#panelLoginPassword').val();
    var email = $('#panelLoginEmail').val();
    //
    // fields present
    //
    var usernamePresent = $('#reqUsername').val();
    var passwordPresent = $('#reqPassword').val();
    var emailPresent = $('#reqEmail').val();
    //
    if (passwordPresent == 1 && (password == null || password == '')) {
        $('#panelLoginPassword').css('background-color', '#f0f890').focus();
        errFlag = 1;
    }
    else {
        $('#panelLoginPassword').css('background-color', '#ffffff');
    }
    //
    if (usernamePresent == 1 && (username == null || username == '')) {
        $('#panelLoginUsername').css('background-color', '#f0f890').focus();
        errFlag = 1;
    }
    else {
        $('#panelLoginUsername').css('background-color', '#ffffff');
    }
    //
    if (emailPresent == 1 && (email == null || email == '')) {
        $('#panelLoginEmail').css('background-color', '#f0f890').focus();
        errFlag = 1;
    }
    else {
        $('#panelLoginEmail').css('background-color', '#ffffff');
    };
    //
    if (errFlag == 1) {
        if (errMsg == '' || errMsg == null || errMsg == 'undefined') {
            errMsg = 'Please complete all fields in order to login.';
        }
        alert(errMsg);
        return false;
    }
    else {
        return true;
    };
};
/*
send password form
*/
function validateEmailForm() {
    var errFlag;
    var errMsg;
    //
    var email = $('#panelEmailEmail').val();
    //
    if (email == null || email == '') {
        $('#panelEmailEmail').css('background-color', '#f0f890');
        errFlag = 1;
    }
    else {
        $('#panelEmailEmail').css('background-color', '#ffffff');
    };
    //
    if (errFlag == 1) {
        if (errMsg == '' || errMsg == null || errMsg == 'undefined') {
            errMsg = 'Please enter your email address in order to retrieve your login information.';
        }
        alert(errMsg);
        return false;
    }
    else {
        return true;
    };
};
/*
*/
function tpEmailFormSubmit() {
    if (!validateEmailForm()) {
        return false;
    } else {
        cj.ajax.addon('toolPanelLoginFormHandler', '', 'panelLoginForm', 'panelFormContainer', '', '');
        tpSetSpinner('panelFormContainer', 'Gathering Account Information....', $('#panelFormContainer').height());
    }
}
/*
Registration Form
*/
function tpValidateRegisterForm() {
    var errFlag;
    var errMsg;
    //
    var usernamePresent = $('#reqUsername').val();
    var passwordPresent = $('#reqPassword').val();
    //
    var firstName = $('#panelRegistrationFirstName').val();
    var lastName = $('#panelRegistrationLastName').val();
    var email = $('#panelRegistrationEmail').val();
    var username = $('#panelRegistrationUsername').val();
    var password = $('#panelRegistrationPassword').val();
    //
    if (firstName == null || firstName == '') {
        $('#panelRegistrationFirstName').css('background-color', '#f0f890');
        errFlag = 1;
    }
    else {
        $('#panelRegistrationFirstName').css('background-color', '#ffffff');
    };
    if (lastName == null || lastName == '') {
        $('#panelRegistrationLastName').css('background-color', '#f0f890');
        errFlag = 1;
    }
    else {
        $('#panelRegistrationLastName').css('background-color', '#ffffff');
    };
    //
    if (email == null || email == '') {
        $('#panelRegistrationEmail').css('background-color', '#f0f890');
        errFlag = 1;
    }
    else {
        $('#panelRegistrationEmail').css('background-color', '#ffffff');
    };
    //
    if (email == null || email == '') {
        $('#panelRegistrationEmail').css('background-color', '#f0f890');
        errFlag = 1;
    }
    else {
        $('#panelRegistrationEmail').css('background-color', '#ffffff');
    };
    //
    if (usernamePresent == 1 && (username == null || username == '')) {
        $('#panelRegistrationUsername').css('background-color', '#f0f890');
        errFlag = 1;
    }
    else {
        $('#panelRegistrationUsername').css('background-color', '#ffffff');
    };
    if (passwordPresent == 1 && (password == null || password == '')) {
        $('#panelRegistrationPassword').css('background-color', '#f0f890');
        errFlag = 1;
    }
    else {
        $('#panelRegistrationPassword').css('background-color', '#ffffff');
    };
    //
    if (errFlag == 1) {
        if (errMsg == '' || errMsg == null || errMsg == 'undefined') {
            errMsg = 'Please complete all required fields in order to continue.';
        }
        alert(errMsg);
        return false;
    }
    else {
        return true;
    }
}
/*
*/
function tpRegisterSubmit() {
    if (!tpValidateRegisterForm()) {
        return false;
    }
    var varString;
    varString = 'panelRegistrationFirstName=' + $('#panelRegistrationFirstName').val();
    varString += '&panelRegistrationLastName=' + $('#panelRegistrationLastName').val();
    varString += '&panelRegistrationEmail=' + $('#panelRegistrationEmail').val();
    varString += '&panelRegistrationUsername=' + $('#panelRegistrationUsername').val();
    varString += '&panelRegistrationPassword=' + $('#panelRegistrationPassword').val();
    //
    cj.ajax.addonCallback('toolPanelRegistrationFormHandler', varString, tpRedirectHome, 'panelFormContainer');
    //cj.ajax.addon('toolPanelRegistrationFormHandler', varString, '', 'panelFormContainer', '','');
    tpSetSpinner('panelFormContainer', 'Creating Account...', $('#panelFormContainer').height());
    return false;
}
/*
*/
function tpRequestRegister() {
    cj.ajax.addon('toolPanelDefaultRegistrationForm', '', '', 'panelFormContainer', '', '');
    tpSetSpinner('panelFormContainer', '', $('#panelFormContainer').height());
    return false;
}
/*
*/
$(function () {
    $("#tpDraggable").draggable({ axis: "y" });
    $("#tpDraggable").draggable({ axis: "x" });

    $("#tpDraggable").draggable({ containment: "#containment-wrapper", scroll: false });
    $("#draggable5").draggable({ containment: "parent" });
});

