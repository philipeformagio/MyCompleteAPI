using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace DevIO.Api.V1.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected Guid UsuarioId { get; set; }
        protected bool UsuarioAutenticado { get; set; }


        private readonly INotificador _notificador;
        public readonly IUser AppUser;
        public MainController(INotificador notificador,
                              IUser appUser)
        {
            this._notificador = notificador;
            this.AppUser = appUser;

            if (appUser.IsAuthenticated())
            {
                UsuarioAutenticado = true;
                UsuarioId = appUser.GetUserId();
            }
        }

        /// <summary>
        /// Means if the method is going to return a 404(badRequest) or not
        /// In case it's true will always return a 200(ok) result
        /// </summary>
        /// <returns></returns>
        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificador.ObterNotificacoes().Select(x => x.Mensagem)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in errors)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(errorMsg);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }
    }
}
