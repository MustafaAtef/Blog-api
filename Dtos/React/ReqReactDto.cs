using BlogApi.Entities;
using BlogApi.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.React {
    public class ReqReactDto {
        [BindNever]
        public int PostId { get; set; }

        [Required(ErrorMessage ="Reaction type is requied!")]
        [ValidEnumTypeValue(typeof(ReactiontType), ErrorMessage ="Invalid reation type")]
        public int? ReactionType { get; set; }
    }
}
