using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeCouple.WebApi.Pages.MediaPlayer
{
    public class MediaPlayerPageController : Controller
    {
        /// <summary>
        /// 
        /// Url: 
        /// https://webapilcprod.azurewebsites.net/MediaPlayerPage
        /// https://webapilcprod.azurewebsites.net/MediaPlayerPage?width=640&height=360
        /// Note: height and width are optional. The video aspect ratio is 1.778 so other height/width examples without any black borders are 500/281 and 1024/579
        /// 
        /// <!--*****START OF Azure Media Player Scripts*****-->
        /// <!--Note: DO NOT USE the "latest" folder in production.Replace "latest" with a version number like "1.0.0"-->
        /// <!--EX:<script src = "//amp.azure.net/libs/amp/1.0.0/azuremediaplayer.min.js" ></ script > -->
        /// < !--Azure Media Player versions can be queried from //aka.ms/ampchangelog-->
        /// <link href="https:///amp.azure.net/libs/amp/2.1.7/skins/amp-default/azuremediaplayer.min.css" rel="stylesheet">
        /// <script src = "https:///amp.azure.net/libs/amp/2.1.7/azuremediaplayer.min.js" ></ script >
        /// < !--*****END OF Azure Media Player Scripts*****-->
        /// <video id="azuremediaplayer" class="azuremediaplayer amp-default-skin amp-big-play-centered" controls autoplay width="640" height="360" poster="" data-setup='{}' tabindex="0">
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Index([FromQuery(Name = "height")] string height, [FromQuery(Name = "width")] string width)
        {
            if(!string.IsNullOrEmpty(height) && int.TryParse(height, out var h))
            {
                ViewData["Height"] = $"{h}";
            }

            if (!string.IsNullOrEmpty(width) && int.TryParse(width, out var w))
            {
                ViewData["Width"] = $"{w}";
            }

            return View("/Pages/MediaPlayer/Index.cshtml");
        }
    }
}
