﻿using Infrastructure.Attribute;
using SqlSugar;
using System.Collections.Generic;
using ZR.Model.System;

namespace ZR.Repository.System
{
    /// <summary>
    /// 角色操作类
    /// </summary>
    [AppService(ServiceLifetime = LifeTime.Transient)]
    public class SysRoleRepository : BaseRepository
    {
        /// <summary>
        /// 根据条件分页查询角色数据
        /// </summary>
        /// <returns></returns>
        public List<SysRole> SelectRoleList(SysRole sysRole)
        {
            return Db.Queryable<SysRole>()
                .Where(role => role.DelFlag == "0")
                .WhereIF(!string.IsNullOrEmpty(sysRole.RoleName), role => role.RoleName.Contains(sysRole.RoleName))
                .WhereIF(!string.IsNullOrEmpty(sysRole.Status), role => role.Status == sysRole.Status)
                .WhereIF(!string.IsNullOrEmpty(sysRole.RoleKey), role => role.RoleKey == sysRole.RoleKey)
                .OrderBy(role => role.RoleSort)
                .ToList();
        }

        /// <summary>
        /// 根据用户查询
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SysRole> SelectRolePermissionByUserId(long userId)
        {
            return Db.Queryable<SysRole>()
                .Where(role => role.DelFlag == "0")
                .Where(it => SqlFunc.Subqueryable<SysUserRole>().Where(s => s.UserId == userId).Any())
                .OrderBy(role => role.RoleSort)
                .ToList();
        }

        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <returns></returns>
        public List<SysRole> SelectRoleAll()
        {
            return Db.Queryable<SysRole>().OrderBy(it => it.RoleSort).ToList();
        }

        /// <summary>
        /// 通过角色ID查询角色
        /// </summary>
        /// <param name="roleId">角色编号</param>
        /// <returns></returns>
        public SysRole SelectRoleById(long roleId)
        {
            return Db.Queryable<SysRole>().InSingle(roleId);
        }

        /// <summary>
        /// 通过角色ID删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int DeleteRoleByRoleIds(long[] roleId)
        {
            return Db.Deleteable<SysRole>().In(roleId).ExecuteCommand();
        }

        /// <summary>
        /// 获取用户所有角色信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SysRole> SelectUserRoleListByUserId(long userId)
        {
            return Db.Queryable<SysUserRole, SysRole>((ur, r) => new SqlSugar.JoinQueryInfos(
                    SqlSugar.JoinType.Left, ur.RoleId == r.RoleId
                )).Where((ur, r) => ur.UserId == userId)
                .Select((ur, r) => r).ToList();
        }

        #region 用户角色对应菜单 用户N-1角色

        /// <summary>
        /// 根据角色获取菜单id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<SysRoleMenu> SelectRoleMenuByRoleId(long roleId)
        {
            return Db.Queryable<SysRoleMenu>().Where(it => it.Role_id == roleId).ToList();
        }

        /// <summary>
        /// 批量插入用户菜单
        /// </summary>
        /// <param name="sysRoleMenus"></param>
        /// <returns></returns>
        public int AddRoleMenu(List<SysRoleMenu> sysRoleMenus)
        {
            return Db.Insertable(sysRoleMenus).ExecuteCommand();
        }

        /// <summary>
        /// 删除角色与菜单关联
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int DeleteRoleMenuByRoleId(long roleId)
        {
            return Db.Deleteable<SysRoleMenu>().Where(it => it.Role_id == roleId).ExecuteCommand();
        }

        #endregion

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="sysRole"></param>
        /// <returns></returns>
        public long InsertRole(SysRole sysRole)
        {
            sysRole.Create_time = Db.GetDate();
            return Db.Insertable(sysRole).ExecuteReturnBigIdentity();
        }

        /// <summary>
        /// 修改用户角色
        /// </summary>
        /// <param name="sysRole"></param>
        /// <returns></returns>
        public int UpdateSysRole(SysRole sysRole)
        {
            var db = Db;
            sysRole.Update_time = db.GetDate();

            return db.Updateable<SysRole>()
            .SetColumns(it => it.Update_time == sysRole.Update_time)
            .SetColumns(it => it.Remark == sysRole.Remark)
            .SetColumns(it => it.Update_by == sysRole.Update_by)
            .SetColumnsIF(!string.IsNullOrEmpty(sysRole.RoleName), it => it.RoleName == sysRole.RoleName)
            .SetColumnsIF(!string.IsNullOrEmpty(sysRole.RoleKey), it => it.RoleKey == sysRole.RoleKey)
            .SetColumnsIF(sysRole.RoleSort >= 0, it => it.RoleSort == sysRole.RoleSort)
            .Where(it => it.RoleId == sysRole.RoleId)
            .ExecuteCommand();
        }

        /// <summary>
        /// 更改角色权限状态
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateRoleStatus(SysRole role)
        {
            return Db.Updateable(role).UpdateColumns(t => new { t.Status }).ExecuteCommand();
        }

        /// <summary>
        /// 检查角色权限是否存在
        /// </summary>
        /// <param name="roleKey">角色权限</param>
        /// <returns></returns>
        public SysRole CheckRoleKeyUnique(string roleKey)
        {
            return Db.Queryable<SysRole>().Where(it => it.RoleKey == roleKey).Single();
        }

        /// <summary>
        /// 校验角色名称是否唯一
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        public SysRole CheckRoleNameUnique(string roleName)
        {
            return Db.Queryable<SysRole>().Where(it => it.RoleName == roleName).Single();
        }
    }
}