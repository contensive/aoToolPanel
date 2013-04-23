
//
// --------------------------------
// properties
// --------------------------------
//
var tpLoginTabIsPinned=false;
var isLockedAccountTab=false;
var isLoadedLoginPanel=false;
var isOpenLoginPanel=false;
var tpAbortLoginTabClose=false;
var timerIdLoginTab=0;
// 
//
// --------------------------------
// Bind all events
//	called from both the page and ajax content updates
// --------------------------------
//
function tpBindEvents(){
	//
	// bind all events from all forms
	//
	$(document).ready(function(){
		//
		jQuery('#panelLoginPassword').unbind('keypress').keypress( function(e){ tpLoginFormKeypress(e) });
		jQuery('#panelLoginEmail').unbind('keypress').keypress( function(e){ tpLoginFormKeypress(e) });
		jQuery('#panelLoginUsername').unbind('keypress').keypress( function(e){ tpLoginFormKeypress(e) });
		jQuery('#tpTabZone').unbind('mouseenter').mouseenter( tpMouseEnterTabZone );
		jQuery('#tpTabZone').unbind('mouseleave').mouseleave( tpMouseLeaveTabZone );
		jQuery('#tpAccountLock').unbind('click').click( tpClickAccountTabLock );
		jQuery('#tpAccountTab').unbind('mouseenter').mouseenter( tpMouseEnterAccountTab );
		jQuery('#tpAccountTab').unbind('mouseleave').mouseleave( tpMouseLeaveAccountTab );
		jQuery("#tpAccountButton").unbind('click').click( tpToggleAccountPanel );
		jQuery('#tpLoginLock').unbind('click').click( clickLoginTabLock );
		jQuery('#tpLoginTab').unbind('mouseenter').mouseenter( mouseEnterLoginTab );
		jQuery('#tpLoginTab').unbind('mouseleave').mouseleave( mouseLeaveLoginTab );
		jQuery("#tpLoginButton").unbind('click').click( tpToggleLoginPanel );
		jQuery('#tpLoginFormSubmit').unbind('click').click( tpLoginFormSubmit );
		jQuery('#tpEmailFormSubmit').unbind('click').click( tpEmailFormSubmit );
		jQuery('#tpLoginFormRegister').unbind('click').click( tpRequestRegister );
		jQuery('#accountSubmit').unbind('click').click( tpAccountSubmit );
		jQuery('#logoutClick').unbind('click').click( tpAccountLogoutClick );
		jQuery('#tpRegisterSubmit').unbind('click').click( tpRegisterSubmit );
	})
}
//
// --------------------------------
// Utilities
// --------------------------------
//
function tpSetSpinner(divID, message, containerHeight){
  var divContent;
  //
  divContent = '<div class="spinner"><img src="/images/transpin.gif" /><p>' + message + '</p></div>';
  //
  jQuery('#'+divID).html(divContent);
  jQuery('.spinner').height(containerHeight);
}

