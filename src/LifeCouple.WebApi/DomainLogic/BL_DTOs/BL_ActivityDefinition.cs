using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.WebApi.DomainLogic.BL_DTOs
{
    /// <summary>
    /// Holds information that is Common for each type of Activity. 
    /// </summary>
    public class BL_ActivityDefinition
    {
        /// <summary>
        /// Key of the Activity
        /// </summary>
        public BL_ActivityTypeEnum TypeOfActivity { get; set; }

        /// <summary>
        /// Url for the Video
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        /// Comment shown on the screen where the video player reside
        /// </summary>
        public string VideoComment { get; set; }

        /// <summary>
        /// Objective text shown on the page that is common for all activities
        /// </summary>
        public string Objective { get; set; }

        //TODO: Need to figure out if this have a meaning or is simply text (e.g. will it impact indexes, etc.) see q 7 in https://lifecouple.visualstudio.com/LC90/_wiki/wikis/LC90.wiki?wikiVersion=GBwikiMaster&pagePath=%2FActivities
        /// <summary>
        /// Activity Level text shown on the page that is common for all activities
        /// </summary>
        public string ActivityLevel { get; set; }

        //TODO: Need to figure out if this have a meaning or is simply text (e.g. will it impact indexes, etc.) see q 7 in https://lifecouple.visualstudio.com/LC90/_wiki/wikis/LC90.wiki?wikiVersion=GBwikiMaster&pagePath=%2FActivities
        /// <summary>
        /// Format text shown on the page that is common for all activities
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Duration text shown on the page that is common for all activities
        /// </summary>
        public string DurationText { get; set; }

        /// <summary>
        /// LifeCouple Tip text shown on the page that is common for all activities
        /// </summary>
        public string Tip { get; set; }
    }
}
