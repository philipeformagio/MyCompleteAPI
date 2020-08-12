using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.ViewModels
{
    public class EnderecoViewModel
    {
        [Key]
        public Guid Key { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(200, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 2)]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(50, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 1)]
        public string Numero { get; set; }

        public string Complemento { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(8, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 8)]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(100, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 2)]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(100, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 2)]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Field {0} required.")]
        [StringLength(50, ErrorMessage = "Field {0} must have between {2} and {1} characters", MinimumLength = 2)]
        public string Estado { get; set; }

        public Guid FornecedorId { get; set; }
    }
}
