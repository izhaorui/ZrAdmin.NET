﻿using Infrastructure.Attribute;
using Infrastructure.Enums;
using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using ZR.Admin.WebApi.Filters;
using ZR.Common;
using ZR.Model;
using ZR.Model.System;
using ZR.Model.Vo;
using ZR.Service.IService;

namespace ZR.Admin.WebApi.Controllers.System
{
    /// <summary>
    /// 数据字典信息
    /// </summary>
    [Verify]
    [Route("system/dict/type")]
    public class SysDictTypeController : BaseController
    {
        private readonly ISysDictService SysDictService;

        public SysDictTypeController(ISysDictService sysDictService)
        {
            SysDictService = sysDictService;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pagerInfo"></param>
        /// <returns></returns>
        [ActionPermissionFilter(Permission = "system:dict:list")]
        [HttpGet("list")]
        public IActionResult List([FromQuery] SysDictType dict, [FromQuery] PagerInfo pagerInfo)
        {
            var list = SysDictService.SelectDictTypeList(dict, pagerInfo);

            var vm = new VMPageResult<SysDictType>(list, pagerInfo);
            return SUCCESS(vm, TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 查询字典类型详细
        /// </summary>
        /// <param name="dictId"></param>
        /// <returns></returns>
        [HttpGet("{dictId}")]
        [ActionPermissionFilter(Permission = "system:dict:query")]
        public IActionResult GetInfo(long dictId = 0)
        {
            return SUCCESS(SysDictService.GetFirst(f => f.DictId == dictId));
        }

        /// <summary>
        /// 添加字典类型
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [ActionPermissionFilter(Permission = "system:dict:add")]
        [Log(Title = "字典操作", BusinessType = BusinessType.INSERT)]
        [HttpPost("edit")]
        public IActionResult Add([FromBody] SysDictType dict)
        {
            if (UserConstants.NOT_UNIQUE.Equals(SysDictService.CheckDictTypeUnique(dict)))
            {
                return OutputJson(ApiResult.Error($"新增字典'{dict.DictName}'失败，字典类型已存在"));
            }
            //设置添加人
            dict.Create_by = HttpContext.User.Identity.Name;
            dict.Create_time = DateTime.Now;
            return SUCCESS(SysDictService.InsertDictType(dict));
        }

        /// <summary>
        /// 修改字典类型
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [ActionPermissionFilter(Permission = "system:dict:edit")]
        [Log(Title = "字典操作", BusinessType = BusinessType.UPDATE)]
        [Route("edit")]
        [HttpPut]
        public IActionResult Edit([FromBody] SysDictType dict)
        {
            if (UserConstants.NOT_UNIQUE.Equals(SysDictService.CheckDictTypeUnique(dict)))
            {
                return OutputJson(ApiResult.Error($"修改字典'{dict.DictName}'失败，字典类型已存在"));
            }
            //设置添加人
            dict.Update_by = HttpContext.User.Identity.Name;
            return SUCCESS(SysDictService.UpdateDictType(dict));
        }

        /// <summary>
        /// 删除字典类型
        /// </summary>
        /// <returns></returns>
        [ActionPermissionFilter(Permission = "system:dict:remove")]
        [Log(Title = "删除字典类型", BusinessType = BusinessType.DELETE)]
        [HttpDelete("{ids}")]
        public IActionResult Remove(string ids)
        {
            long[] idss = Tools.SpitLongArrary(ids);

            return SUCCESS(SysDictService.DeleteDictTypeByIds(idss));
        }
    }
}