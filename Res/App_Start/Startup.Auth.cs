﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Res.Business;

namespace Res
{
	public partial class Startup
	{
		// 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// 配置数据库上下文、用户管理器和登录管理器，以便为每个请求使用单个实例
			//app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

			// 使应用程序可以使用 Cookie 来存储已登录用户的信息
			// 并使用 Cookie 来临时存储有关使用第三方登录提供程序登录的用户的信息
			// 配置登录 Cookie
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,

				LoginPath = new PathString("/"+ThisApp.ProjectKey + "/Account/Login"),
				Provider = new CookieAuthenticationProvider
				{
					// 当用户登录时使应用程序可以验证安全戳。
					// 这是一项安全功能，当你更改密码或者向帐户添加外部登录名时，将使用此功能。
					//OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, MyUser, long>(
					//	 validateInterval: TimeSpan.FromMinutes(30),
					//	 regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
				},
				// 为防止多个项目 application cookie 发生干扰，所以采用独立的cookiename 来解决问题
				CookieName = Business.ThisApp.DomainCookie
			});

		}
	}
}