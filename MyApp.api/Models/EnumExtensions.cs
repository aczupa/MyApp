using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MyApp.api.Models
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            return value.GetType()
                .GetField(value.ToString())?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName() ?? value.ToString();
        }

        public static bool EstMonegasque(this FormeJuridique forme) => forme is
    FormeJuridique.EntrepriseIndividuelleMonaco or FormeJuridique.SARLMonaco
    or FormeJuridique.SAM or FormeJuridique.SNCMonaco
    or FormeJuridique.SCSMonaco or FormeJuridique.SocieteCivileMonaco;
    }
}