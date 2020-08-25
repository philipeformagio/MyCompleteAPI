using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DevIO.Api.Extentions.CustomAuthorization;

namespace DevIO.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;
        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      IEnderecoRepository enderecoRepository,
                                      IFornecedorService fornecedorService,
                                      IMapper mapper,
                                      INotificador notificador,
                                      IUser user) : base(notificador, user)
        {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        // Can be done in the both aways
        //public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> GetAll()
        //{
        //    var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
        //    return Ok(fornecedores);
        //}
        [AllowAnonymous]
        [HttpGet]        
        public async Task<IEnumerable<FornecedorViewModel>> GetAll()
        {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return fornecedores;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> GetById(Guid id)
        {
            var fornecedor = await this.ObterFornecedoresProdutosEndereco(id);

            if (fornecedor == null) return NotFound();

            return fornecedor;
        }

        [HttpGet("get-address/{id:guid}")]
        public async Task<EnderecoViewModel> GetAddressById(Guid id)
        {
            var enderecoViewModel = _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
            return enderecoViewModel;
        }

        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Post(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
            await _fornecedorService.Adicionar(fornecedor);

            return CustomResponse(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Put(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            // either way can be used
            //if (id != fornecedorViewModel.Id) return BadRequest();
            if (id != fornecedorViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(fornecedorViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
            await _fornecedorService.Atualizar(fornecedor);

            return CustomResponse(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("update-address/{id:guid}")]
        public async Task<ActionResult<EnderecoViewModel>> Put(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id) return BadRequest();

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var endereco = _mapper.Map<Endereco>(enderecoViewModel);
            await _fornecedorService.AtualizarEndereco(endereco);

            return CustomResponse(enderecoViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Delete(Guid id)
        {
            var fornecedorViewModel = await this.ObterFornecedoresEndereco(id);

            if (fornecedorViewModel == null) return NotFound();
            await _fornecedorService.Remover(id);

            return CustomResponse(fornecedorViewModel);
        }


        #region .: Private Methods :.
        private async Task<FornecedorViewModel> ObterFornecedoresProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedoresEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }
        #endregion
    }
}
