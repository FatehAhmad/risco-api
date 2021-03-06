using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using static BasketApi.Utility;
using static DAL.CustomModels.Enumeration;


namespace WebApplication1.EmailTemplates
{
    /*
    public static class Templates
    {
        public static string GetEmailTemplate(EmailTypes Type, int EntityId, string VerificationCode = "", string lastLogin = "", string Description = "")
        {
            var html = string.Empty;
            using (RiscoContext ctx = new RiscoContext())
            {
                User user = new User();
                switch (Type)
                {
                    case EmailTypes.ResetPassword:
                        #region ResetPasswordHtml

                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px;background:#fff;margin:0auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/mVS4X9/email_banner.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Forgot Password</h2>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear [VAR_NAME]</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>We have received a request for the password reset. Click on the given link to set the new password. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<a style='border-radius: 5px;padding: 6px 20px;background: #1bace2;color: #fff;font-size: 14px;border: 1px solid transparent;display: inline-block;text-decoration: none;' href='#'>Set a new password</a>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Note: If you do not want to reset the password, kindly ignore the email. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Team Risco</p>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";
                        #endregion
                        break;
                    case EmailTypes.Welcome:
                        user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region WelcomeHtml
                        html = @"
<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background:#fff;margin:0 auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/b9Fiwe/welcome.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Welcome Email</h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Hi,</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>" + user.FullName + @",</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Welcome to Risco</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Congratulations on joining the Risco!</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Risco is all about the latest happenings and events near you. By joining Risco you will never be out of the loop. We hope that you will have the best experience with Risco. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Risco </p>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";
                        #endregion
                        break;
                    case EmailTypes.ForgetPassword:
                        user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region ForgetPasswordHtml
                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background:#fff;margin:0 auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/joGOwe/banner2.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Forgot Password</h2>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear " + user.FullName + @"</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>We have received a request for the password reset. Click on the given link to set the new password. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<a style='border-radius: 5px;padding: 6px 20px;background: #1bace2;color: #fff;font-size: 14px;border: 1px solid transparent;display: inline-block;text-decoration: none;' href='https://risco.ingicweb.com/#/reset-password?userId=" + user.Id+ @">Set a new password</a>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Note: If you do not want to reset the password, kindly ignore the email. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Team Risco</p>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";
                        #endregion
                        break;
                    case EmailTypes.PhoneVerification:
                        #region PhoneVerificationHtml
                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background:#fff;margin:0 auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/mVS4X9/email_banner.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Phone Verification</h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Welcome to Risco</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Thank you for signing up.</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Kindly enter the following code to verify your phone number. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>" + VerificationCode + @"</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>If you have any trouble kindly email us at <a href='mailto:care@risco.co'>care@risco.co</a></p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Team Risco </p>
		</td>
	</tr>

	<tr>
			<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
	<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>
";
                        #endregion
                        break;
                    case EmailTypes.UpdateProfile:
                        user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region UpdateProfileHtml
                        html = @"
<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background:#fff; margin:0 auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/gUck9z/banner3.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Profile Update</h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear " + user.FullName + @"</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Congratulations!</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Your profile has been updated. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Click on the below link to view your profile or simply log in to the app. </p>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Team  Risco </p>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";
                        #endregion
                        break;
                    case EmailTypes.AccountSuspensionWarning:
                        //user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region AccountSuspensionWarningHtml
                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background: #fff; margin:0 auto; font-family: Arial, Tahoma, 'Lucida Sans';' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/juZZ6e/warning.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Account Suspension Warning</h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear User,</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Our system has observed that your ID has been blocked by several users on Risco. We recommend that you go through our privacy policy and terms and conditions in order to figure out where you are going wrong. Failing to comply with the rules, and getting blocked by more users (a total of 10users) may lead your account towards suspension. The temporary suspension lasts 30 days while a permanent will lead to account termination.</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards,</p>
		</td>
	</tr>
	

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='images/fb-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='images/google-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='images/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='images/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='images/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";
                        #endregion
                        break;
                    case EmailTypes.AccountSuspended:
                        user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region AccountSuspendedHtml
                        html = @"
<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background: #fff; margin:0 auto; font-family: Arial, Tahoma, 'Lucida Sans';' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/g3HnRe/Suspended.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Account Suspension</h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear User,</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>As per our previous warning regarding a suspicious behavior/activity on our platform from your account, we have suspended your account on failing to comply with the community rules. This suspension will last for 30 days. After 30 days, your account will be automatically re-activated. This suspension has been put because 10 Risco users blocked your profile in recent times.</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards,</p>
		</td>
	</tr>
	

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='images/fb-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='images/google-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='images/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='images/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='images/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";
                        #endregion
                        break;
                    case EmailTypes.PostDeletionWarning:
                        user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region PostDeletetionWarningHtml
                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background: #fff; margin:0 auto; font-family: Arial, Tahoma, 'Lucida Sans';' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/cn6QCK/postwarning.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Post Deletion Warning</h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear User,</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Our system has observed that one of your posts have been blocked by several users on Risco. We recommend that you go through our privacy policy and terms and conditions in order to figure out where you are going wrong. Failing to comply with the rules, and getting more flags on the post (by a total of 10 users) may require us to delete the post permanently. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards,</p>
		</td>
	</tr>
	

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='images/fb-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='images/google-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='images/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='images/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='images/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";
                        #endregion
                        break;
                    case EmailTypes.PostDeleted:
                        user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region PostDeletedHtml

                        html = @"
<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background: #fff; margin:0 auto; font-family: Arial, Tahoma, 'Lucida Sans';' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/exVVez/postdeletion.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Post Deletion </h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear User,</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>As per our previous warning email, one of your posts were violating our community guidelines and was flagged by 10 users. As a result, the post has been deleted.</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards,</p>
		</td>
	</tr>
	

	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='images/fb-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='images/google-icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='images/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='images/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='images/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
		<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>";

                        #endregion
                        break;
                    case EmailTypes.NewLogin:
                        user = ctx.Users.FirstOrDefault(x => x.Id == EntityId);
                        #region LoginFromNewDevice

                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background:#fff;margin:0 auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>
	<tr>
		<td colspan='2' style='text-align: center; padding-top: 35px;border-bottom: solid 1px #dadada;padding-bottom: 30px;'>
			<a href='/'><img src='https://image.ibb.co/kmNhKp/logo.png'></a>
		</td>
	</tr>

	<tr>
		<td colspan='2' style='padding-top: 0;text-align: center;'>
			<img src='https://image.ibb.co/gUck9z/banner3.jpg' alt=''>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Device Verification</h2>
		</td>
	</tr>



	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Dear " + user.FullName + @" </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Your Risco account was signed-in from another device from unknown location on " + String.Format("{0:f}", user.LastLoginTime) + @" UTC.</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>If it is you, kindly ignore the email.  </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>For any unusual activity email us at <a href='mailto:info@risco.co' target='_top'>info@risco.co </a>  to report. </p>
               </td>
	</tr>

	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Team Risco </p>
		</td>
	</tr>

	<tr>
			<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='https://www.facebook.com/Risco-306763169879879/' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://plus.google.com/b/118328344691801542740/118328344691801542740' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.instagram.com/risco7/' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://www.pinterest.com/officialrisco/' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='https://twitter.com/Risco62426670' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
	<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2018 Risco , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>
";
                        #endregion
                        break;
                    case EmailTypes.EmailVerification:
                        #region EmailVerificationHtml
                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background:#fff;margin:0 auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>	
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Email Verification</h2>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Welcome to Risco</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Thank you for signing up.</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Kindly enter the following code to verify your email account. </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>" + VerificationCode + @"</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>If you have any trouble kindly email us at <a href='mailto:riscoweb1@gmail.com'>riscoweb1@gmail.com</a></p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Team Risco </p>
		</td>
	</tr>

	<tr>
			<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
	<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2019 Tobdon , All right reserved.</p></td>
	</tr>
</table>



</body>
</html>
";
                        #endregion
                        break;
                    case EmailTypes.General:
                        #region General
                        html = @"<html>
<head>
<meta charset='utf-8'>
<title>Risco</title>
</head>

<body style='background: #1bace2;'>
<table style='width:600px; background:#fff;margin:0 auto;font-family:Arial,Tahoma;' border='0' cellspacing='0' cellpadding='0' align='center'>	
	<tr>
		<td colspan='2' style='padding: 20px 0 20px 26px;'>
			<h2 style='margin: 0;font-size: 20px;color: #1b0303;font-weight: 500; letter-spacing:1px;'>Risco</h2>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>" + Description + @"</p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>If you have any trouble kindly email us at <a href='mailto:riscoweb1@gmail.com'>riscoweb1@gmail.com</a></p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Regards, </p>
		</td>
	</tr>
	<tr>
		<td colspan='2' style='text-align: left;padding: 0 25px 25px 25px;'>
			<p style='color: #666666;font-size: 15px;margin: 0;'>Team Risco </p>
		</td>
	</tr>

	<tr>
			<td colspan='2' style='text-align: center;padding:0 25px 0px 25px;'>
			<ul style='margin: 0;padding: 0 0 20px 0;'>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/eni05U/fb_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/gGdBC9/google_icon.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/b1uwep/instagram.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/i1e7kU/pin.png' alt=''></a></li>
				<li style='display: inline-block'><a href='#' target='_blank'><img src='https://image.ibb.co/iPZJs9/twiter.png' alt=''></a></li>
			</ul>
		</td>
	
	</tr>
	<tr>
	<td colspan='2' style='text-align: center;padding:0 25px 25px 25px;'>
			<a href='#'><img src='https://image.ibb.co/jA19zp/app_store.png' alt=''></a>
			<a href='#'><img src='https://image.ibb.co/gtLDQU/g_play.png' alt=''></a>
		</td>
	</tr>
	<tr>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;background: #ff211f;'><a style='font-size: 12px;color: #fff;text-decoration: none;font-weight: 600;' href='#'>Unsubscribe from this list</a></td>
		<td style='padding:12px 15px;border-bottom: 1px solid #e5e5e5;text-align: right;background: #ff211f;'><p style='margin: 0;font-size: 12px;color: #fff;'>© 2019 Tobdon , All right reserved.</p></td>
	</tr>
</table>



</body>
</html> 
                            ";
                        #endregion
                        break;
                    default:
                        break;
                }
                return html;
            }
        }

    }
    */
}