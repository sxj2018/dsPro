using Code.DbContexts;
using Code.SysModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Code.Auth
{
    public class AuthConfirm : IDisposable
    {
        private SysDbServiceContext _sysctx;
        public AuthConfirm()
        {
            try
            {
                _sysctx = new SysDbServiceContext();
                
            }catch(Exception e){
            }
        }

        public async Task<bool> RegisterUser(Sys_User userModel)
        {

            _sysctx.Sys_User.Add(userModel);
            return await _sysctx.SaveChangesAsync() > 0;
        }


        public async Task<Sys_User> FindUser(string userName, string password)
        {
            try { 
            var user = _sysctx.Sys_User.SingleOrDefault(r => r.UserCode == userName && r.Password == password);
            if (user == null) return null;
            return user;
                }
            catch(Exception e){
                return null;
            }
        }

        //public async Task<bool> AddRefreshToken(Sys_RefreshToken token)
        //{

        //    var existingToken = _sysctx.Sys_RefreshToken.SingleOrDefault(r => r.Subject == token.Subject);

        //    if (existingToken != null)
        //    {
        //        var result = await RemoveRefreshToken(existingToken.Id);
        //    }

        //    _sysctx.Sys_RefreshToken.Add(token);

        //    return await _sysctx.SaveChangesAsync() > 0;
        //}

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
           // var refreshToken = _sysctx.Sys_RefreshToken.SingleOrDefault(r=>r.Id==refreshTokenId);

            var refreshToken = DapperContext.QueryNormal(string.Format("select * from Sys_RefreshToken where Id={0}",refreshTokenId));
            if (refreshToken.total>0)
            {
                return await DapperContext.ExecuteAsync(string.Format("delete from Sys_RefreshToken where Id={0}", refreshTokenId)) > 0;
               // _sysctx.Sys_RefreshToken.Remove(refreshToken);
                //return await _sysctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        //public async Task<bool> RemoveRefreshToken(Sys_RefreshToken refreshToken)
        //{
        //    _sysctx.Sys_RefreshToken.Remove(refreshToken);
        //    return await _sysctx.SaveChangesAsync() > 0;
        //}

        //public async Task<Sys_RefreshToken> FindRefreshToken(string refreshTokenId)
        //{
        //    var refreshToken =  _sysctx.Sys_RefreshToken.SingleOrDefault(r=>r.Id==refreshTokenId);

        //    return refreshToken;
        //}

        public void Dispose()
        {
            _sysctx.Dispose(); 

        }

    }
}