using DevIO.Api.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.ViewModels
{
    [ModelBinder(typeof(JsonWithFilesFormDataModelBinder), Name = "produto")]
    public class ProdutoImagemViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        public Guid FornecedorId { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(200, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 2)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(1000, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 2)]
        public string Descricao { get; set; }
        
        public string Imagem { get; set; }
        
        public IFormFile ImagemUpload { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        public decimal Valor { get; set; }
        
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }
        
        public bool Ativo { get; set; }

        [ScaffoldColumn(false)]
        public string NomeFornecedor { get; set; }
    }
}
