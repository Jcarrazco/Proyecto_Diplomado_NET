using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IndigoAssits.Core.Dtos
{
    [DataContract]
    public class CategoriaDto
    {
        [DataMember]
        public int IdCategoria { get; set; }
        [DataMember]
        public string Categoria { get; set; }
    }
}
