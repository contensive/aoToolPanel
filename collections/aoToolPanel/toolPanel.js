
/*
properties2
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
    $('body').on('keypress', '#panelLoginPassword', function (e) { return tpLoginFormKeypress(e) }); 
    $('body').on('keypress', '#panelLoginEmail', function (e) { return tpLoginFormKeypress(e) }); 
    $('body').on('keypress', '#panelLoginUsername', function (e) { return tpLoginFormKeypress(e) }); 
    $('body').on('mouseenter', '#tpTabZone', function (e) { return tpMouseEnterTabZone(e) }); 
    $('body').on('mouseleave', '#tpTabZone', function (e) { return tpMouseLeaveTabZone(e) }); 
    $('body').on('click', '#tpAccountLock', function (e) { return tpClickAccountTabLock(e) }); 
    $('body').on('mouseenter', '#tpAccountTab', function (e) { return tpMouseEnterAccountTab(e) }); 
    $('body').on('mouseleave', '#tpAccountTab', function (e) { return tpMouseLeaveAccountTab(e) });
    $('body').on('click', '#tpAccountButton', function (e) { return tpToggleAccountPanel(e) });
    $('body').on('click', '#tpLoginLock', function (e) { return clickLoginTabLock(e) });
    $('body').on('mouseenter', '#tpLoginTab', function (e) { return mouseEnterLoginTab(e) });
    $('body').on('mouseleave', '#tpLoginTab', function (e) { return mouseLeaveLoginTab(e) });
    $('body').on('click', '#tpLoginButton', function (e) { return tpToggleLoginPanel(e) });
    $('body').on('click', '#tpLoginFormSubmit', function (e) { return tpLoginFormSubmit(e) });
    $('body').on('click', '#tpEmailFormSubmit', function (e) { return tpEmailFormSubmit(e) });
    $('body').on('click', '#tpLoginFormRegister', function (e) { return tpRequestRegister(e) });
    $('body').on('click', '#accountSubmit', function (e) { return tpAccountSubmit(e) });
    $('body').on('click', '#logoutClick', function (e) { return tpAccountLogoutClick(e) });
    $('body').on('click', '#tpRegisterSubmit', function (e) { return tpRegisterSubmit(e) });
    // -- enable bootstrap tooltips
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
})
/*
utils
*/
function tpSetSpinner(divId, message, containerHeight) {
    var divContent;
    divContent = '<div class="spinner"><img src="/toolpanel/transpin.gif" /><p>' + message + '</p></div>';
    $('#' + divId).html(divContent);
    $('.spinner').height(containerHeight);
}