function tpGetParameterByName( name )
{
  name = name.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
  var regexS = "[\\?&]"+name+"=([^&#]*)";
  var regex = new RegExp( regexS );
  var results = regex.exec( window.location.href );
  if( results == null )
    return "";
  else
    return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function tpRedirectHome(returnVal, container){
	var testString = returnVal;
	//
	testString = testString.replace(/(\r\n|\n|\r)/gm,' ').replace(/(\s)/g, '');
	//
 	if (testString=='') {
		window.location = document.head.baseURI;
	} else {
		jQuery('#'+container).html(returnVal);
	}
} 
//
// --------------------------------
// zone to make tabs reappear
// --------------------------------
//
//
function tpMouseEnterTabZone() {
	tpIsWantedAccountTab=true;
	jQuery("#tpAccountTab").slideDown("slow");
	tpAbortLoginTabClose=true;
	jQuery("#tpLoginTab").slideDown("slow");
}
//
function tpMouseLeaveTabZone() {
	tpMouseLeaveAccountTab();
	mouseLeaveLoginTab();
}
//
// --------------------------------
// account tab 
// --------------------------------
//
var tpIsLoadedAccountPanel=false;
var tpIsOpenAccountPanel=false;
var tpIsWantedAccountTab=false;
var tpTimerIdAccountTab=0;
//
function tpToggleAccountPanel() {
	if(tpIsOpenAccountPanel) {
		tpCloseAccountPanel();
	} else {
		tpOpenAccountPanel();
	}
	return false;
}
function tpOpenAccountPanel() {
	tpIsOpenAccountPanel = true;
	if (!tpIsLoadedAccountPanel) {
		setAccountWait();
		cj.ajax.addonCallback('toolPanelAccountForm','',function(content,ignore){
			tpClearAccountWait();
			jQuery('#tpAccountPanel').html( content );
			tpOpenAccountPanelReady();
			tpIsLoadedAccountPanel = true;
		});
	} else {
		tpOpenAccountPanelReady();
	}
}
function tpOpenAccountPanelReady() {
	closeLoginPanel();
	jQuery("#tpAccountTab").addClass("tpOpen");
	jQuery("#tpAccountPanel").slideDown("slow");
}
function setAccountWait() {
	jQuery('#tpAccountWait').css('visibility','visible');
}
function tpClearAccountWait() {
	jQuery('#tpAccountWait').css('visibility','hidden');
}
function tpCloseAccountPanel () {
	jQuery("#tpAccountPanel").slideUp("slow");
	jQuery("#tpAccountTab").removeClass("tpOpen");
	tpIsOpenAccountPanel=false;
}
function tpClickAccountTabLock() {
	if (isLockedAccountTab) {
		jQuery('#tpAccountLock').html('<img src="/toolPanel/lockOpen.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=isLockedAccountTab&v=0');
		isLockedAccountTab=false;
	} else {
		jQuery('#tpAccountLock').html('<img src="/toolPanel/lockClosed.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=isLockedAccountTab&v=1');
		isLockedAccountTab=true;
	}
	
}
function tpMouseEnterAccountTab() {
	if (!isLockedAccountTab)
	{
		if(tpTimerIdAccountTab!=0)
		{
			clearTimeout( tpTimerIdAccountTab );
			tpTimerIdAccountTab=0;
		}
		tpIsWantedAccountTab=true;
	}
}
function tpMouseLeaveAccountTab() {
	if ((!isLockedAccountTab) & (!tpIsOpenAccountPanel))
	{
		if(tpTimerIdAccountTab!=0)
		{
			clearTimeout( tpTimerIdAccountTab );
			tpTimerIdAccountTab=0;
		}
		tpTimerIdAccountTab = setTimeout("accountTabClose()",2000);
		tpIsWantedAccountTab=false;
	}
}
function accountTabClose() {
	if(!tpIsWantedAccountTab)
	{
		jQuery("#tpAccountTab").slideUp("slow");
	} else {
		tpIsWantedAccountTab = tpIsWantedAccountTab;
	}
}
//
// --------------------------------
// login tab
// --------------------------------
//
function tpToggleLoginPanel() {
	if(isOpenLoginPanel) {
		closeLoginPanel();
	} else {
		tpOpenLoginPanel();
	}
	return false;
};
function tpOpenLoginPanel() {
	isOpenLoginPanel = true;
	if (!isLoadedLoginPanel) {
		setLoginWait();
		cj.ajax.addonCallback('toolPanelLoginForm','',function(content,ignore){ tpStoreAndOpenLoginPanel( content ) });
	} else {
		tpOpenLoginPanel_contentReady();
	}
}
function tpStoreAndOpenLoginPanel( content ) {
	clearLoginWait();
	jQuery('#tpLoginPanel').html( content );
	tpOpenLoginPanel_contentReady();
	isLoadedLoginPanel = true;
}
function tpOpenLoginPanel_contentReady() {
	tpCloseAccountPanel();
	jQuery("#tpLoginTab").addClass("tpOpen");
	jQuery("#tpLoginPanel").slideDown("slow");
}
function setLoginWait() {
	jQuery('#tpLoginWait').css('visibility','visible');
}
function clearLoginWait() {
	jQuery('#tpLoginWait').css('visibility','hidden');
}
function closeLoginPanel () {
	jQuery("#tpLoginPanel").slideUp("slow");
	jQuery("#tpLoginTab").removeClass("tpOpen");
	isOpenLoginPanel=false;
}
function clickLoginTabLock() {
	if (tpLoginTabIsPinned) {
		jQuery('#tpLoginLock').html('<img src="/toolPanel/lockOpen.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=tpLoginTabIsPinned&v=0');
		tpLoginTabIsPinned=false;
	} else {
		jQuery('#tpLoginLock').html('<img src="/toolPanel/lockClosed.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=tpLoginTabIsPinned&v=1');
		tpLoginTabIsPinned=true;
	}
	
}
function mouseEnterLoginTab() {
	if (!tpLoginTabIsPinned)
	{
		if(timerIdLoginTab!=0)
		{
			clearTimeout( timerIdLoginTab );
			timerIdLoginTab=0;
		}
		tpAbortLoginTabClose=true;
	}
}
function mouseLeaveLoginTab() {
	if ((!tpLoginTabIsPinned) & (!isOpenLoginPanel))
	{
		if(timerIdLoginTab!=0)
		{
			clearTimeout( timerIdLoginTab );
			timerIdLoginTab=0;
		}
		timerIdLoginTab = setTimeout("LoginTabClose()",2000);
		tpAbortLoginTabClose=false;
	}
}
function LoginTabClose() {
	if(!tpAbortLoginTabClose)
	{
		jQuery("#tpLoginTab").slideUp("slow");
	} else {
		tpAbortLoginTabClose = tpAbortLoginTabClose;
	}
}
function tpLoginFormSubmit(){
	var varString;
	if (!validateLoginForm()) {
		return false;
	} else {
		//
		varString = 'panelLoginUsername='+jQuery('#panelLoginUsername').val();
		varString += '&panelLoginPassword='+jQuery('#panelLoginPassword').val();
		varString += '&panelLoginEmail='+jQuery('#panelLoginEmail').val();
		varString += '&panelLoginAuto='+jQuery('#panelLoginAuto').val();
		//
		cj.ajax.addonCallback('toolPanelLoginFormHandler', varString, tpRedirectHome, 'panelFormContainer');
		//
		tpSetSpinner('panelFormContainer', 'Authenticating Account....', jQuery('#panelFormContainer').height());
		return false;
	}
}
function tpLoginFormKeypress(e){
	if(e.which == 13){
		tpLoginFormSubmit();
		e.preventDefault();
		return false;
	}
}
//
// --------------------------------
// Account Form
// --------------------------------
//
function validateAccountForm() {
		var errFlag;
		var errMsg;
		//
		var firstName = jQuery('#panelAccountFirstName').val();
		var lastName = jQuery('#panelAccountLastName').val();
		var email = jQuery('#panelAccountEmail').val();
		//
		if (firstName==null || firstName=='')
		{
			jQuery('#panelAccountFirstName').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelAccountFirstName').css('background-color', '#ffffff');
		};
		if (lastName==null || lastName=='')
		{
			jQuery('#panelAccountLastName').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelAccountLastName').css('background-color', '#ffffff');
		};
		//
		if (email==null || email=='')
		{
			jQuery('#panelAccountEmail').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelAccountEmail').css('background-color', '#ffffff');
		};
		//
		if (errFlag==1) {
			if (errMsg=='' || errMsg==null || errMsg=='undefined')
			{
				errMsg = 'Please complete all required fields in order to continue.';
			}
			alert(errMsg);
			return false;
		}
		else
		{
			return true;
		};
};
function tpAccountSubmit(){
	if (!validateAccountForm()) {
		return false;
	}
	var varString;
	varString = 'panelAccountFirstName=' + jQuery('#panelAccountFirstName').val();
	varString += '&panelAccountLastName=' + jQuery('#panelAccountLastName').val();
	varString += '&panelAccountEmail=' + jQuery('#panelAccountEmail').val();
	varString += '&panelAccountUsername=' + jQuery('#panelAccountUsername').val();
	varString += '&panelAccountPassword=' + jQuery('#panelAccountPassword').val();
	//
	cj.ajax.addon('toolPanelAccountFormHandler', varString, '', 'panelFormContainer', '','');
	tpSetSpinner('panelFormContainer', 'Updating Account....', jQuery('#panelFormContainer').height());
	return false;
}
function tpAccountLogoutClick(){
	cj.ajax.addonCallback('toolPanelLogoutHandler', '', tpRedirectHome, 'panelFormContainer');
	tpSetSpinner('panelFormContainer', 'Logging Out....', jQuery('#panelFormContainer').height());
	return false;
}

//
// --------------------------------
// Login Form
// --------------------------------
//
function validateLoginForm() {
		var errFlag;
		var errMsg;
		//
		var username = jQuery('#panelLoginUsername').val();
		var password = jQuery('#panelLoginPassword').val();
		var email = jQuery('#panelLoginEmail').val();
		//
		// fields present
		//
		var usernamePresent = jQuery('#reqUsername').val();
		var passwordPresent = jQuery('#reqPassword').val();
		var emailPresent = jQuery('#reqEmail').val();
		//
		if (passwordPresent==1 && (password==null || password==''))
		{
			jQuery('#panelLoginPassword').css('background-color', '#f0f890').focus();
			errFlag = 1;
		}
		else
		{
			jQuery('#panelLoginPassword').css('background-color', '#ffffff');
		}
		//
		if (usernamePresent==1 && (username==null || username==''))
		{
			jQuery('#panelLoginUsername').css('background-color', '#f0f890').focus();
			errFlag = 1;
		}
		else
		{
			jQuery('#panelLoginUsername').css('background-color', '#ffffff');
		}
		//
		if (emailPresent==1 && (email==null || email==''))
		{
			jQuery('#panelLoginEmail').css('background-color', '#f0f890').focus();
			errFlag = 1;
		}
		else
		{
			jQuery('#panelLoginEmail').css('background-color', '#ffffff');
		};
		//
		if (errFlag==1) {
			if (errMsg=='' || errMsg==null || errMsg=='undefined')
			{
				errMsg = 'Please complete all fields in order to login.';
			}
			alert(errMsg);
			return false;
		}
		else
		{
			return true;
		};
};
//
// --------------------------------
// Send Password Form
// --------------------------------
//
function validateEmailForm() {
		var errFlag;
		var errMsg;
		//
		var email = jQuery('#panelEmailEmail').val();
		//
		if (email==null || email=='')
		{
			jQuery('#panelEmailEmail').css('background-color', '#f0f890');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelEmailEmail').css('background-color', '#ffffff');
		};
		//
		if (errFlag==1) {
			if (errMsg=='' || errMsg==null || errMsg=='undefined')
			{
				errMsg = 'Please enter your email address in order to retrieve your login information.';
			}
			alert(errMsg);
			return false;
		}
		else
		{
			return true;
		};
};
function tpEmailFormSubmit() {
	if (!validateEmailForm()) {
		return false;
	} else {
		cj.ajax.addon('toolPanelLoginFormHandler', '', 'panelLoginForm', 'panelFormContainer', '', '');
		tpSetSpinner('panelFormContainer', 'Gathering Account Information....', jQuery('#panelFormContainer').height());
	}
}
//
// --------------------------------
// Registration Form
// --------------------------------
//
function tpValidateRegisterForm() {
		var errFlag;
		var errMsg;
		//
		var usernamePresent = jQuery('#reqUsername').val();
		var passwordPresent = jQuery('#reqPassword').val();
		//
		var firstName = jQuery('#panelRegistrationFirstName').val();
		var lastName = jQuery('#panelRegistrationLastName').val();
		var email = jQuery('#panelRegistrationEmail').val();
		var username = jQuery('#panelRegistrationUsername').val();
		var password = jQuery('#panelRegistrationPassword').val();
		//
		if (firstName==null || firstName=='')
		{
			jQuery('#panelRegistrationFirstName').css('background-color', '#f0f890');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelRegistrationFirstName').css('background-color', '#ffffff');
		};
		if (lastName==null || lastName=='')
		{
			jQuery('#panelRegistrationLastName').css('background-color', '#f0f890');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelRegistrationLastName').css('background-color', '#ffffff');
		};
		//
		if (email==null || email=='')
		{
			jQuery('#panelRegistrationEmail').css('background-color', '#f0f890');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelRegistrationEmail').css('background-color', '#ffffff');
		};
		//
		if (email==null || email=='')
		{
			jQuery('#panelRegistrationEmail').css('background-color', '#f0f890');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelRegistrationEmail').css('background-color', '#ffffff');
		};
		//
		if (usernamePresent==1 && (username==null || username==''))
		{
			jQuery('#panelRegistrationUsername').css('background-color', '#f0f890');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelRegistrationUsername').css('background-color', '#ffffff');
		};
		if (passwordPresent==1 && (password==null || password==''))
		{
			jQuery('#panelRegistrationPassword').css('background-color', '#f0f890');
			errFlag = 1;
		}
		else
		{
			jQuery('#panelRegistrationPassword').css('background-color', '#ffffff');
		};
		//
		if (errFlag==1) {
			if (errMsg=='' || errMsg==null || errMsg=='undefined')
			{
				errMsg = 'Please complete all required fields in order to continue.';
			}
			alert(errMsg);
			return false;
		}
		else
		{
			return true;
		}
}
function tpRegisterSubmit(){
	if (!tpValidateRegisterForm()) {
		return false;
	}
	var varString;
	varString = 'panelRegistrationFirstName=' + jQuery('#panelRegistrationFirstName').val();
	varString += '&panelRegistrationLastName=' + jQuery('#panelRegistrationLastName').val();
	varString += '&panelRegistrationEmail=' + jQuery('#panelRegistrationEmail').val();
	varString += '&panelRegistrationUsername=' + jQuery('#panelRegistrationUsername').val();
	varString += '&panelRegistrationPassword=' + jQuery('#panelRegistrationPassword').val();
	//
	cj.ajax.addonCallback('toolPanelRegistrationFormHandler', varString, tpRedirectHome, 'panelFormContainer');
	//cj.ajax.addon('toolPanelRegistrationFormHandler', varString, '', 'panelFormContainer', '','');
	tpSetSpinner('panelFormContainer', 'Creating Account...', jQuery('#panelFormContainer').height());
	return false;
}
function tpRequestRegister(){
	cj.ajax.addon('toolPanelDefaultRegistrationForm','', '', 'panelFormContainer', '', '');
	tpSetSpinner('panelFormContainer', '', jQuery('#panelFormContainer').height());
	return false;
}
//
// --------------------------------
// initialize
// --------------------------------
//
tpBindEvents();
jQuery( tpMouseLeaveAccountTab() );
jQuery( mouseLeaveLoginTab() );
