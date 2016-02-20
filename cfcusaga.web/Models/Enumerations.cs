using System.ComponentModel.DataAnnotations;

namespace Cfcusaga.Web.Models
{
    public enum TshirtSizesEnums
    {
        [Display(Name = "Adult Small")]
        AdultSmall = 1,
        [Display(Name = "Adult Medium")]
        AdultMedium,
        [Display(Name = "Adult Large")]
        AdultLarge,
        [Display(Name = "Adult X-Large")]
        AdultXLarge
    }
}