﻿using Res.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Symber.Web.Data;

namespace Res.Controllers
{

	[Authorize]
	public class BaseController : Controller
	{
		public BaseController()
		{
			db = new APDBDef();

         //CurrentActive = ResSettings.SettingsInSession.CurrentActive;
      }

     // protected Active CurrentActive { get; private set; }

		protected APDBDef db { get; set; }

		protected override void Dispose(bool disposing)
		{
			if (db != null)
				db.Dispose();
			base.Dispose(disposing);
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			// 标记异常已处理
			filterContext.ExceptionHandled = true;
			// 跳转到错误页
			filterContext.Result = View("Error", filterContext.Exception); //RedirectToAction("Error //RedirectResult(Url.Action("Error", "Shared"));
		}

		/// <summary>
		/// Not Ajax call.
		/// </summary>
		/// <returns></returns>
		protected ActionResult IsNotAjax()
		{
			return Content("Is Not Ajax.");
		}


		protected void ThrowNotAjax()
		{
			if (!Request.IsAjaxRequest())
				throw new NotSupportedException("Action must be Ajax call.");
		}


		public List<CroMyResource> CrolistComment(string searchPhrase, string Audittype, out int total, int take, int skip = 0)
		{
			var t = APDBDef.CroResource;
			var t1 = APDBDef.CroComment;

			var query = APQuery.select(t.CrosourceId, t.Title, t.Author, t.Description, t1.OccurTime, t1.OccurId, t1.Content, t1.Audittype, t1.Auditor, t1.AuditedTime, t1.Audittype)
				.from(t, t1.JoinInner(t.CrosourceId == t1.ResourceId))
				.order_by(t1.OccurTime.Desc)
				.primary(t.CrosourceId)
				.take(take)
				.skip(skip);

			if (Audittype != null & Audittype != "")
			{
				query.where_and(t1.Audittype == Int64.Parse(Audittype));
			}
			// 按资源标题过滤
			if (searchPhrase != null)
			{
				searchPhrase = searchPhrase.Trim();
				if (searchPhrase != "")

					query.where_and(t.Title.Match(searchPhrase) | t1.Content.Match(searchPhrase));
			}

         var user = ResSettings.SettingsInSession.User;
         if (user.ProvinceId > 0)
            query.where_and(t.ProvinceId == user.ProvinceId);
         if (user.AreaId > 0)
            query.where_and(t.AreaId == user.AreaId);
         if (user.CompanyId > 0)
            query.where_and(t.CompanyId == user.CompanyId);

         total = db.ExecuteSizeOfSelect(query);

			return db.Query(query, reader =>
			{
				var des = t.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);
				return new CroMyResource()
				{
					CrosourceId = t.CrosourceId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Author = t.Author.GetValue(reader),
					//CoverPath = t.CoverPath.GetValue(reader),
					//FileExtName = t.FileExtName.GetValue(reader),
					Description = des,
					OccurId = t1.OccurId.GetValue(reader),
					Content = t1.Content.GetValue(reader),
					Auditor = t1.Auditor.GetValue(reader),
					Audittype = t1.Audittype.GetValue(reader),
					AuditedTime = t1.AuditedTime.GetValue(reader),
					
				};
			}).ToList();
		}


      /// <summary>
      /// Intial area drop down data
      /// </summary>
      protected void InitAreaDropDownData(bool filterByuser=false)
      {
         //删除单位的缓存信息
         ResSettings.SettingsInSession.RemoveCache(typeof(List<ResCompany>));

         var user = ResSettings.SettingsInSession.User;

         var provinces = ResSettings.SettingsInSession.AllProvince();
         var areas = ResSettings.SettingsInSession.AllAreas();
         var schools = ResSettings.SettingsInSession.AllSchools();

         if (filterByuser)
         {
            if (user.ProvinceId > 0)
            {
               provinces = provinces.Where(x => x.CompanyId == user.ProvinceId).ToList();
            }
            if (user.AreaId > 0)
            {
               areas = areas.Where(x => x.CompanyId == user.AreaId).ToList();
            }
            if (user.CompanyId > 0)
            {
               schools = schools.Where(x => x.CompanyId == user.CompanyId).ToList();
            }
         }

         ViewBag.Provinces = provinces;
         ViewBag.Areas = areas;
         ViewBag.Companies = schools;

         ViewBag.Actives = ResSettings.SettingsInSession.Actives;

         ViewBag.ProvincesDic = CrosourceController.GetStrengthDict(areas);
         ViewBag.AreasDic = CrosourceController.GetStrengthDict(areas);
         ViewBag.SchoolsDic = CrosourceController.GetStrengthDict(schools);
         
      }


      

   }

}