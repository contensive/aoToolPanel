
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
  $('#'+divID).html(divContent);
  $('.spinner').height(containerHeight);
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
		$('#'+container).html(returnVal);
	}
} 
//
// --------------------------------
// zone to make tabs reappear
// --------------------------------
//
jQuery('#tpTabZone').mouseenter( function(){ tpMouseEnterTabZone() });
jQuery('#tpTabZone').mouseleave( function(){ tpMouseLeaveTabZone() });
//
function tpMouseEnterTabZone() {
	tpIsWantedAccountTab=true;
	$("#tpAccountTab").slideDown("slow");
	isWantedLoginTab=true;
	$("#tpLoginTab").slideDown("slow");
}
//
function tpMouseLeaveTabZone() {
	tpMouseLeaveAccountTab();
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
jQuery('#tpAccountLock').click( function (){ tpClickAccountTabLock() });
jQuery('#tpAccountTab').mouseenter( function(){ tpMouseEnterAccountTab() });
jQuery('#tpAccountTab').mouseleave( function(){ tpMouseLeaveAccountTab() });
jQuery( tpMouseLeaveAccountTab() );
$("#tpAccountButton").click(function () {
	if(tpIsOpenAccountPanel) {
		tpCloseAccountPanel();
	} else {
		tpOpenAccountPanel();
	}
	return false;
});
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
	$("#tpAccountTab").addClass("tpOpen");
	$("#tpAccountPanel").slideDown("slow");
}
function setAccountWait() {
	jQuery('#tpAccountWait').css('visibility','visible');
}
function tpClearAccountWait() {
	jQuery('#tpAccountWait').css('visibility','hidden');
}
function tpCloseAccountPanel () {
	$("#tpAccountPanel").slideUp("slow");
	$("#tpAccountTab").removeClass("tpOpen");
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
		$("#tpAccountTab").slideUp("slow");
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
var isLoadedLoginPanel=false;
var isOpenLoginPanel=false;
var isWantedLoginTab=false;
var timerIdLoginTab=0;
// 
jQuery('#tpLoginLock').click( function (){ clickLoginTabLock() });
jQuery('#tpLoginTab').mouseenter( function(){ mouseEnterLoginTab() });
jQuery('#tpLoginTab').mouseleave( function(){ mouseLeaveLoginTab() });
jQuery( mouseLeaveLoginTab() );
$("#tpLoginButton").click(function () {
	if(isOpenLoginPanel) {
		closeLoginPanel();
	} else {
		openLoginPanel();
	}
	return false;
});
function openLoginPanel() {
	isOpenLoginPanel = true;
	if (!isLoadedLoginPanel) {
		setLoginWait();
		cj.ajax.addonCallback('toolPanelLoginForm','',function(content,ignore){
			clearLoginWait();
			jQuery('#tpLoginPanel').html( content );
			openLoginPanelReady();
			isLoadedLoginPanel = true;
		});
	} else {
		openLoginPanelReady();
	}
}
function openLoginPanelReady() {
	tpCloseAccountPanel();
	$("#tpLoginTab").addClass("tpOpen");
	$("#tpLoginPanel").slideDown("slow");
}
function setLoginWait() {
	jQuery('#tpLoginWait').css('visibility','visible');
}
function clearLoginWait() {
	jQuery('#tpLoginWait').css('visibility','hidden');
}
function closeLoginPanel () {
	$("#tpLoginPanel").slideUp("slow");
	$("#tpLoginTab").removeClass("tpOpen");
	isOpenLoginPanel=false;
}
function clickLoginTabLock() {
	if (isLockedLoginTab) {
		//alert('unlock Login tab lock');
		jQuery('#tpLoginLock').html('<img src="/toolPanel/lockOpen.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=isLockedLoginTab&v=0');
		isLockedLoginTab=false;
	} else {
		//alert('lock Login tab lock');
		jQuery('#tpLoginLock').html('<img src="/toolPanel/lockClosed.png" height="10" width="10">');
		cj.ajax.addon('setUserProperty','n=isLockedLoginTab&v=1');
		isLockedLoginTab=true;
	}
	
}
function mouseEnterLoginTab() {
	if (!isLockedLoginTab)
	{
		if(timerIdLoginTab!=0)
		{
			clearTimeout( timerIdLoginTab );
			timerIdLoginTab=0;
		}
		isWantedLoginTab=true;
	}
}
function mouseLeaveLoginTab() {
	if ((!isLockedLoginTab) & (!isOpenLoginPanel))
	{
		if(timerIdLoginTab!=0)
		{
			clearTimeout( timerIdLoginTab );
			timerIdLoginTab=0;
		}
		timerIdLoginTab = setTimeout("LoginTabClose()",2000);
		isWantedLoginTab=false;
	}
}
function LoginTabClose() {
	if(!isWantedLoginTab)
	{
		$("#tpLoginTab").slideUp("slow");
		//alert('LoginTabClose');
	} else {
		isWantedLoginTab = isWantedLoginTab;
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
		var firstName = $('#panelAccountFirstName').val();
		var lastName = $('#panelAccountLastName').val();
		var email = $('#panelAccountEmail').val();
		//
		if (firstName==null || firstName=='')
		{
			$('#panelAccountFirstName').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelAccountFirstName').css('background-color', '#ffffff');
		};
		if (lastName==null || lastName=='')
		{
			$('#panelAccountLastName').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelAccountLastName').css('background-color', '#ffffff');
		};
		//
		if (email==null || email=='')
		{
			$('#panelAccountEmail').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelAccountEmail').css('background-color', '#ffffff');
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
		if (usernamePresent==1 && (username==null || username==''))
		{
			$('#panelLoginUsername').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelLoginUsername').css('background-color', '#ffffff');
		};
		if (passwordPresent==1 && (password==null || password==''))
		{
			$('#panelLoginPassword').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelLoginPassword').css('background-color', '#ffffff');
		};
		//
		if (emailPresent==1 && (email==null || email==''))
		{
			$('#panelLoginEmail').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelLoginEmail').css('background-color', '#ffffff');
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
		var email = $('#panelEmailEmail').val();
		//
		if (email==null || email=='')
		{
			$('#panelEmailEmail').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelEmailEmail').css('background-color', '#ffffff');
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
		var usernamePresent = $('#reqUsername').val();
		var passwordPresent = $('#reqPassword').val();
		//
		var firstName = $('#panelRegistrationFirstName').val();
		var lastName = $('#panelRegistrationLastName').val();
		var email = $('#panelRegistrationEmail').val();
		var username = $('#panelRegistrationUsername').val();
		var password = $('#panelRegistrationPassword').val();
		//
		if (firstName==null || firstName=='')
		{
			$('#panelRegistrationFirstName').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelRegistrationFirstName').css('background-color', '#ffffff');
		};
		if (lastName==null || lastName=='')
		{
			$('#panelRegistrationLastName').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelRegistrationLastName').css('background-color', '#ffffff');
		};
		//
		if (email==null || email=='')
		{
			$('#panelRegistrationEmail').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelRegistrationEmail').css('background-color', '#ffffff');
		};
		//
		if (email==null || email=='')
		{
			$('#panelRegistrationEmail').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelRegistrationEmail').css('background-color', '#ffffff');
		};
		//
		if (usernamePresent==1 && (username==null || username==''))
		{
			$('#panelRegistrationUsername').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelRegistrationUsername').css('background-color', '#ffffff');
		};
		if (passwordPresent==1 && (password==null || password==''))
		{
			$('#panelRegistrationPassword').css('background-color', '#dddddd');
			errFlag = 1;
		}
		else
		{
			$('#panelRegistrationPassword').css('background-color', '#ffffff');
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








