﻿using FinnanceControll.Exceptions;
using FinnanceControll.Helpers;
using FinnanceControll.Models.Common;
using FinnanceControll.Resources;
using Microsoft.AspNetCore.Mvc;

namespace FinnanceControll.Controllers
{
    public abstract class CustomBaseController<TController> : ControllerBase
    {
        private const int DeletedStatusCode = 204;
        private readonly ILogger<TController> _logger;

        private readonly Uri _uri;

        protected CustomBaseController(ILogger<TController> logger)
        {
            _logger = logger;

            var controllerUri = Consts.UriBase + typeof(TController).Name.Replace("Controller", string.Empty);

            _uri = new Uri(controllerUri, UriKind.Relative);
        }

        protected ActionResult<TPayload> WrappedOkExecute<TPayload, TParam>(Func<TParam, TPayload> func, TParam param)
        {
            try
            {
                var result = func.Invoke(param);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogMessageHelper.GetRawExceptionTreatedFormattedMessage<TController>(),
                    ex
                );

                return Problem(ResponseMessages.InternalServerError);
            }
        }

        protected ActionResult<TPayload> WrappedCreatedExecute<TPayload, TParam>(Func<TParam, TPayload> func, TParam param)
        {
            try
            {
                var result = func.Invoke(param);

                return Created(_uri, result);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.ValidationFailures);
            }
            catch (Exception)
            {
                //_logger.LogError(
                //    LogMessageHelper.GetRawExceptionTreatedFormattedMessage<TController>(),
                //    ex
                //);

                return Problem(ResponseMessages.InternalServerError);
            }
        }

        protected ActionResult WrappedDeletedExecute<TParam>(Action<TParam> action, TParam param)
        {
            try
            {
                action.Invoke(param);

                return StatusCode(DeletedStatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogMessageHelper.GetRawExceptionTreatedFormattedMessage<TController>(),
                    ex
                );

                return Problem(ResponseMessages.InternalServerError);
            }
        }
    }
}