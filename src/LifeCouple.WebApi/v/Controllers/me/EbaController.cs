using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCouple.DTO.v;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LifeCouple.WebApi.v.Controllers.me
{
    [Produces("application/json")]
    [Route("api/userprofiles/me/eba")]
    public class EbaController : Controller
    {
        BusinessLogicEba _bl;
        private readonly IConfiguration _config;

        public EbaController(BusinessLogicEba businessLogic, IConfiguration config)
        {
            _bl = businessLogic;
            _config = config;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int nrOfRecentTransactions = 3)
        {
            try
            {
                var jwtPayloadInfo = this.GetJwtPayloadInfo();
                var userProfileId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userProfileId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var bl_ebaResponse = await _bl.GetEbaTransactions_ByUserIdAsync(userProfileId);
                var response = new EbaResponseInfo
                {
                    EbaPointOptions = getEbaPointOptions(),
                    EbaPointsBalance = bl_ebaResponse.EbaPointsBalance,
                    EbaPointsDeposited = bl_ebaResponse.EbaPointsDeposited,
                    RecentTransactions = getRecentTransactions()
                };

                List<EbaTransaction> getRecentTransactions()
                {
                    var r = new List<EbaTransaction>();
                    foreach (var item in bl_ebaResponse.RecentTransactions)
                    {
                        r.Add(new EbaTransaction { Comment = item.Comment, FirstName = item.FirstName, Id = item.Id, Point = item.Point, Posted = item.Posted, TypeOfTransaction = EnumMapper.From(item.TypeOfTransaction) });
                    }
                    return r;
                }

                List<EbaPointOption> getEbaPointOptions()
                {
                    var r = new List<EbaPointOption>();
                    foreach (var item in bl_ebaResponse.EbaPointOptions)
                    {
                        r.Add(new EbaPointOption { Text = item.Text, Value = item.PointValue });
                    }

                    return r;
                }


                return Json(response);

            }
            catch (BusinessLogicException ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return this.ApiErrorMessage400BadRequest(ex);
            }
            catch (Exception ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EbaRequestInfo request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.ApiErrorMessage400BadRequest(ModelState);
                }

                var jwtPayloadInfo = this.GetJwtPayloadInfo();
                var userProfileId = await _bl.GetCachedUserId_byExternalReferenceIdAsync(jwtPayloadInfo.ExtReferenceId);
                if (userProfileId == null)
                {
                    return this.ApiErrorMessage400BadRequestUserIdInTokenNotFound(jwtPayloadInfo.ExtReferenceId);
                }

                var bl_dto = new BL_EbaTransactionRequest
                {
                    Comment = request.Comment,
                    PointsToDeposit = request.PointsToDeposit,
                    FromUserprofileId = userProfileId,
                };

                await _bl.SetEbaTransactionAsync(bl_dto);

                return this.ApiPostMessage204NotContent();

            }
            catch (BusinessLogicException ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return this.ApiErrorMessage400BadRequest(ex);
            }
            catch (Exception ex)
            {
                HttpContext.Items.Add("ex", ex); //For instrumentation
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

    }
}