using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DevIO.Api.Extentions.CustomAuthorization;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;
        public ProdutosController(INotificador notificador,
                                  IProdutoService produtoService,
                                  IProdutoRepository produtoRepository,
                                  IMapper mapper,
                                  IUser user) : base(notificador, user)
        {
            this._produtoRepository = produtoRepository;
            this._produtoService = produtoService;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> GetAll()
        {
            var produto = await _produtoRepository.ObterProdutosFornecedores();
            var protudoViewModel = _mapper.Map<IEnumerable<ProdutoViewModel>>(produto);
            return protudoViewModel;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> GetById(Guid id)
        {
            var produtoViewModel = await this.GetProductById(id);

            if (produtoViewModel == null) return NotFound();

            return produtoViewModel;
        }


        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Post(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imageName = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
            if (!this.FileUpload(produtoViewModel.ImagemUpload, imageName))
            {
                return CustomResponse(produtoViewModel);
            }

            produtoViewModel.Imagem = imageName;
            var produto = _mapper.Map<Produto>(produtoViewModel);
            await _produtoService.Adicionar(produto);

            return CustomResponse(produtoViewModel);
        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost("AddBigFile")]
        public async Task<ActionResult<ProdutoViewModel>> Post2(ProdutoImagemViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgPrefix = Guid.NewGuid() + "_";
            if (!await this.FileUpload2(produtoViewModel.ImagemUpload, imgPrefix))
            {
                return CustomResponse(produtoViewModel);
            }

            produtoViewModel.Imagem = imgPrefix + produtoViewModel.ImagemUpload.FileName;
            var produto = _mapper.Map<Produto>(produtoViewModel);
            await _produtoService.Adicionar(produto);

            return CustomResponse(produtoViewModel);
        }


        //[DisableRequestSizeLimit] // no limit
        [RequestSizeLimit(40000000)] // limit of 40mb
        [HttpPost("imagem")]
        public async Task<ActionResult> AddImage(IFormFile file)
        {
            return Ok(file);
        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id) return NotFound();

            var produtoAtualizacao = await this.GetProductById(id);

            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (produtoViewModel.ImagemUpload != null)
            {
                var imagemName = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
                if (!this.FileUpload(produtoViewModel.ImagemUpload, imagemName))
                {
                    return CustomResponse(ModelState);
                }

                produtoAtualizacao.Imagem = imagemName;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;
            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            return CustomResponse(produtoViewModel);
        }


        [ClaimsAuthorize("Produto", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Delete(Guid id)
        {
            var produto = await this.GetProductById(id);

            if (produto == null) return NotFound();

            await _produtoService.Remover(id);

            return CustomResponse(produto);
        }


        #region .: Private Methods :.
        private async Task<ProdutoViewModel> GetProductById(Guid id)
        {
            var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
            return produto;
        }

        private bool FileUpload(string file, string imgName)
        {
            if (string.IsNullOrEmpty(file))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(file);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgName);

            if(System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);
            return true;
        }

        private async Task<bool> FileUpload2(IFormFile file, string imgPrefix)
        {
            if (file ==null || file.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefix + file.FileName);

            if (System.IO.File.Exists(path))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return true;
        }
        #endregion
    }
}
