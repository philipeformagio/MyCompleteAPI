using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.ViewModels
{
    public class FornecedorViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(100, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 2)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(14, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 11)]
        public string Documento { get; set; }

        public int TipoFornecedor { get; set; }

        public EnderecoViewModel Endereco { get; set; }
        public bool Ativo { get; set; }

        /* EF Relations */
        public IEnumerable<ProdutoViewModel> Produtos { get; set; }
    }
}
