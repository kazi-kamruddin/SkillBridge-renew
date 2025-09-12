using System.Collections.Generic;

namespace SkillBridge.Models
{
    public class ExploreViewModel
    {
        public List<PublicProfileViewModel> BestMatches { get; set; }
        public List<PublicProfileViewModel> PartialMatches { get; set; }
    }
}