/*
*/
function tpRedirectHome(returnVal, container) {
    var testString = returnVal.replace(/(\r\n|\n|\r)/gm, ' ').replace(/(\s)/g, '');
    if (testString == '') {
        window.location = document.head.baseURI;
    } else {
        $('#' + container).html(returnVal);
    }
}
/*
tab zone
*/
function tpMouseEnterTabZone(e) {
    tpIsWantedAccountTab = true;
    $("#tpAccountTab").slideDown("slow");
    tpAbortLoginTabClose = true;
    $("#tpLoginTab").slideDown("slow");
}
/*
*/
function tpMouseLeaveTabZone(e) {
    tpMouseLeaveAccountTab(e);
    mouseLeaveLoginTab(e);
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
function tpToggleAccountPanel(e) {
    e.stopPropagation();
    e.preventDefault();
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
function tpOpenAccountPanelReady(e) {
    closeLoginPanel(e);
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
function tpClickAccountTabLock(e) {
    e.stopPropagation();
    e.preventDefault();
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
function tpMouseEnterAccountTab(e) {
    e.stopPropagation();
    e.preventDefault();
    if (!isLockedAccountTab) {
        if (tpTimerIdAccountTab != 0) {
            clearTimeout(tpTimerIdAccountTab);
            tpTimerIdAccountTab = 0;
        }
        tpIsWantedAccountTab = true;
    }
	return false;
}
/*
*/
function tpMouseLeaveAccountTab(e) {
    e.stopPropagation();
    e.preventDefault();
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
function accountTabClose(e) {
    if (!tpIsWantedAccountTab) {
        $("#tpAccountTab").slideUp("slow");
    } else {
        tpIsWantedAccountTab = tpIsWantedAccountTab;
    }
}
/*
login
*/
function tpToggleLoginPanel(e) {
    e.stopPropagation();
    e.preventDefault();
    if (isOpenLoginPanel) {
        closeLoginPanel(e);
    } else {
        tpOpenLoginPanel(e);
    }
    return false;
};
/*
*/
function tpOpenLoginPanel(e) {
	// load form server-side
	isLoadedLoginPanel=true;
    isOpenLoginPanel = true;
    if (!isLoadedLoginPanel) {
        setLoginWait(e);
        cj.ajax.addonCallback('toolPanelLoginForm', '', function (content, ignore) { tpStoreAndOpenLoginPanel(e, content) });
    } else {
        tpOpenLoginPanel_contentReady(e);
    }
}
/*
*/
function tpStoreAndOpenLoginPanel(e, content) {
    clearLoginWait(e);
    $('#tpLoginPanel').html(content);
    tpOpenLoginPanel_contentReady(e);
    isLoadedLoginPanel = true;
}
/*
*/
function tpOpenLoginPanel_contentReady(e) {
    tpCloseAccountPanel(e);
    $("#tpLoginTab").addClass("tpOpen");
    $("#tpLoginPanel").slideDown("slow");
}
/*
*/
function setLoginWait(e) {
    $('#tpLoginWait').css('visibility', 'visible');
}
/*
*/
function clearLoginWait(e) {
    $('#tpLoginWait').css('visibility', 'hidden');
}
/*
*/
function closeLoginPanel(e) {
    $("#tpLoginPanel").slideUp("slow");
    $("#tpLoginTab").removeClass("tpOpen");
    isOpenLoginPanel = false;
}
/*
*/
function clickLoginTabLock(e) {
    e.stopPropagation();
    e.preventDefault();
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
function mouseEnterLoginTab(e) {
    e.stopPropagation();
    e.preventDefault();
    if (!tpLoginTabIsPinned) {
        if (timerIdLoginTab != 0) {
            clearTimeout(timerIdLoginTab);
            timerIdLoginTab = 0;
        }
        tpAbortLoginTabClose = true;
    }
	return false;
}
/*
*/
function mouseLeaveLoginTab(e) {
    e.stopPropagation();
    e.preventDefault();
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
function tpLoginFormSubmit(e) {
    e.stopPropagation();
    e.preventDefault();
    var varString;
    if (validateLoginForm(e)) {
        //
        varString = 'panelLoginUsername=' + $('#panelLoginUsername').val();
        varString += '&panelLoginPassword=' + $('#panelLoginPassword').val();
        varString += '&panelLoginEmail=' + $('#panelLoginEmail').val();
        varString += '&panelLoginAuto=' + $('#panelLoginAuto').val();
        //
        cj.ajax.addonCallback('toolPanelLoginFormHandler', varString, tpRedirectHome, 'panelFormContainer');
        //
        tpSetSpinner('panelFormContainer', 'Authenticating Account....', $('#panelFormContainer').height());
    }
    return false;
}
/*
*/
function tpLoginFormKeypress(e) {
    if (e.which == 13) {
        return tpLoginFormSubmit(e);
    }
	return true;
}
/*
account form
*/
function validateAccountForm(e) {
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
function tpAccountSubmit(e) {
    e.stopPropagation();
    e.preventDefault();
	if (validateAccountForm(e)) {
		var varString;
		varString = 'panelAccountFirstName=' + $('#panelAccountFirstName').val();
		varString += '&panelAccountLastName=' + $('#panelAccountLastName').val();
		varString += '&panelAccountEmail=' + $('#panelAccountEmail').val();
		varString += '&panelAccountUsername=' + $('#panelAccountUsername').val();
		varString += '&panelAccountPassword=' + $('#panelAccountPassword').val();
		//
		cj.ajax.addon('toolPanelAccountFormHandler', varString, '', 'panelFormContainer', '', '');
		tpSetSpinner('panelFormContainer', 'Updating Account....', $('#panelFormContainer').height());
		tpCloseAccountPanel(e);
    }
    return false;
}
/*
*/
function tpAccountLogoutClick(e) {
    e.stopPropagation();
    e.preventDefault();
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
function tpEmailFormSubmit(e) {
    e.stopPropagation();
    e.preventDefault();
    if (validateEmailForm(e)) {
        cj.ajax.addon('toolPanelLoginFormHandler', '', 'panelLoginForm', 'panelFormContainer', '', '');
        tpSetSpinner('panelFormContainer', 'Gathering Account Information....', $('#panelFormContainer').height());
    }
    return false;
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
function tpRegisterSubmit(e) {
    e.stopPropagation();
    e.preventDefault();
    if (tpValidateRegisterForm(e)) {
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
    }
    return false;
}
/*
*/
function tpRequestRegister(e) {
    e.stopPropagation();
    e.preventDefault();
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
