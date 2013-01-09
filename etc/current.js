
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
function tbBindEvents(){
	//
	// bind all events from all forms
	//
	$(document).ready(function(){
		//
		jQuery('#panelLoginPassword').keypress( function(e){ tpLoginFormKeypress(e) });
		jQuery('#panelLoginEmail').keypress( function(e){ tpLoginFormKeypress(e) });
		jQuery('#panelLoginUsername').keypress( function(e){ tpLoginFormKeypress(e) });
		jQuery('#tpTabZone').mouseenter( function(){ tpMouseEnterTabZone() });
		jQuery('#tpTabZone').mouseleave( function(){ tpMouseLeaveTabZone() });
		jQuery('#tpAccountLock').click( function (){ tpClickAccountTabLock() });
		jQuery('#tpAccountTab').mouseenter( function(){ tpMouseEnterAccountTab() });
		jQuery('#tpAccountTab').mouseleave( function(){ tpMouseLeaveAccountTab() });
		jQuery("#tpAccountButton").click( function(){ tpToggleAccountPanel(); return false; });
		jQuery('#tpLoginLock').click( function (){ clickLoginTabLock() });
		jQuery('#tpLoginTab').mouseenter( function(){ mouseEnterLoginTab() });
		jQuery('#tpLoginTab').mouseleave( function(){ mouseLeaveLoginTab() });
		jQuery("#tpLoginButton").click( function(){ tpToggleLoginPanel(); return false; });
		jQuery('#loginSubmit').click( function() { tpLoginFormSubmit(); return false; });
		jQuery('#emailSubmit').click(function(){
			if (!validateEmailForm()) {
				return false;
			} else {
				cj.ajax.addon('toolPanelLoginFormHandler', '', 'panelLoginForm', 'panelFormContainer', '', '');
				tpSetSpinner('panelFormContainer', 'Gathering Account Information....', jQuery('#panelFormContainer').height());
			}
		});
		jQuery('#registrationClick').click(function(){
			cj.ajax.addon('toolPanelDefaultRegistrationForm','', '', 'panelFormContainer', '', '');
			tpSetSpinner('panelFormContainer', 'Building Registration Form....', jQuery('#panelFormContainer').height());
		});
		//
		return false;
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
 	if (testString=='')
        {
		window.location = document.head.baseURI;
	}
	else{
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
		//alert('unlock account tab lock');
		jQuery('#tpAccountLock').html('<img src="/toolPanel/lockOpen.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=isLockedAccountTab&v=0');
		isLockedAccountTab=false;
	} else {
		//alert('lock account tab lock');
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
		//alert('accountTabClose');
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
		//alert('unlock Login tab lock');
		jQuery('#tpLoginLock').html('<img src="/toolPanel/lockOpen.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=tpLoginTabIsPinned&v=0');
		tpLoginTabIsPinned=false;
	} else {
		//alert('lock Login tab lock');
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
		//alert('LoginTabClose');
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
//
// --------------------------------
// Registration Form
// --------------------------------
//
function validateRegistrationForm() {
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
		};
};
//
// --------------------------------
// initialize
// --------------------------------
//
tbBindEvents();
jQuery( tpMouseLeaveAccountTab() );
jQuery( mouseLeaveLoginTab() );
